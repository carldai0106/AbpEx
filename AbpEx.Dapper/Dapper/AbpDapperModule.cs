using System.Reflection;
using Castle.MicroKernel.Registration;
using Abp.Dapper.Configuration;
using Abp.Dapper.Repositories;
using Abp.Modules;
using Abp.Dependency;
using Abp.Configuration.Startup;
using Abp.Dapper.DbContext;
using Abp.Dapper.Uow;

namespace Abp.Dapper
{
    [DependsOn(
         typeof(AbpKernelModule)
      )]
    public class AbpDapperModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IDapperConfiguration, DapperConfiguration>();
            IocManager.RegisterIfNot<IAbpDapperModuleConfiguration, AbpDapperModuleConfiguration>();

            Configuration.ReplaceService<IDapperTransactionStrategy, DapperTransactionStrategy>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.IocContainer.Install(new DapperRepositoryInstaller());

            IocManager.IocContainer.Register(
               Component.For(typeof(IDapperDbContextProvider))
                   .ImplementedBy(typeof(UnitOfWorkDapperContextProvider))
                   .LifestyleTransient()
               );

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
