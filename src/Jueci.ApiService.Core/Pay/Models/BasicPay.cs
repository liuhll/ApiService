using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.Pay.Models
{
    public abstract class BasicPay
    {
        public string AppCode { get; set; }

        public string Name { get; set; }

        public string AppId { get; set; }

        public string NotifyUrl { get; set; }

        public abstract PayType PayType { get; protected set; }
    }
}