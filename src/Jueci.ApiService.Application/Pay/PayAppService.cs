using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
using Newtonsoft.Json.Linq;

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


        public UserPayOrderInfo NewPayOrder(PayOrderInfoDto dto, PayOrderInput payOrderInput)
        {

            ServicePrice servicePrice = null;
            int? goodsId = null;
            string attachStr = null;
            if (dto.GoodsType != 0)
            {
                if (!dto.GoodsId.HasValue)
                {
                    LogHelper.Logger.Error("创建支付订单失败，原因：购买授权，商品Id不能为空");
                    throw new Exception("创建支付订单失败，原因：购买授权，商品Id不能为空");
                }
                servicePrice = _servicePriceRepository.Get(dto.GoodsId.Value);
                if (servicePrice == null)
                {
                    LogHelper.Logger.Error(string.Format("不存在Id为{0}的商品", dto.GoodsId.Value));
                    throw new Exception(string.Format("不存在Id为{0}的商品", dto.GoodsId.Value));
                }
                if (servicePrice.ServiceId != dto.GoodsType)
                {
                    LogHelper.Logger.Error(string.Format("商品Id{0}不属于服务{1}", dto.GoodsId, dto.GoodsType));
                    throw new Exception(string.Format("商品Id{0}不属于服务{1}", dto.GoodsId, dto.GoodsType));
                }

                if (!string.IsNullOrEmpty(payOrderInput.SubscriptionOrderId))
                {
                    if (!payOrderInput.SuggestCost.HasValue)
                    {
                        throw new Exception(string.Format("基于订单{0}升级，则相应的建议价不能为空", payOrderInput.SubscriptionOrderId));
                    }
                    JObject jObject = new JObject();
                    jObject["SubscriptionOrderId"] = payOrderInput.SubscriptionOrderId;
                    jObject["SuggestCost"] = payOrderInput.SuggestCost;
                    attachStr = jObject.ToString();
                }
                goodsId = servicePrice.Id;
            }

            try
            {
                var payConfig = _payConfigLoader.GetPayConfigInfo(dto.PayType, payOrderInput.AppCode);

                var userPayOrder = dto.MapTo<UserPayOrderInfo>();
                userPayOrder.Id = OrderHelper.GenerateNewId();
                userPayOrder.PayAppId = payConfig.AppId;
                userPayOrder.GoodsName = dto.GoodsType != 0 ? servicePrice?.AuthDesc : "在线充值";
                userPayOrder.Attach = attachStr;
                return userPayOrder;
            }
            catch (Exception e)
            {
                LogHelper.Logger.Error(e.Message);
                throw e;
            }
        }

        [UnitOfWork]
        public ResultMessage<UnifiedPayOrderOutput> UnifiedPayOrder(PayOrderInput input)
        {
            var userInfo = _userRepository.Get(input.Uid);
            if (userInfo == null)
            {
                LogHelper.Logger.Error(string.Format("不存在Id为{0}的用户", input.Uid));
                return new ResultMessage<UnifiedPayOrderOutput>(ResultCode.Fail, string.Format("不存在Id为{0}的用户", input.Uid));
            }
            try
            {
                var payType = ConvertHelper.StrigToEnum<PayType>(input.PayTypeStr);
                var orderInfoDto = input.MapTo<PayOrderInfoDto>();
                orderInfoDto.PayType = payType;
                orderInfoDto.PayMode = ConvertHelper.StrigToEnum<PayMode>(input.PayModeStr);
                var payOrderInfo = NewPayOrder(orderInfoDto, input);
                var payConfig = _payConfigLoader.GetPayConfigInfo(PayType.Wechat, input.AppCode);
                switch (payType)
                {
                    case PayType.Wechat:
                        return WxUnifiedPayOrder(payConfig, payOrderInfo, userInfo);

                    case PayType.AliPay:
                        throw new NotImplementedException();
                    default:
                        throw new Exception("没有该类型的支付方式");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("统一下单失败！");
                return new ResultMessage<UnifiedPayOrderOutput>(ResultCode.Fail, "统一下单失败！原因:" + ex.Message);
            }
        }

        

        private ResultMessage<UnifiedPayOrderOutput> WxUnifiedPayOrder(BasicPay payConfig, UserPayOrderInfo payOrderInfo, UserInfo userInfo)
        {
            var wxpayConfig = (Models.WxPay)payConfig;
            var serviceOrder = new ServiceOrder()
            {
                appid = wxpayConfig.AppId,
                mch_id = wxpayConfig.MchId,
                notify_url = wxpayConfig.NotifyUrl,
                trade_type = SetTradeTypeByPyMode(payOrderInfo.PayMode),
                body = payOrderInfo.GoodsName,
                total_fee = Convert.ToInt32(payOrderInfo.Cost * 100),
                out_trade_no = payOrderInfo.Id,
            };
            if (payOrderInfo.PayMode == PayMode.Mobile)
            {
                if (userInfo.WeChat == null)
                {
                    return new ResultMessage<UnifiedPayOrderOutput>(ResultCode.Fail, string.Format("id为{0}的用户还没有绑定微信公众号，无法使用微信公众号支付!", userInfo.Id));
                }
                serviceOrder.openid = userInfo.WeChat;
            }

            var wxpayData = _wxPayService.UnifiedOrder(serviceOrder, wxpayConfig);

            payOrderInfo.State = 1;
            payOrderInfo.UpdateTime = DateTime.Now;
            _userPayOrderRepository.Insert(payOrderInfo);

            var orderData = _wxPayService.GetPaySign(wxpayConfig, payOrderInfo, wxpayData);
            var unifiedPayOrderOutput = new UnifiedPayOrderOutput()
            {
                OrderData = payOrderInfo.PayMode == PayMode.App ? (object)(WxPaySignAppOptions)orderData : (WxPaySignMobileOptions)orderData,
                OrderId = payOrderInfo.Id,
            };

            return new ResultMessage<UnifiedPayOrderOutput>(ResultCode.Success, "统一下单成功", unifiedPayOrderOutput);
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