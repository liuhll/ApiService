using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.Base.Dtos;

namespace Jueci.ApiService.UserAuth.Dtos
{
    /// <summary>
    /// 用户授权输入
    /// </summary>
    public class UserAuthInput : BasicDto
    {
        /// <summary>
        /// 服务Id
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// 销售价Id
        /// </summary>
        public int ServicePriceId { get; set; }

        /// <summary>
        /// 用户通行证
        /// </summary>
        public string UserPassport { get; set; }

        /// <summary>
        /// 销售价
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 销售人员Id
        /// </summary>
        public string SalesManId { get; set; }
    }
}
