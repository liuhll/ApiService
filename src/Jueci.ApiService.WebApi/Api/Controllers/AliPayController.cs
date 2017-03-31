using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Abp.Domain.Repositories;
using Abp.Logging;
using Abp.WebApi.Controllers;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Pay;
using Jueci.ApiService.Pay.Entities;
using Jueci.ApiService.Pay.Lib;
using Jueci.ApiService.Pay.Models;
using Jueci.ApiService.Recharge;
using Jueci.ApiService.UserAuth;
using Jueci.ApiService.UserAuth.Dtos;
using Newtonsoft.Json.Linq;

namespace Jueci.ApiService.Api.Controllers
{
    public class AliPayController : AbpApiController
    {
        private readonly IRepository<UserPayOrderInfo, string> _userPayOrderRepository;
        private readonly IPayConfigLoader _payConfigLoader;
        private readonly IUserAuthAppService _userAuthAppService;
        private readonly IRechargeService _rechargeService;

        public AliPayController(IRepository<UserPayOrderInfo, string> userPayOrderRepository,
            IPayConfigLoader payConfigLoader,
            IUserAuthAppService userAuthAppService,
            IRechargeService rechargeService)
        {
            _userPayOrderRepository = userPayOrderRepository;
            _payConfigLoader = payConfigLoader;
            _userAuthAppService = userAuthAppService;
            _rechargeService = rechargeService;
        }

        public async Task<string> AliPayNotify()
        {
            LogHelper.Logger.Debug("回调支付宝支付接口");
            var response = Request.CreateResponse(HttpStatusCode.OK);
            var alipayData = new AlipayData();
            var resultMsg = string.Empty;
            foreach (var key in HttpContext.Current.Request.Form.AllKeys)
            {
                alipayData.SetValue(key, HttpContext.Current.Request.Form[key]);
            }
            
            var outTradeNo = alipayData.GetValue("out_trade_no");
            var userPayOrder = _userPayOrderRepository.Get(outTradeNo);
            if (userPayOrder == null)
            {
                Logger.Error(string.Format("数据库中不存在单号为{0}的订单",outTradeNo));
                resultMsg = "fail";
                return resultMsg;
            }
            var payConfig = _payConfigLoader.GetPayConfigInfoByAppid<Alipay>(PayType.AliPay, userPayOrder.PayAppId);

            var signVerify = alipayData.SignVerified(payConfig);
            if (!signVerify)
            {
                Logger.Error("签名失败，这可能是假冒的回调");
                resultMsg = "fail";
                return resultMsg;
            }

            if (!VerifyOrder(alipayData,userPayOrder, payConfig))
            {
                resultMsg = "fail";
                return resultMsg;
            }

            if (userPayOrder.GoodsType == 0)
            {
            }
            else
            {
                if (!userPayOrder.GoodsId.HasValue)
                {
                    resultMsg = "fail";
                    return resultMsg;
                }

                var userAuthInput = new UserAuthInput()
                {
                    PId = userPayOrder.GoodsId.Value,
                    AppId = payConfig.AppId,
                    Cost = userPayOrder.Cost,
                    SalesWay = "Online",
                    Remarks =
                userPayOrder.PayMode == PayMode.App
                    ? "手机APP支付"
                    : userPayOrder.PayMode == PayMode.Mobile ? "微信公众号支付" : "PC端支付",
                    OrderId = userPayOrder.Id,

                };
                if (!string.IsNullOrEmpty(userPayOrder.Attach))
                {
                    var jObject = new JObject(userPayOrder.Attach);
                    userAuthInput.SubscriptionOrderId = jObject["SubscriptionOrderId"].ToString();
                    userAuthInput.SuggestCost = Convert.ToInt32(jObject["SuggestCost"]);
                }

                var authResult = await _userAuthAppService.UserAuthorize(userAuthInput);

                if (authResult.Code != ResultCode.Success)
                {
                    resultMsg = "fail";
                    return resultMsg;
                }
                Logger.Debug(string.Format("为用户{0}授权成功!", userPayOrder.Uid));
                resultMsg = "success";
            }
            return resultMsg;

        }

        private bool VerifyOrder(AlipayData alipayData, UserPayOrderInfo payOrder, Alipay alipayConfig)
        {
            if (Convert.ToDecimal(alipayData.GetValue("invoice_amount")) != payOrder.Cost)
            {
                LogHelper.Logger.Error("用户支付的金额与订单预付款金额不一致,交易失败");
                return false;
            }
            if (!alipayConfig.PId.Equals(alipayData.GetValue("seller_id")))
            {
                LogHelper.Logger.Error("seller_id不一致，交易失败");
                return false;
            }
            if (!AliPayConfig.APPID.Equals(alipayData.GetValue("app_id")))
            {
                LogHelper.Logger.Error("app_id不一致，交易失败");
                return false;
            }
            return true;

        }
    }
}