using System.Threading.Tasks;
using Abp.Dependency;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.Recharge
{
    public interface IRechargeService : ITransientDependency
    {
        Task<bool> UserRecharge(string orderId);
    }
}