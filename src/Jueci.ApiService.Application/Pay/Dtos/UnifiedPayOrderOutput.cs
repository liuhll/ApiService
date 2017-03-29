namespace Jueci.ApiService.Pay.Dtos
{
    public class UnifiedPayOrderOutput
    {
        /// <summary>
        /// 统一下单数据，用于调起支付，获取作为调起支付的参数
        /// </summary>
        public object OrderData { get; set; }

        /// <summary>
        /// 订单号(公司内部单号)
        /// </summary>
        public string OrderId { get; set; }

    }
}