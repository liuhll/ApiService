using Abp.Dependency;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.Pay
{
    public interface IPayService : ITransientDependency
    {
        string GetPayAppId(PayType payType,AppCode appCode);
    }
}