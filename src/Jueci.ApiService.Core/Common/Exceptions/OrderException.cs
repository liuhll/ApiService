using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp;

namespace Jueci.ApiService.Common.Exceptions
{
    public class OrderException : AbpException
    {
        public OrderException(string msg) : base(msg)
        {
        }
    }
}
