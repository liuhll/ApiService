using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jueci.ApiService.Common.Exceptions
{
    public class InsufficientFundsException : Exception
    {
        public InsufficientFundsException()
        {

        }

        public InsufficientFundsException(string message)
            : base(message)
        {
        }
    }
}
