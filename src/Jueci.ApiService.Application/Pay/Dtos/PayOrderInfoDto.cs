using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Jueci.ApiService.Base.Dtos;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Pay.Entities;

namespace Jueci.ApiService.Pay.Dtos
{
    /// <summary>
    /// 创建支付订单输入参数
    /// </summary>
    [AutoMap(typeof(UserPayOrderInfo))]
    public class PayOrderInfoDto 
    {
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

        ///// <summary>
        ///// AppCode 
        ///// </summary>
        //public string AppCode { get; set; }
    }
}