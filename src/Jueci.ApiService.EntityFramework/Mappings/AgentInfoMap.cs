using System.Data.Entity.ModelConfiguration;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.Mappings
{
    public class AgentInfoMap : EntityTypeConfiguration<AgentInfo>
    {
        public AgentInfoMap()
        {
            ToTable("AgentInfo");

            HasKey(t => t.Id);

            Property(t => t.Id).HasColumnName("AgentID");

            //HasRequired(t => t.UserInfo)
            //    .WithOptional(t => t.AgentInfo)
            //    .Map(t => t.MapKey("AgentId"));

            HasRequired(t => t.AgentRole)
                .WithMany(t => t.AgentInfos)
                .HasForeignKey(t => t.ArId);
        }
    }
}