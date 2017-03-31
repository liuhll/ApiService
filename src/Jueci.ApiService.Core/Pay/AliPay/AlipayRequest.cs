using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Logging;
using Aop.Api.Request;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Common.Tools;
using Jueci.ApiService.Pay.Lib;
using Jueci.ApiService.Pay.Models;
using Aop.Api;

namespace Jueci.ApiService.Pay.AliPay
{
    public class AlipayRequest : IAlipayRequest
    {
        public bool Wappay(AlipayOrderOptions options, Alipay alipayConfig, PayMode payMode, out object respData)
        {
            IAopClient aopClient = new DefaultAopClient(alipayConfig.ReturnUrl,
                alipayConfig.AppId,
                alipayConfig.AppPrivateKey,
                AliPayConfig.FORMAT,
                AliPayConfig.CHARSET,
                AliPayConfig.SIGN_TYPE,
                alipayConfig.AppPublicKey);

            switch (payMode)
            {
                case PayMode.Mobile:
                    //实例化具体API对应的request类,类名称和接口名称对应,当前调用接口名称如：alipay.open.public.template.message.industry.modify 
                    AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
                    //SDK已经封装掉了公共参数，这里只需要传入业务参数
                    request.SetNotifyUrl(alipayConfig.NotifyUrl);
                    request.SetReturnUrl(alipayConfig.ReturnUrl);

                    request.BizContent = options.ToJson();
                    string form = aopClient.pageExecute(request).Body;

                    respData = form;
                    break;
                case PayMode.App:
                    AlipayTradeAppPayRequest appRequest = new AlipayTradeAppPayRequest();
                    appRequest.BizContent = options.ToJson();
                    appRequest.SetReturnUrl(alipayConfig.ReturnUrl);
                    appRequest.BizContent = options.ToJson();
                    appRequest.SetBizModel(options);

                    var signData = aopClient.SdkExecute(appRequest);
                    //signData.OutTradeNo = options.out_trade_no;
                    //signData.SellerId = options.seller_id;
                    //signData.TotalAmount = options.total_amount;
                    respData = signData.Body;
                    break;
                default:
                    throw new Exception("暂不支持该类型的支付方式");
            }


            return true;
        }

        public AlipayData Query(string id, OrderType outTradeNo, Alipay alipayConfig)
        {
            IAopClient aopClient = new DefaultAopClient(alipayConfig.ReturnUrl,
                alipayConfig.AppId,
                alipayConfig.AppPrivateKey,
                AliPayConfig.FORMAT,
                AliPayConfig.CHARSET,
                AliPayConfig.SIGN_TYPE,
                alipayConfig.AppPublicKey);
            AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
            var requstData = "{\"" + (outTradeNo == OrderType.OutTradeNo ? "out_trade_no" : "trade_no") + "\":\"" + id +
                             "\"}";

            //string.Format(, outTradeNo == OrderType.OutTradeNo ? "out_trade_no" : "trade_no",id);
            request.BizContent = requstData;
            var response = aopClient.Execute(request);
            var alipayData = new AlipayData();
            if (response.IsError)
            {
                LogHelper.Logger.Error("查询订单失败：" + response.Msg + "," + response.SubMsg);
                if (response.Code == "40004")
                {
                    alipayData.SetValue("trade_status", "NOPAY");
                    alipayData.SetValue(outTradeNo == OrderType.OutTradeNo ? "out_trade_no" : "trade_no", id);
                    return alipayData;
                }

                throw new Exception("查询订单失败，原因" + response.Msg);
            }
            alipayData.SetValue("trade_status", response.TradeStatus);
            alipayData.SetValue("trade_no", response.TradeStatus);
            alipayData.SetValue("out_trade_no", response.OutTradeNo);
            alipayData.SetValue("buyer_logon_id", response.BuyerLogonId);
            alipayData.SetValue("total_amount", response.TotalAmount);
            alipayData.SetValue("receipt_amount", response.ReceiptAmount);
            alipayData.SetValue("buyer_pay_amount", response.BuyerPayAmount);
            return alipayData;

        }
    }
}
