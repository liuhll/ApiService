﻿using System.Reflection;
using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;

namespace Jueci.ApiService
{
    [DependsOn(typeof(ApiServiceCoreModule),typeof(AbpAutoMapperModule))]
    public class ApiServiceApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;
            base.PreInitialize();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
