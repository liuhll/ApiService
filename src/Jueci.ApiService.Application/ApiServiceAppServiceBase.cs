using Abp.Application.Services;

namespace Jueci.ApiService
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ApiServiceAppServiceBase : ApplicationService
    {
        protected ApiServiceAppServiceBase()
        {
            LocalizationSourceName = ApiServiceConsts.LocalizationSourceName;
        }
    }
}