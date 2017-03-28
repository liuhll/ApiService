namespace Jueci.ApiService.Pay.Dtos
{
    /// <summary>
    /// 微信支付统一下单返回参数
    /// </summary>
    public class WxPayOrderInfoOutput
    {
        /// <summary>
        /// 预付单Id
        /// </summary>
        public string PrepayId { get; set; }

        /// <summary>
        /// 支付单号
        /// </summary>
        public string OrderId { get; set; }
    }
}