using Abp.Dependency;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Pay.Lib;

namespace Jueci.ApiService.Pay
{
    public interface IOrderQueryPolicy : ITransientDependency
    {
        WxPayData Orderquery(string orderId, OrderType type,Models.WxPay payConfig);
        AlipayData AliOrderQuery(string id, OrderType outTradeNo);
    }
}