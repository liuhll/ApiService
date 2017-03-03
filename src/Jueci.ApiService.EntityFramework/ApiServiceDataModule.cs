using System.Data.Entity;
using System.Reflection;
using Abp.EntityFramework;
using Abp.Modules;
using Jueci.ApiService.EntityFramework;

namespace Jueci.ApiService
{
    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(ApiServiceCoreModule))]
    public class ApiServiceDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Database.SetInitializer<ApiServiceDbContext>(null);
        }
    }
}
