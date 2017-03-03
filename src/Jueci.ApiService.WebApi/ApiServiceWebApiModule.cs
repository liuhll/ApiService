using System.Reflection;
using Abp.Application.Services;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.WebApi;

namespace Jueci.ApiService
{
    [DependsOn(typeof(AbpWebApiModule), typeof(ApiServiceApplicationModule))]
    public class ApiServiceWebApiModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            Configuration.Modules.AbpWebApi().DynamicApiControllerBuilder
                .ForAll<IApplicationService>(typeof(ApiServiceApplicationModule).Assembly, "app")
                .Build();
        }
    }
}
