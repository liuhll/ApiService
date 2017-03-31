using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aop.Api;

namespace Jueci.ApiService.Pay.Models
{
    public class AlipayOrderOptions : AopObject
    {
        public string out_trade_no { get; set; }

        public string subject { get; set; }

        public string seller_id { get; set; }

        public string total_amount { get; set; }
    }
}
