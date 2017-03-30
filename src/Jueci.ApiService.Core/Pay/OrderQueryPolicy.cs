using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Common.Tools;
using Jueci.ApiService.Pay.Lib;

namespace Jueci.ApiService.Pay
{
    public class OrderQueryPolicy : IOrderQueryPolicy
    {
        public WxPayData Orderquery(string orderId, OrderType orderType, Models.WxPay payConfig)
        {
            string queryApiAddress = "https://api.mch.weixin.qq.com/pay/orderquery";
            var wxPayData = new WxPayData(payConfig);
            wxPayData.SetValue("appid", payConfig.AppId);
            wxPayData.SetValue("mch_id", payConfig.MchId);
            wxPayData.SetValue(orderType == OrderType.OutTradeNo
                ? "out_trade_no" : "transaction_id", orderId);

            wxPayData.SetValue("nonce_str", NonceHelper.GenerateNonceStr());
            wxPayData.SetValue("sign", wxPayData.MakeSign());

            string xml = wxPayData.ToXml();
            var response = HttpService.Post(xml, queryApiAddress, false, 10);
            if (string.IsNullOrEmpty(response))
            {
                return null;
            }
            WxPayData result = new WxPayData(payConfig);
            result.FromXml(response);

            return result;
        }

        public AlipayData AliOrderQuery(string id, OrderType outTradeNo)
        {
            throw new NotImplementedException();
        }
    }
}
