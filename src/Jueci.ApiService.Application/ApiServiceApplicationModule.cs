using System.Reflection;
using Abp.Modules;

namespace Jueci.ApiService
{
    [DependsOn(typeof(ApiServiceCoreModule))]
    public class ApiServiceApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
