using Abp.AutoMapper;
using Jueci.ApiService.Base.Dtos;

namespace Jueci.ApiService.Pay.Dtos
{
    [AutoMap(typeof(PayOrderInfoInput))]
    public class WxPayOrderInput : BasicDto
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
        /// 支付模式
        /// </summary>
        public string PayModeStr { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// AppCode 
        /// </summary>
        public string AppCode { get; set; }
    }
}