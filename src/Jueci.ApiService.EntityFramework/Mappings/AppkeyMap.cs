using System.Data.Entity.ModelConfiguration;
using Jueci.ApiService.ApiAuthorization.Entities;

namespace Jueci.ApiService.Mappings
{
    public class AppkeyMap : EntityTypeConfiguration<AppKey>
    {
        public AppkeyMap()
        {
            ToTable("Appkey");
        }
    }
}