using Abp.AutoMapper;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.UserAuth.ViewModel;

namespace Jueci.ApiService.UserAuth.Dtos
{
    /// <summary>
    /// 用户授权价格信息
    /// </summary>    
    [AutoMap(typeof(UserServicePrice))]
    public class UserServicePriceOutput
    {
        /// <summary>
        /// 服务Id
        /// </summary>
        public int Sid { get; set; }

        /// <summary>
        /// 产品价格Id
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 原始价格
        /// </summary>
        public decimal OriginalCost { get; set; }

        /// <summary>
        /// 代理商销售价格
        /// </summary>
        public decimal? AgentCost { get; set; }

        /// <summary>
        /// 授权描述
        /// </summary>
        public string AuthDesc { get; set; }

        /// <summary>
        /// 授权类型
        /// </summary>
        public int AuthType { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 购买类型
        /// </summary>
        public PurchaseType PurchaseType { get; set; }

        /// <summary>
        /// 升级价格计划订单Id
        /// </summary>
        public string SubscriptionOrderId { get; set; }
    }
}