using Abp.Web.Mvc.Views;

namespace Jueci.ApiService.Web.Views
{
    public abstract class ApiServiceWebViewPageBase : ApiServiceWebViewPageBase<dynamic>
    {

    }

    public abstract class ApiServiceWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected ApiServiceWebViewPageBase()
        {
            LocalizationSourceName = ApiServiceConsts.LocalizationSourceName;
        }
    }
}