using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Jueci.ApiService.Base.Dtos;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth.Dtos
{
    /// <summary>
    /// 用户授权输入
    /// </summary>
    [AutoMap(typeof(SalesInfoModel))]
    public class UserAuthInput : BasicDto
    {
        /// <summary>
        /// 销售价Id
        /// </summary>
        public int PId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int UId { get; set; }

        /// <summary>
        /// 销售途径 取值为 [Admin,Agent,Online,CustomService]
        /// </summary>
        public string SalesWay { get; set; }

        /// <summary>
        /// 销售人员Id
        /// </summary>
        public int? AdminId { get; set; }

        /// <summary>
        /// 代理商Id
        /// </summary>
        public int? AgentId { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 建议价
        /// </summary>
        public decimal? SuggestCost { get; set; }

        /// <summary>
        /// 建议价计算来源单号
        /// </summary>
        public string SubscriptionOrderId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 支付单号
        /// </summary>
        public string OrderId { get; set; }
    }
}
