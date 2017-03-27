using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Logging;
using Jueci.ApiService.Common;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Common.Tools;
using Jueci.ApiService.Pay.Dtos;
using Jueci.ApiService.Pay.Entities;
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

        public PayAppService(IRepository<UserPayOrderInfo, string> userPayOrderRepository,
            IRepository<UserInfo> userRepository, 
            IRepository<ServicePrice> servicePriceRepository, 
            IPayConfigLoader payConfigLoader)
        {
            _userPayOrderRepository = userPayOrderRepository;
            _userRepository = userRepository;
            _servicePriceRepository = servicePriceRepository;
            _payConfigLoader = payConfigLoader;
        }

        public ResultMessage<PayOrderInfoOutput> NewPayOrder(PayOrderInfoInput input)
        {
            var userInfo = _userRepository.Get(input.Uid);
            if (userInfo == null)
            {
                LogHelper.Logger.Error(string.Format("不存在Id为{0}的用户", input.Uid));
                return new ResultMessage<PayOrderInfoOutput>(ResultCode.Fail,string.Format("不存在Id为{0}的用户",input.Uid));
            }
            ServicePrice servicePrice = null;
            int? goodsId = null;
            if (input.GoodsType != 0)
            {
                if (!input.GoodsId.HasValue)
                {
                    LogHelper.Logger.Error("创建支付订单失败，原因：购买授权，商品Id不能为空");
                    return new ResultMessage<PayOrderInfoOutput>(ResultCode.Fail,"创建支付订单失败，原因：购买授权，商品Id不能为空");
                }
                servicePrice = _servicePriceRepository.Get(input.GoodsId.Value);
                if (servicePrice == null)
                {
                    LogHelper.Logger.Error(string.Format("不存在Id为{0}的用户", input.Uid));
                    return new ResultMessage<PayOrderInfoOutput>(ResultCode.Fail, string.Format("不存在Id为{0}的用户", input.Uid));
                }
                if (servicePrice.ServiceId != input.GoodsType)
                {
                    LogHelper.Logger.Error(string.Format("商品Id{0}不属于服务{1}",input.GoodsId,input.GoodsType));
                    return new ResultMessage<PayOrderInfoOutput>(ResultCode.Fail, string.Format("商品Id{0}不属于服务{1}", input.GoodsId, input.GoodsType));
                }
                goodsId = servicePrice.Id;
            }

            try
            {
                var payType = ConvertHelper.StrigToEnum<PayType>(input.PayType);
                var appCode = ConvertHelper.StrigToEnum<AppCode>(input.AppCode);
                var payConfig = _payConfigLoader.GetPayConfigInfo(payType, appCode);
                var userPayOrder = new UserPayOrderInfo()
                {
                    Id = OrderHelper.GenerateNewId(),
                    Uid = input.Uid,
                    GoodsId = goodsId,
                    GoodsName = input.GoodsType == 0? "在线充值" : servicePrice?.AuthDesc,
                    Cost = input.Cost,
                    GoodsType = input.GoodsType,
                    PayAppId = payConfig.AppId,
                    PayType = payType,
                    PayMode = ConvertHelper.StrigToEnum<PayMode>(input.PayMode),
                    Remarks = input.Remarks,
                    State = 0,
                };
                var model = _userPayOrderRepository.Insert(userPayOrder);
                return new ResultMessage<PayOrderInfoOutput>(ResultCode.Success,"新增支付订单成功!",new PayOrderInfoOutput()
                {
                    PayOrderId = model.Id
                });
            }
            catch (Exception e)
            {
                LogHelper.Logger.Error(e.Message);
                return new ResultMessage<PayOrderInfoOutput>(ResultCode.Fail,e.Message);
            }
        }
    }
}