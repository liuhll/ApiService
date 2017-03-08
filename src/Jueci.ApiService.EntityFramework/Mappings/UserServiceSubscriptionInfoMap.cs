using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.Mappings
{
    public class UserServiceSubscriptionInfoMap : EntityTypeConfiguration<UserServiceSubscriptionInfo>
    {
        public UserServiceSubscriptionInfoMap()
        {
            ToTable("UserServiceSubscriptionInfo");

            HasRequired(t => t.ServiceInfo)
                .WithMany(p => p.UserServiceSubscriptionInfos)
                .HasForeignKey(t => t.SId);

            HasRequired(t => t.UserInfo)
                .WithMany(p => p.UserServiceSubscriptionInfos)
                .HasForeignKey(t => t.UId);

            //HasRequired(t => t.ServicePrice)
            //    .WithMany(t => t.UserServiceSubscriptionInfos)
            //    .HasForeignKey(t => t.SpId);



        }
    }
}
