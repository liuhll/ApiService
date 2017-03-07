using Abp.Application.Services;
using System.Web.Http;
using Jueci.ApiService.ApiAuthorization;
using Jueci.ApiService.Base;
using Jueci.ApiService.UserAuth.Dtos;

namespace Jueci.ApiService.UserAuth
{
    [JeuciAuthorize]
    public interface IUserAuthAppService : IApplicationService
    {
        [HttpPost]
        string UserAuthorize(UserAuthInput input);
    }
}