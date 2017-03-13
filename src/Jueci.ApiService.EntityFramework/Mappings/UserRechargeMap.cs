using System.Data.Entity.ModelConfiguration;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.Mappings
{
    public class UserRechargeMap : EntityTypeConfiguration<UserRecharge>
    {
        public UserRechargeMap()
        {
            ToTable("UserRecharge");
            HasRequired(t => t.UserInfo)
                .WithMany(t => t.UserRecharges)
                .HasForeignKey(t => t.UId);
        }
    }
}