using System.Transactions;
using Abp.Dependency;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Pay.Lib;
using Jueci.ApiService.Pay.Models;

namespace Jueci.ApiService.Pay.AliPay
{
    public interface IAlipayRequest : ISingletonDependency
    {
        bool Wappay(AlipayOrderOptions options, Alipay alipayConfig, PayMode payMode, out object respData);

        AlipayData Query(string id, OrderType outTradeNo);
    }
}