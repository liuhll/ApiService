using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class ServicePrice : Entity<int>
    {
        public int ServiceId { get; set; }

        public virtual ServiceInfo ServerInfo { get; set; }

        public decimal Price { get; set; }

        public decimal AgentPrice { get; set; }

        public int? DateYear { get; set; }

        public int DateMonth { get; set; }

        public int DateDay { get; set; }

        public string AuthDesc { get; set; }

        public int AuthType { get; set; }

        public string Description { get; set; }

        public State State { get; set; }

        public decimal Discount { get; set; }

        public bool IsLifeLongVersion
        {
            get
            {
                //为空的时候是终身版本
                return !DateYear.HasValue;
            }
        }

        #region 扩展属性

        //public virtual ICollection<UserServiceSubscriptionInfo> UserServiceSubscriptionInfos { get; set; }

        #endregion


    }
}
