using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.Common;
using Jueci.ApiService.Pay.Dtos;

namespace Jueci.ApiService.Pay
{
    public class PayAppService : IPayAppService
    {
        public ResultMessage<PayOrderInfoOutput> NewPayOrder(PayOrderInfoInput input)
        {
            throw new NotImplementedException();
        }
    }
}
