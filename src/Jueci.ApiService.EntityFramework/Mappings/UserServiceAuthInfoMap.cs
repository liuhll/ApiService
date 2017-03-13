using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.Mappings
{
    public class UserServiceAuthInfoMap : EntityTypeConfiguration<UserServiceAuthInfo>
    {
        public UserServiceAuthInfoMap()
        {
            ToTable("UserServiceAuthInfo");
            HasKey(t => new { t.UId, t.SId });

            Ignore(t => t.Id);
            Ignore(t => t.IsActive);
        }
    }
}
