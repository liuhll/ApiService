using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Events.Bus;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class AgentInfo : Entity
    {
        public AgentInfo()
        {
            CreateTime = DateTime.Now;
            State = State.Active;

        }

        public DateTime CreateTime { get; set; }

        public State State { get; set; }

        public int ArId { get; set; }

        public string Website { get; set; }

        public int CreateAdminId { get; set; }

        public string FrozenRemarks { get; set; }

        public DateTime? UpdateTime { get; set; }

        public int? UpdateAdminId { get; set; }

        public bool AgentIsActive
        {
            get { return State == State.Active; }
        }

        #region 扩展属性

        //public virtual ICollection<UserServiceSubscriptionInfo> UserServiceSubscriptionInfos { get; set; }

        public virtual AgentRole AgentRole { get; set; }


        #endregion
    }
}
