using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.Pay.Models
{
    public class WxPay : BasicPay
    {
        public WxPay()
        {
            PayType = PayType.Wechat;
        }

        public string MchId { get; set; }

        public string Key { get; set; }

        public string AppSecret { get; set; }

        public override PayType PayType { get; protected set; }
    }
}