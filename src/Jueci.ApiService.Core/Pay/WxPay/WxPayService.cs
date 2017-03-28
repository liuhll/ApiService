using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Logging;
using Jueci.ApiService.Common.Exceptions;
using Jueci.ApiService.Pay.Entities;
using Jueci.ApiService.Pay.Lib;
using Jueci.ApiService.Pay.Models;

namespace Jueci.ApiService.Pay.WxPay
{
    public class WxPayService : IWxPayService
    {
        public WxPayData UnifiedOrder(ServiceOrder serviceOrder,Models.WxPay wxPayConfig, int timeOut = 10)
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";

            //1. 检测必填参数
            CheckParams(serviceOrder);

            WxPayData wxPayData = new WxPayData(wxPayConfig);

            // 2.通过反射设置参数值
            var dataProperties = serviceOrder.GetType().GetProperties();

            foreach (var property in dataProperties)
            {
                if (property.GetValue(serviceOrder, null) != null)
                {
                    wxPayData.SetValue(property.Name, property.GetValue(serviceOrder, null));
                }

            }

            // 3.签名
            wxPayData.SetValue("sign", wxPayData.MakeSign());

            //4.获取上传的xml字符串参数
            string xml = wxPayData.ToXml();

            try
            {
                LogHelper.Logger.Debug("UnfiedOrder request : " + xml);
                string response = HttpService.Post(xml, url, false, timeOut);
                LogHelper.Logger.Debug("UnfiedOrder response : " + response);

                WxPayData result = new WxPayData(wxPayConfig);
                result.FromXml(response);

                if (result.GetValue("return_code").ToString() == "FAIL")
                {
                    LogHelper.Logger.Error("调用统一下单接口失败,原因:" + result.GetValue("return_msg"));
                    throw new OrderException("下单失败，请稍后重试!" + result.GetValue("return_msg"));
                }

              //  result.SetValue("orderid", wxPayData.GetValue("out_trade_no"));

                return result;
            }
            catch (Exception e)
            {
                LogHelper.Logger.Error("调用统一下单接口失败,原因:" + e.Message);
                throw new OrderException("由于网络原因下单失败,请重试");
            }


        }

        private void CheckParams(ServiceOrder serviceOrder)
        {
            //检测必填参数
            if (string.IsNullOrEmpty(serviceOrder.out_trade_no))
            {
                throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (string.IsNullOrEmpty(serviceOrder.body))
            {
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            else if (serviceOrder.total_fee <= 0)
            {
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (string.IsNullOrEmpty(serviceOrder.trade_type))
            {
                throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (serviceOrder.trade_type == "JSAPI" && string.IsNullOrEmpty(serviceOrder.openid))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (serviceOrder.trade_type == "NATIVE" && string.IsNullOrEmpty(serviceOrder.product_id))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }
        }
    }
}
