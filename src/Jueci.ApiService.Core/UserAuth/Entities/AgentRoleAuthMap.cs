using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class AgentRoleAuthMap : Entity<int>
    {
        public int SId { get; set; }

        public int ArId { get; set; }

        public int AuthType { get; set; }

        public DateTime CreateTime { get; set; }

        #region 扩展属性

        public virtual ServiceInfo ServiceInfo { get; set; }

        public virtual AgentRole AgentRole { get; set; }


        #endregion
    }
}
