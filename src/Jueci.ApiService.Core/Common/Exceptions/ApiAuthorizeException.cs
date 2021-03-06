﻿using System;
using Abp;

namespace Jueci.ApiService.Common.Exceptions
{
    public class ApiAuthorizeException : AbpException
    {
        public ApiAuthorizeException(string msg) : base(msg)
        {
        }

        public ApiAuthorizeException(string message, Exception innerException)
         : base(message, innerException)
        {
        }
    }
}