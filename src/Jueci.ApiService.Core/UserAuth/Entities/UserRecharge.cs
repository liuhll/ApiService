using System;
using Abp.Domain.Entities;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class UserRecharge : Entity<string>
    {
        public UserRecharge()
        {
            CreateTime = DateTime.Now;
        }

        public int UId { get; set; }

        public int? AdminId { get; set; }

        public decimal Cost { get; set; }

        public DateTime CreateTime { get; set; }

        public string OrderId { get; set; }

        public string Remarks { get; set; }

        #region 扩展属性
        public virtual UserInfo UserInfo { get; set; }

        #endregion
    }
}