using Abp.Web.Mvc.Controllers;

namespace Jueci.ApiService.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class ApiServiceControllerBase : AbpController
    {
        protected ApiServiceControllerBase()
        {
            LocalizationSourceName = ApiServiceConsts.LocalizationSourceName;
        }
    }
}