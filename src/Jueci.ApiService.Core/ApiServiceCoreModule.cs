using System.Reflection;
using Abp.Modules;

namespace Jueci.ApiService
{
    public class ApiServiceCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
