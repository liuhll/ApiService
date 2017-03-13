using System;
using System.Collections.Generic;
using Abp.Events.Bus;

namespace Jueci.ApiService.UserAuth.Entities
{
    public class UserInfo : UserBase
    {
        private bool _isAgentor = false;
        //  private AgentInfo _agentInfo;


        public string PayPassword { get; set; }

        public string QQ { get; set; }

        public string NickName { get; set; }

        public bool? Sex { get; set; }

        public string SafeMobile { get; set; }

        public string SafeEmail { get; set; }

        public DateTime RegTime { get; set; }

        public int? AppType { get; set; }

        public string IP { get; set; }

        /// <summary>
        /// 从哪个服务注册的
        /// </summary>
        public int? SID { get; set; }

        public decimal Fund { get; set; }

        public string FrozenRemarks { get; set; }

        public string AppCode { get; set; }

        public virtual ICollection<UserServiceSubscriptionInfo> UserServiceSubscriptionInfos { get; set; }

        public virtual ICollection<UserRecharge> UserRecharges { get; set; }

        /// <summary>
        ///如果是代理商，则获取代理商信息
        /// </summary>
        //public AgentInfo AgentInfo
        //{
        //    //get
        //    //{
        //    //    if (!_isAgentor)
        //    //    {
        //    //        throw new SalesSysException(string.Format("Id为：{0}的用户不是代理商,无法获取相关信息", Id));
        //    //    }
        //    //    return _agentInfo;
        //    //}
        //    get; private set;
        //}


    }
}