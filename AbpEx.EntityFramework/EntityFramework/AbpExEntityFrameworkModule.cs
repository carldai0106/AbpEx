using System.Reflection;
using Abp.Dependency;
using Abp.EntityFramework.Batch;
using Abp.Modules;

namespace Abp.EntityFramework
{    
    [DependsOn(
        typeof(AbpKernelModule),
        typeof(AbpEntityFrameworkModule)
        )]
    public class AbpExEntityFrameworkModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IAbpBatchRunner, AbpSqlServerBatchRunner>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
