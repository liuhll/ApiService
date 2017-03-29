using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jueci.ApiService.Pay.Models
{
    public abstract class WxPaySignOptionsBasic
    {
        /// <summary>
        /// 微信应用Id
        /// </summary>
        public string Appid { get; set; }

       

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
