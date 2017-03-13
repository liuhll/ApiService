using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jueci.ApiService.Common.Exceptions
{
    public class AgentNoActiveException : Exception
    {
        public AgentNoActiveException()
        {
        }

        public AgentNoActiveException(string message) : base(message)
        {

        }
    }
}
