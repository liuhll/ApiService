using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.UserAuth.Entities;

namespace Jueci.ApiService.Mappings
{
    public class AgentRoleMap : EntityTypeConfiguration<AgentRole>
    {
        public AgentRoleMap()
        {
            ToTable("AgentRoleInfo");

        }
    }
}
