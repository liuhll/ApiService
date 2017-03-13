using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jueci.ApiService.UserAuth.ViewModel
{
    public class SalesInfoModel
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
        /// 销售价格
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 产品价值
        /// </summary>
        public decimal? SuggestCost { get; set; }

        /// <summary>
        /// 产品价值计算来源单号
        /// </summary>
        public string SubscriptionOrderId { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 销售人员Id
        /// </summary>
        public int? AdminId { get; set; }

        public int? AgentId { get; set; }

        /// <summary>
        /// 支付单号
        /// </summary>
        public string OrderId { get; set; }
    }
}
