using System.Collections.Generic;
using Abp.Dependency;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Pay.Models;

namespace Jueci.ApiService.Pay
{
    public interface IPayConfigLoader : ISingletonDependency
    {
        IList<Models.BasicPay> GetPayApps();

        T GetPayConfigInfo<T>(PayType payType, string appCode) where T : BasicPay;

        BasicPay GetPayConfigInfo(PayType payType, string appCode);
    }
}