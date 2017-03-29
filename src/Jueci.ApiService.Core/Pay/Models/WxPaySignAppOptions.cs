using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jueci.ApiService.Pay.Models
{
    public class WxPaySignAppOptions : WxPaySignOptionsBasic
    {
        /// <summary>
        /// 商家合作号Id
        /// </summary>
        public string PartnerId { get; set; }
    }
}
