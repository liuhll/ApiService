using System.Collections.Generic;
using Abp.Domain.Entities;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class ServiceInfo : Entity
    {
        public string ServiceName { get; set; }

        public int BrandId { get; set; }

        public virtual BrandInfo BrandInfo { get; set; }

        public int State { get; set; }

        public string Remarks { get; set; }

        public int Sort { get; set; }

        //   private IList<ServicePrice> _servicePrices;

        #region 扩展属性
        public virtual IList<ServicePrice> ServicePrices
        {
            get; set;
        }

        public virtual ICollection<UserServiceSubscriptionInfo> UserServiceSubscriptionInfos { get; set; }

        public virtual ICollection<AgentRoleAuthMap> AgentRoleAuthMaps { get; set; }


        #endregion
    }
}