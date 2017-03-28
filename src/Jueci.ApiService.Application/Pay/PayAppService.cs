using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Logging;
using Jueci.ApiService.Common;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Common.Tools;
using Jueci.ApiService.Pay.Dtos;
using Jueci.ApiService.Pay.Entities;
using Jueci.ApiService.Pay.Lib;
using Jueci.ApiService.Pay.Models;
using Jueci.ApiService.Pay.WxPay;
using Jueci.ApiService.UserAuth.Entities;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.Pay
{
    public class PayAppService : IPayAppService
    {
        private readonly IRepository<UserPayOrderInfo, string> _userPayOrderRepository;
        private readonly IRepository<UserInfo> _userRepository;
        private readonly IRepository<ServicePrice> _servicePriceRepository;
        private readonly IPayConfigLoader _payConfigLoader;
        private readonly IWxPayService _wxPayService;

        public PayAppService(IRepository<UserPayOrderInfo, string> userPayOrderRepository,
            IRepository<UserInfo> userRepository,
            IRepository<ServicePrice> servicePriceRepository,
            IPayConfigLoader payConfigLoader,
            IWxPayService wxPayService)
        {
            _userPayOrderRepository = userPayOrderRepository;
            _userRepository = userRepository;
            _servicePriceRepository = servicePriceRepository;
            _payConfigLoader = payConfigLoader;
            _wxPayService = wxPayService;
        }

   
        public UserPayOrderInfo NewPayOrder(PayOrderInfoInput input, string appCode)
        {

            ServicePrice servicePrice = null;
            int? goodsId = null;
            if (input.GoodsType != 0)
            {
                if (!input.GoodsId.HasValue)
                {
                    LogHelper.Logger.Error("创建支付订单失败，原因：购买授权，商品Id不能为空");
                    throw new Exception("创建支付订单失败，原因：购买授权，商品Id不能为空");
                }
                servicePrice = _servicePriceRepository.Get(input.GoodsId.Value);
                if (servicePrice == null)
                {
                    LogHelper.Logger.Error(string.Format("不存在Id为{0}的商品", input.GoodsId.Value));
                    throw new Exception(string.Format("不存在Id为{0}的商品", input.GoodsId.Value));
                }
                if (servicePrice.ServiceId != input.GoodsType)
                {
                    LogHelper.Logger.Error(string.Format("商品Id{0}不属于服务{1}", input.GoodsId, input.GoodsType));
                    throw new Exception(string.Format("商品Id{0}不属于服务{1}", input.GoodsId, input.GoodsType));
                }
                goodsId = servicePrice.Id;
            }

            try
            {
                var payConfig = _payConfigLoader.GetPayConfigInfo(input.PayType, appCode);

                var userPayOrder = input.MapTo<UserPayOrderInfo>();
                userPayOrder.Id = OrderHelper.GenerateNewId();
                userPayOrder.PayAppId = payConfig.AppId;
                userPayOrder.GoodsName = input.GoodsType != 0 ? servicePrice?.AuthDesc : "在线充值";
                return userPayOrder;
            }
            catch (Exception e)
            {
                LogHelper.Logger.Error(e.Message);
                throw e;
            }
        }

        [UnitOfWork]
        public ResultMessage<WxPayOrderInfoOutput> WxPayUnifiedOrder(WxPayOrderInput input)
        {
            var userInfo = _userRepository.Get(input.Uid);
            if (userInfo == null)
            {
                LogHelper.Logger.Error(string.Format("不存在Id为{0}的用户", input.Uid));
                return new ResultMessage<WxPayOrderInfoOutput>(ResultCode.Fail, string.Format("不存在Id为{0}的用户", input.Uid));
            }

            var payOrderInfoInput = input.MapTo<PayOrderInfoInput>();
            payOrderInfoInput.PayType = PayType.Wechat;
            payOrderInfoInput.PayMode = ConvertHelper.StrigToEnum<PayMode>(input.PayModeStr);
            var payOrderInfo = NewPayOrder(payOrderInfoInput, input.AppCode);
            var payConfig = (Models.WxPay)_payConfigLoader.GetPayConfigInfo(PayType.Wechat, input.AppCode);
            var serviceOrder = new ServiceOrder()
            {
                appid = payConfig.AppId,
                mch_id = payConfig.MchId,
                notify_url = payConfig.NotifyUrl,
                trade_type = SetTradeTypeByPyMode(payOrderInfo.PayMode),
                body = payOrderInfo.GoodsName,
                total_fee = Convert.ToInt32(payOrderInfo.Cost * 100),
                out_trade_no = payOrderInfo.Id,
              
            };
            if (payOrderInfo.PayMode == PayMode.Mobile)
            {
                if (userInfo.WeChat == null)
                {
                    return new ResultMessage<WxPayOrderInfoOutput>(ResultCode.Fail, "id为{0}的用户还没有绑定微信公众号，无法使用微信公众号支付!");
                }
                serviceOrder.openid = userInfo.WeChat;
            }

            var wxpayData = _wxPayService.UnifiedOrder(serviceOrder, payConfig);

            payOrderInfo.State = 1;
            payOrderInfo.UpdateTime = DateTime.Now;
            _userPayOrderRepository.Insert(payOrderInfo);
            var output = new WxPayOrderInfoOutput()
            {
                PrepayId = wxpayData.GetValue("prepay_id").ToString(),
                OrderId = payOrderInfo.Id,
            };
             
            return new ResultMessage<WxPayOrderInfoOutput>(ResultCode.Success,"创建支付订单成功",output);

        }

        private string SetTradeTypeByPyMode(PayMode payMode)
        {
            string trade_type = String.Empty;
            switch (payMode)
            {
                case PayMode.App:
                    trade_type = WxPayConfig.TRADE_TYPE_APP;
                    break;
                case PayMode.Mobile:
                    trade_type = WxPayConfig.TRADE_TYPE_JSAPI;
                    break;
                case PayMode.PC:
                    trade_type = WxPayConfig.TRADE_TYPE_NATIVE;
                    break;
            }
            return trade_type;
        }
    }
}