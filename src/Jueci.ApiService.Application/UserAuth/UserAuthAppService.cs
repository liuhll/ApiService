using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Jueci.ApiService.Common;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.UserAuth.Dtos;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth
{
    public class UserAuthAppService : IUserAuthAppService
    {
        private readonly IUserAuthServiceManager _authServiceManager;

        public UserAuthAppService(IUserAuthServiceManager authServiceManager)
        {
            _authServiceManager = authServiceManager;
        }


        public async Task<ResultMessage<IList<UserServicePriceOutput>>> UserServiceList(int uid, int sid, int? agentId, string appId, string sign, long timestamp)
        {
            try
            {
                var userPriceList = await _authServiceManager.UserServiceList(uid, sid, agentId);
                return new ResultMessage<IList<UserServicePriceOutput>>(userPriceList.MapTo<List<UserServicePriceOutput>>(),"OK");
            }
            catch (Exception e)
            {
                return new ResultMessage<IList<UserServicePriceOutput>>(ResultCode.Fail,e.Message);
            }
        }

        public string UserAuthorize(UserAuthInput input)
        {
            return "OK";
        }
    }
}