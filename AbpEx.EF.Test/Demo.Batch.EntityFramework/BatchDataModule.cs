using System.Reflection;
using Abp;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.Modules;
using Demo.Batch.Configuration;

namespace Demo.Batch
{
    [DependsOn(
        typeof(BatchCoreModule),        
        typeof(AbpExEntityFrameworkModule)
        )]
    public class BatchDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            //web.config (or app.config for non-web projects) file should containt a connection string named "Default".
            #if !UnitiTest
                IocManager.RegisterIfNot<IBatchDataModuleConfiguration, BatchDataModuleConfiguration>();
            #endif


        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
