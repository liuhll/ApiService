using System.Data.Entity.ModelConfiguration;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.Mappings
{
    public class ServiceInfoMap : EntityTypeConfiguration<ServiceInfo>
    {
        public ServiceInfoMap()
        {
            ToTable("ServiceInfo");
            //HasRequired(t => t.BrandInfo)
            //    .WithMany(t => t.ServiceInfos)
            //    .HasForeignKey(t => t.BrandId);

        }
    }
}