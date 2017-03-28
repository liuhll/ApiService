using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.Common.Tools;
using Jueci.ApiService.Pay.Lib;

namespace Jueci.ApiService.Pay.Models
{
    public class ServiceOrder
    {
        public string appid { get; set; }

        public string mch_id { get; set; }

        public string device_info
        {
            get { return "WEB"; }
        }

        public string nonce_str
        {
            get { return NonceHelper.GenerateNonceStr(); }
        }

        public string sign_type
        {
            get
            {
                return "MD5";
            }
        }

        public string body { get; set; }

        public string detail { get; set; }

        public string attach { get; set; }

        public string out_trade_no { get; set; }

        public int total_fee { get; set; }

        public string spbill_create_ip
        {
            get { return IpAddressHelper.GetLocalIpAddress(); }
        }

        public string time_start
        {
            get { return DateTime.Now.ToString("yyyyMMddHHmmss"); }
        }

        //public string time_expire
        //{
        //    get { return WxPayConfig.TIME_EXPIRE; }
        //}

        public string notify_url {
            get; set;
        }

        public string trade_type { get; set; }

        public string openid { get; set; }

        public string product_id { get; set; }
    }
}
