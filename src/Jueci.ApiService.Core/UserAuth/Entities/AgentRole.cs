using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class AgentRole : Entity
    {
        public string RoleName { get; set; }

        public decimal Discount { get; set; }

        public string Remark { get; set; }

        #region 扩展属性

        public virtual ICollection<AgentInfo> AgentInfos { get; set; }

        public virtual ICollection<AgentRoleAuthMap> AgentRoleAuthMaps { get; set; }

        #endregion
    }
}
