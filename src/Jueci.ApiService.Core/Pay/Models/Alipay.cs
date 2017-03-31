using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.Common.Enums;

namespace Jueci.ApiService.Pay.Models
{
    public class Alipay : BasicPay
    {
        public Alipay()
        {
            PayType = PayType.AliPay;
        }

        public override PayType PayType { get; protected set; }

        public string ReturnUrl { get; set; }

        public string AppPrivateKey { get; set; }

        public string AppPublicKey { get; set; }

        public string ServerUrl { get; set; }
        public string PId { get; set; }
    }
}
