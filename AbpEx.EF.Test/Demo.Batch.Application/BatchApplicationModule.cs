using System.Reflection;
using Abp;
using Abp.AutoMapper;
using Abp.Modules;
using Demo.Batch.Configuration.Startup;

namespace Demo.Batch.Application
{
    [DependsOn(
        typeof(BatchCoreModule),
        typeof(BatchDataModule),
        typeof(AbpAutoMapperModule))]
    public class BatchApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            //Configuration.Authorization.Providers.Add<VhasAuthorizationProvider>();
#if !UnitTest

            Configuration.Modules.BatchDataModule().NameOrConnectionString = "BatchDemo";
#endif
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
