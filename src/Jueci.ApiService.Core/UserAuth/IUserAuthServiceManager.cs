using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth
{
    public interface IUserAuthServiceManager : ITransientDependency
    {
        Task<IList<UserServicePrice>> UserServiceList(int uid, int sid,int? agentId);
    }
}