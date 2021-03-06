﻿using System;
using Abp.Domain.Entities;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class UserServiceSubscriptionInfo : Entity<string>
    {
        public UserServiceSubscriptionInfo()
        {
            CreateTime = DateTime.Now;
        }

        public int UId { get; set; }

        public int SId { get; set; }

        //public int SpId { get; set; }

        public decimal Cost { get; set; }

        public decimal Profit { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        public DateTime? AuthExpiration { get; set; }

        public string Remarks { get; set; }

        public OrderState State { get; set; }

        public int AuthType { get; set; }

        public int? AgentId { get; set; }

        public int? AdminId { get; set; }

        public int? Puid { get; set; }

        public string OrderId { get; set; }

        public string AuthDesc { get; set; }

        public decimal OriginalCost { get; set; }

        #region 扩展属性

        public virtual UserInfo UserInfo { get; set; }

        public virtual ServiceInfo ServiceInfo { get; set; }

        //public virtual ServicePrice ServicePrice { get; set; }

        //public virtual AgentInfo AgentInfo { get; set; }

        //public virtual AdministratorInfo Administrator { get; set; }

        public bool IsValidityPeriod
        {
            get { return AuthExpiration > DateTime.Now; }

        }

        #endregion
    }
}