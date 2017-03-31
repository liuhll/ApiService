using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Logging;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Common.Tools;
using Jueci.ApiService.Pay;
using Jueci.ApiService.Pay.AliPay;
using Jueci.ApiService.Pay.Entities;
using Jueci.ApiService.Pay.Models;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.Recharge
{
    public class RechargeService : IRechargeService
    {
        private readonly IRepository<UserPayOrderInfo, string> _userPayOrderRepository;
        private readonly IOrderQueryPolicy _orderQueryPolicy;
        private readonly IPayConfigLoader _payConfigLoader;
        private readonly IRepository<UserInfo> _userInfoRepository;
        private readonly IRepository<UserRecharge, string> _userRechargeRepository;
        private readonly IAlipayRequest _alipayRequest;

        public RechargeService(IRepository<UserPayOrderInfo, string> userPayOrderRepository,
            IOrderQueryPolicy orderQueryPolicy,
            IPayConfigLoader payConfigLoader,
            IRepository<UserInfo> userInfoRepository,
            IRepository<UserRecharge, string> userRechargeRepository, IAlipayRequest alipayRequest)
        {
            _userPayOrderRepository = userPayOrderRepository;
            _orderQueryPolicy = orderQueryPolicy;
            _payConfigLoader = payConfigLoader;
            _userInfoRepository = userInfoRepository;
            _userRechargeRepository = userRechargeRepository;
            _alipayRequest = alipayRequest;
        }

        [UnitOfWork]
        public async Task<bool> UserRecharge(string orderId)
        {
            var userPayOrder = await _userPayOrderRepository.GetAsync(orderId);
            if (userPayOrder == null)
            {
                throw new Exception(string.Format("系统中不存在单号为{0}的支付订单,请确认您的支付单号是是否正确", orderId));
            }

            string transactionId = null;
            string tradeState = null;

            #region 微信支付

            if (userPayOrder.PayType == PayType.Wechat)
            {
                var payconfig = _payConfigLoader.GetPayConfigInfoByAppid<WxPay>(PayType.Wechat, userPayOrder.PayAppId);
                var wxPayData = _orderQueryPolicy.Orderquery(orderId, OrderType.OutTradeNo, payconfig);
                if (wxPayData.GetValue("return_code").ToString() != "SUCCESS" ||
                    wxPayData.GetValue("result_code").ToString() != "SUCCESS")
                {
                    throw new Exception(string.Format("单号为{0}的订单支付失败", orderId));
                }
                transactionId = wxPayData.GetValue("transaction_id").ToString();
                tradeState = wxPayData.GetValue("trade_state").ToString();

                if (tradeState == "SUCCESS") //交易成功
                {
                    try
                    {
                        var userInfo = _userInfoRepository.Get(userPayOrder.Uid);
                        var fee = Convert.ToDecimal(wxPayData.GetValue("total_fee"))/100;

                        userPayOrder.PayExtendInfo = wxPayData.ToJson();
                        userPayOrder.PayState = tradeState;
                        userPayOrder.PayOrderId = transactionId;
                        userPayOrder.State = 2;
                        _userPayOrderRepository.Update(userPayOrder);

                        _userRechargeRepository.Insert(new UserRecharge()
                        {
                            Id = OrderHelper.GenerateNewId(),
                            Cost = fee,
                            AdminId = null,
                            CreateTime = DateTime.Now,
                            OrderId = orderId,
                            Remarks = "微信公众号充值",
                            UId = userInfo.Id
                        });
                        userInfo.Fund += fee;
                        await _userInfoRepository.UpdateAsync(userInfo);
                        return true;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                else if (tradeState == "USERPAYING") //正在支付
                {


                    userPayOrder.PayExtendInfo = wxPayData.ToJson();
                    userPayOrder.PayState = tradeState;
                    userPayOrder.PayOrderId = transactionId;
                    _userPayOrderRepository.Update(userPayOrder);
                    LogHelper.Logger.Debug("订单正在支付");
                    return false;
                }
                else if (tradeState == "NOTPAY")
                {
                    //算作超时关闭订单
                    if (userPayOrder.CreateTime.AddMinutes(20) < DateTime.Now)
                    {

                        userPayOrder.PayExtendInfo = wxPayData.ToJson();
                        userPayOrder.PayState = tradeState;
                        userPayOrder.PayOrderId = transactionId;
                        userPayOrder.State = 3;
                        _userPayOrderRepository.Update(userPayOrder);

                        LogHelper.Logger.Debug("超时关闭订单");
                        return false;
                    }
                    else
                    {

                        userPayOrder.PayExtendInfo = wxPayData.ToJson();
                        userPayOrder.PayState = tradeState;
                        userPayOrder.PayOrderId = transactionId;

                        _userPayOrderRepository.Update(userPayOrder);
                        LogHelper.Logger.Debug("订单尚未支付，等待支付！");
                        return false;

                    }

                }
                // 其他状态不做处理
                return false;

            }

            #endregion

            #region 支付宝支付

            if (userPayOrder.PayType == PayType.AliPay)
            {
                var payConfig = _payConfigLoader.GetPayConfigInfoByAppid<Alipay>(PayType.AliPay, userPayOrder.PayAppId);
                var alipayData = _alipayRequest.Query(userPayOrder.Id, OrderType.OutTradeNo, payConfig);
                tradeState = alipayData.GetValue("trade_status");
                transactionId = string.Empty;
                if (alipayData.IsSet("trade_no"))
                {
                    transactionId = alipayData.GetValue("trade_no");
                }
                if (tradeState.Equals("TRADE_SUCCESS") || tradeState.Equals("TRADE_FINISHED"))
                {
                    try
                    {
                        var userInfo = _userInfoRepository.Get(userPayOrder.Uid);
                        var fee = Convert.ToDecimal(alipayData.GetValue("total_amount"));

                        userPayOrder.PayExtendInfo = alipayData.ToJson();
                        userPayOrder.PayState = tradeState;
                        userPayOrder.PayOrderId = transactionId;
                        userPayOrder.State = 2;
                        _userPayOrderRepository.Update(userPayOrder);

                        _userRechargeRepository.Insert(new UserRecharge()
                        {
                            Id = OrderHelper.GenerateNewId(),
                            Cost = fee,
                            AdminId = null,
                            CreateTime = DateTime.Now,
                            OrderId = orderId,
                            Remarks = "微信公众号充值",
                            UId = userInfo.Id
                        });
                        userInfo.Fund += fee;
                        await _userInfoRepository.UpdateAsync(userInfo);
                        return true;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                if (tradeState == "NOTPAY")
                {
                    //算作超时关闭订单
                    if (userPayOrder.CreateTime.AddMinutes(20) < DateTime.Now)
                    {

                        userPayOrder.PayExtendInfo = alipayData.ToJson();
                        userPayOrder.PayState = tradeState;
                        userPayOrder.PayOrderId = transactionId;
                        userPayOrder.State = 3;
                        _userPayOrderRepository.Update(userPayOrder);

                        LogHelper.Logger.Debug("超时关闭订单");
                        return false;
                    }
                    else
                    {

                        userPayOrder.PayExtendInfo = alipayData.ToJson();
                        userPayOrder.PayState = tradeState;
                        userPayOrder.PayOrderId = transactionId;

                        _userPayOrderRepository.Update(userPayOrder);
                        LogHelper.Logger.Debug("订单尚未支付，等待支付！");
                        return false;

                    }
                }
                return false;
            }

            #endregion

            return false;
        }
    }
}