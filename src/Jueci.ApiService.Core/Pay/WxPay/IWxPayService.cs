using Abp.Dependency;
using Jueci.ApiService.Pay.Entities;
using Jueci.ApiService.Pay.Lib;
using Jueci.ApiService.Pay.Models;

namespace Jueci.ApiService.Pay.WxPay
{
    public interface IWxPayService : ITransientDependency
    {
        WxPayData UnifiedOrder(ServiceOrder serviceOrder, Models.WxPay wxPayConfig, int timeOut = 10);

        WxPaySignOptionsBasic GetPaySign(Models.WxPay payConfig, UserPayOrderInfo payOrderInfo,WxPayData wxPayData);
    }
}