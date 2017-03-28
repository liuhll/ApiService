using System;

namespace Jueci.ApiService.Pay.Lib
{
    class WxPayException : Exception
    {
        public WxPayException(string msg) : base(msg)
        {

        }
    }
}
