using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jueci.ApiService.Common.Tools
{
    public static class ToolHelper
    {
        public static string GetRandomString(int digitNum)
        {
            Random rd = new Random();
            string str = string.Empty;
            while (str.Length < digitNum)
            {
                int temp = rd.Next(0, 10);
                if (!str.Contains(temp + ""))
                {
                    str += temp;
                }
            }
            return str;
        }

        public static string GetServiceSeedLine()
        {
            System.DateTime startTime = new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            //System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,0,0));
            var seedNum = (int)(DateTime.Now - startTime).TotalSeconds;
            return seedNum.ToString().PadLeft(5, '0');
        }

        public static string GetPrimaryId()
        {
            return string.Format("{0}{1}{2}",
                DateTime.Now.ToString("yyyyMMdd"),
                GetServiceSeedLine(),
                GetRandomString(7));
        }
    }
}
