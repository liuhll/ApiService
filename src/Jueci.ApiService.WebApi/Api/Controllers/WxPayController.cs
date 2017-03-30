using System;
using System.Threading.Tasks;
using System.Web;
using Abp.Domain.Repositories;
using Abp.Logging;
using Abp.WebApi.Controllers;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Pay;
using Jueci.ApiService.Pay.Dtos;
using Jueci.ApiService.Pay.Entities;
using Jueci.ApiService.Pay.Models;
using Jueci.ApiService.UserAuth;
using Jueci.ApiService.UserAuth.Dtos;
using Newtonsoft.Json.Linq;

namespace Jueci.ApiService.Api.Controllers
{
    public class WxPayController : AbpApiController
    {
        private readonly IRepository<UserPayOrderInfo, string> _userPayOrderRepository;
        private readonly IPayConfigLoader _payConfigLoader;
        private readonly IUserAuthAppService _userAuthAppService;
        public WxPayController(IRepository<UserPayOrderInfo, string> userPayOrderRepository, IPayConfigLoader payConfigLoader, IUserAuthAppService userAuthAppService)
        {
            _userPayOrderRepository = userPayOrderRepository;
            _payConfigLoader = payConfigLoader;
            _userAuthAppService = userAuthAppService;
        }

        public async Task<string> WxPayNotify()
        {
            LogHelper.Logger.Debug("开始回调支付接口");
            Senparc.Weixin.MP.TenPayLibV3.ResponseHandler payNotifyRepHandler =
                new Senparc.Weixin.MP.TenPayLibV3.ResponseHandler(HttpContext.Current);

            string return_code = payNotifyRepHandler.GetParameter("return_code"); //返回状态码
            string return_msg = payNotifyRepHandler.GetParameter("return_msg"); //返回信息

            string xml = "<xml>" +
                      "<return_code><![CDATA[{0}]]></return_code>" +
                      "<return_msg><![CDATA[{1}]]></return_msg>" +
                      "</xml>";
            if (return_code.ToUpper() != "SUCCESS")
            {
                xml = string.Format(xml, "FAIL", "Communication Failure");
                return xml;
            }

            var outTradeNo = payNotifyRepHandler.GetParameter("out_trade_no");
            var userPayOrder = _userPayOrderRepository.Get(outTradeNo);
            var payConfig = _payConfigLoader.GetPayConfigInfoByAppid<WxPay>(PayType.Wechat, userPayOrder.PayAppId);
            payNotifyRepHandler.SetKey(payConfig.Key);

            if (!payNotifyRepHandler.IsTenpaySign())
            {
                LogHelper.Logger.Debug("签名验证未通过");
                xml = string.Format(xml, "FAIL", "Sign Is Error");
            }

            if (payNotifyRepHandler.GetParameter("return_code").ToString() != "SUCCESS" ||
            payNotifyRepHandler.GetParameter("result_code").ToString() != "SUCCESS")
            {
                //订单查询失败，则立即返回结果给微信支付后台
                xml = string.Format(xml, "FAIL", "Query Order Fail");
                return xml;
            }

            if (userPayOrder.GoodsType == 0)
            {

            }
            else
            {
                if (!userPayOrder.GoodsId.HasValue)
                {
                    xml = string.Format(xml, "FAIL", "产品Id为空，无法为指定的用户授权授权");
                    return xml;
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
                    Logger.Debug(string.Format("为用户{0}授权失败!原因:{1}", userPayOrder.Uid,authResult.Msg));
                    xml = string.Format(xml, "FAIL", authResult.Msg);
                    return xml;
                }
                Logger.Debug(string.Format("为用户{0}授权成功!",userPayOrder.Uid));
                xml = string.Format(xml, "SUCCESS", authResult.Msg);
            }

            return xml;
        }
    }
}