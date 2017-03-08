using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using System.Web.Http;
using Jueci.ApiService.ApiAuthorization;
using Jueci.ApiService.Base;
using Jueci.ApiService.Common;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.UserAuth.Dtos;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth
{
    [JeuciAuthorize]
    public interface IUserAuthAppService : IApplicationService
    {

        /// <summary>
        /// 获取用户可授权的服务列表
        /// </summary>
        /// <param name="uid">用户id</param>
        /// <param name="sid">服务Id</param>
        /// <param name="agentId">代理商Id</param>
        /// <param name="appId">AppId</param>
        /// <param name="sign">签名</param>
        /// <param name="timestamp">时间戳</param>
        /// <returns>用户可授权的服务信息</returns>
        /// <remarks>通过用户通行证和服务Id获取用户可授权的服务列表</remarks>        
        [HttpGet]
       Task<ResultMessage<IList<UserServicePriceOutput>>> UserServiceList(int uid,int sid, int? agentId, string appId,string sign,long timestamp);

        /// <summary>
        /// 用户服务授权
        /// </summary>
        /// <param name="input">输入参数</param>
        /// <returns></returns>
        [HttpPost]
        string UserAuthorize(UserAuthInput input);
    }
}