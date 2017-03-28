using Abp.AutoMapper;
using Jueci.ApiService.Pay.Models;

namespace Jueci.ApiService.Pay.Dtos
{
    /// <summary>
    /// 微信支付统一下单返回参数
    /// </summary>
    [AutoMap(typeof(WxPaySignOptions))]
    public class WxPaySignOptionsOutput
    {
        /// <summary>
        /// 微信应用Id
        /// </summary>
        public string Appid { get; set; }

        /// <summary>
        /// 商家合作号Id
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// 预支付交易会话ID
        /// </summary>
        public string PrepayId { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string Noncestr { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        ///  调起支付的签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 支付单号
        /// </summary>
        public string OrderId { get; set; }
    }
}