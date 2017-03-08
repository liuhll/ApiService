using System.Collections.Generic;
using Abp.Domain.Entities;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class BrandInfo : Entity<int>
    {
        public string BrandName { get; set; }

        public State State { get; set; }

        public virtual IList<ServiceInfo> ServiceInfos { get; set; }
    }
}