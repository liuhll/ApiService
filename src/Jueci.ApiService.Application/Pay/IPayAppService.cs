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
        /// 微信支付统一下单接口
        /// </summary>
        /// <param name="input">支付订单参数</param>
        /// <remarks>创建一个支付订单，并调用微信统一下单API接口，并返回预付单信息</remarks>
        ResultMessage<WxPaySignOptionsOutput> WxPayUnifiedOrder(WxPayOrderInput input);


    }
}