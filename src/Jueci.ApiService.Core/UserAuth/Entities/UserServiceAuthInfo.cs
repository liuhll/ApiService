using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class UserServiceAuthInfo : Entity
    {
        public UserServiceAuthInfo()
        {
            CreateTime = DateTime.Now;
        }

        public int UId { get; set; }

        public int SId { get; set; }

        public int AuthType { get; set; }

        public DateTime? AuthExpiration { get; set; }

        public int? PUid { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool IsActive
        {
            get
            {
                if (AuthExpiration == null)
                {
                    return true;
                }
                return AuthExpiration.Value >= DateTime.Now;
            }
        }
    }
}
