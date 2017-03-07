using Jueci.ApiService.UserAuth.Dtos;

namespace Jueci.ApiService.UserAuth
{
    public class UserAuthAppService : IUserAuthAppService
    {
        public string UserAuthorize(UserAuthInput input)
        {
            return "OK";
        }
    }
}