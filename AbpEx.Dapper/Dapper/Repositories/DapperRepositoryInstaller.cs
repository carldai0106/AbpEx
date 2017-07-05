using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Abp.Dapper.Repositories
{
    internal class DapperRepositoryInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For(typeof(IDapperRepository<>)).ImplementedBy(typeof(DapperRepositoryBase<>)).LifestyleTransient(),
                Component.For(typeof(IDapperRepository<,>)).ImplementedBy(typeof(DapperRepositoryBase<,>)).LifestyleTransient()
                );
        }
    }
}
