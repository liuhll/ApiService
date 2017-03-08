using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Abp.Logging;
using Jueci.ApiService.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Jueci.ApiService.ApiAuthorization.Policy;

namespace Jueci.ApiService.ApiAuthorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class JeuciAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
#if DEBUG
                return true;
#else
                string appid;
                string sign;
                long timestamp;
                var paramList = GetRequestParams(actionContext,out appid, out sign, out timestamp);
                IApiAuthorizePolicy authorizePolicy = new ApiAuthorizePolicy(paramList, timestamp, sign);
                return authorizePolicy.IsValidTime() && authorizePolicy.IsLegalSign();
#endif
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("授权验证失败,原因" + ex.Message);
                return false;
            }

        }

        private IDictionary<string, string> GetRequestParams(HttpActionContext actionContext,out string appid, out string sign, out long timestamp)
        {
            var requestParams = new Dictionary<string, string>();
            var requestAppId = string.Empty;
            var requestSign = string.Empty;            
            long requesTimestamp = 0;
            if (actionContext.Request.Method == HttpMethod.Get)
            {
                var qString = actionContext.Request.RequestUri.ParseQueryString();
                //Array.Sort(qString.AllKeys);
                foreach (var q in qString.AllKeys)
                {               
                    if (q.ToLower().Contains("appid"))
                    {
                        requestAppId = qString[q];
                    }
                    if (q.ToLower().Contains("sign"))
                    {
                        requestSign = qString[q];
                    }
                    if (q.ToLower().Contains("timestamp"))
                    {
                        requesTimestamp = Convert.ToInt64(qString[q]);
                    }
                    requestParams.Add(q.Split('.')[1].ToLower(), qString[q]);
                }
            }
            else
            {
                //actionContext.Request.Content
                Task<string> content = actionContext.Request.Content.ReadAsStringAsync();
                string body = content.Result;
                var data = (JObject)JsonConvert.DeserializeObject(body);
                foreach (var item in data)
                {
                    if (item.Key.Equals("appid", StringComparison.OrdinalIgnoreCase))
                    {
                        requestAppId = item.Value.ToString();
                    }

                    if (item.Key.Equals("sign", StringComparison.OrdinalIgnoreCase))
                    {
                        requestSign = item.Value.ToString();
                    }
                    if (item.Key.Equals("timestamp", StringComparison.OrdinalIgnoreCase))
                    {
                        requesTimestamp = Convert.ToInt64(item.Value.ToString());
                    }

                    requestParams.Add(item.Key.ToLower(), item.Value.ToString());
                }
            }
            if (string.IsNullOrEmpty(requestAppId))
            {
                throw new ApiAuthorizeException("请求的appkey不能为空");
            }

            if (string.IsNullOrEmpty(requestSign))
            {               
                throw new ApiAuthorizeException("请求的签名不能为空");
            }
            if (requesTimestamp == 0)
            {
                throw new ApiAuthorizeException("请求的时间戳不能为空");
            }

            sign = requestSign;
            timestamp = requesTimestamp;
            appid = requestAppId;
            return requestParams;
        }

    }
}