using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Abp.Extensions;
using Abp.Logging;
using Jueci.ApiService.Common.Enums;
using Jueci.ApiService.Pay.Models;

namespace Jueci.ApiService.Pay
{
    public class PayConfigLoader : IPayConfigLoader
    {
        private IList<BasicPay> payAppConfigs;

        public PayConfigLoader()
        {
            payAppConfigs = new List<BasicPay>();
            LoadPayApps();
        }

        private void LoadPayApps()
        {

            var xmlDoc = new XmlDocument();
            var configFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PayConfig.xml");
            xmlDoc.Load(configFileName);
            XmlNode wxpayNode = xmlDoc.SelectSingleNode("config/wxpay");
            Debug.Assert(wxpayNode != null, "wxpayNode != null");
            foreach (XmlNode node in wxpayNode.ChildNodes)
            {
                var wxpay = new Models.WxPay()
                {
                    AppId = node.SelectSingleNode("./appId")?.InnerText,
                    AppCode = node.Attributes["AppCode"]?.InnerText,
                    AppSecret = node.SelectSingleNode("./appSecret")?.InnerText,
                    Key = node.SelectSingleNode("./key")?.InnerText,
                    MchId = node.SelectSingleNode("./mchId")?.InnerText,
                    Name = node.Attributes["Name"]?.InnerText,
                    NotifyUrl = node.SelectSingleNode("./notifyUrl")?.InnerText,

                };

                payAppConfigs.Add(wxpay);
            }

            XmlNode aliPayNode = xmlDoc.SelectSingleNode("config/alipay");
            foreach (XmlNode node in aliPayNode.ChildNodes)
            {
                var alipay = new Alipay()
                {
                    Name = node.Attributes["Name"]?.InnerText,
                    NotifyUrl = node.SelectSingleNode("./notifyUrl")?.InnerText,
                    AppId = node.SelectSingleNode("./appId")?.InnerText,
                    AppCode = node.Attributes["AppCode"]?.InnerText,
                    AppPublicKey = node.SelectSingleNode("./PublicKey")?.InnerText.Replace("\r", "").Replace("\n", "").Replace(" ", ""),
                    AppPrivateKey = node.SelectSingleNode("./PrivateKey")?.InnerText.Replace("\r", "").Replace("\n", "").Replace(" ", ""),
                    ReturnUrl = node.SelectSingleNode("./returnUrl")?.InnerText,
                    ServerUrl = node.SelectSingleNode("./serverUrl")?.InnerText,
                    PId = node.SelectSingleNode("./pid")?.InnerText,
                };
                payAppConfigs.Add(alipay);
            }

        }


        public IList<BasicPay> GetPayApps()
        {
            return payAppConfigs;
        }

        public T GetPayConfigInfo<T>(PayType payType, string appCode) where T : BasicPay
        {
            var payconfig = GetPayConfigInfo(payType, appCode);
            return (T)payconfig;
        }

        public T GetPayConfigInfoByAppid<T>(PayType payType, string appid) where T : BasicPay
        {
            var payconfig = payAppConfigs.FirstOrDefault(p => p.PayType == payType && p.AppId.Equals(appid, StringComparison.OrdinalIgnoreCase));
            if (payconfig == null)
            {
                LogHelper.Logger.Error(string.Format("不存在指定的支付配置信息;PayType:{0},Appcode:{1}", payType, appid));
                throw new Exception(string.Format("不存在指定的支付配置信息;PayType:{0},Appcode:{1}", payType, appid));
            }
            return (T)payconfig;
        }

        public BasicPay GetPayConfigInfo(PayType payType, string appCode)
        {
            if (payType == PayType.AliPay && string.IsNullOrEmpty(appCode))
            {
                appCode = "Default";
            }

            var payconfig = payAppConfigs.FirstOrDefault(p => p.PayType == payType && p.AppCode.Equals(appCode, StringComparison.OrdinalIgnoreCase));
            if (payconfig == null)
            {
                var defaultPayConfig = payAppConfigs.FirstOrDefault(p => p.PayType == payType && p.AppCode.Equals("Default", StringComparison.OrdinalIgnoreCase));
                if (defaultPayConfig != null)
                {
                    return defaultPayConfig;
                }
                LogHelper.Logger.Error(string.Format("不存在指定的支付配置信息;PayType:{0},Appcode:{1}", payType, appCode));
                throw new Exception(string.Format("不存在指定的支付配置信息;PayType:{0},Appcode:{1}", payType, appCode));
            }
            return payconfig;
        }
    }
}
