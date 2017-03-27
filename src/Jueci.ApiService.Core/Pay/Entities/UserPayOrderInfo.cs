using System;
using Abp.Domain.Entities;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.Pay.Entities
{
    public class UserPayOrderInfo : Entity<string>
    {
        public UserPayOrderInfo()
        {
            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 用户Id
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 0为用户充值，其他数值则对应响应的服务Id
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 商品Id
        /// </summary>
        public int? GoodsId { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// 支付模式
        /// </summary>
        public PayMode PayMode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        public string GoodsName { get; set; }

        public string PayAppId { get; set; }

        public string PayOrderId { get; set; }

        public string PayExtendInfo { get; set; }

        public string PayState { get; set; }

        public int State { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }


    }
}