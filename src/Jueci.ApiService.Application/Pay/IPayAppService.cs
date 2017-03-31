using Abp.Application.Services;
using Jueci.ApiService.Common;
using Jueci.ApiService.Pay.Dtos;

namespace Jueci.ApiService.Pay
{
    public interface IPayAppService : IApplicationService
    {
        /// <summary>
        /// 创建一个新的支付订单
        /// </summary>
        /// <param name="input">支付订单参数</param>
        /// <returns>返回支付订单单号等数据</returns>
        /// <remarks>创建一个新的支付订单，并返回支付订单单号</remarks>
        //  ResultMessage<PayOrderInfoOutput> NewPayOrder(PayOrderInfoInput input);

        ///<summary>
        /// 统一下单接口
        /// </summary>
        /// <param name="input">支付订单参数</param>
        /// <remarks>创建一个支付订单，并向支付系统后台下单，<br/>并返回调起支付API参数(仅限微信公众号支付和APP支付)</remarks>
        ResultMessage<UnifiedPayOrderOutput> UnifiedPayOrder(PayOrderInput input);



    }
}