using System.Threading.Tasks;
using Abp.Dependency;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.UserAuth.Entities;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth.Sales
{
    public interface ISalesSoftwareProcessor : ITransientDependency
    {
        /// <summary>
        /// --0 未购买更高本版或是终身版
        /// --1 已经购买了当前版本的终身版
        /// --2 已经购买了更高版本
        /// </summary>
        /// <param name="user"></param>
        /// <param name="servicePriceId"></param>
        /// <returns></returns>
        Task<UserCanPurchaseCode> UserCanbyPruductService(UserInfo user, int servicePriceId);

        //  Task<bool> PurchaseSoftwareService(UserInfo user, SalesInfoModel model, int salesManId, UserRole userRole);
        Task<bool> PurchaseSoftService(UserInfo userInfo, SalesInfoModel model, bool isAgent);

        bool TestProductValues(int uid,int pid,string subscriptionOrderId, decimal productValue, decimal salesCost, out string msg);

        Task PurchaseSoftServiceOnline(UserInfo userInfo, SalesInfoModel salesInfo);
    }
}