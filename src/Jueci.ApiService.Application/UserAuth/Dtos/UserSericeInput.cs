using Jueci.ApiService.Base.Dtos;

namespace Jueci.ApiService.UserAuth.Dtos
{
    public class UserSericeInput : BasicDto
    {
        /// <summary>
        /// 用户通行证
        /// </summary>
        public string UserPassport { get; set; }

        /// <summary>
        /// 服务Id
        /// </summary>
        public int ServiceId { get; set; }
    }
}