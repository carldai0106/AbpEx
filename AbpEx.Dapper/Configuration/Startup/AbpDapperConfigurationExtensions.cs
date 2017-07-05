using Abp.Dapper.Configuration;

namespace Abp.Configuration.Startup
{
    public static class AbpDapperConfigurationExtensions
    {
        public static IAbpDapperModuleConfiguration AbpDapper(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.
                GetOrCreate("Abp.Dapper", 
                () => configurations.AbpConfiguration.IocManager.Resolve<IAbpDapperModuleConfiguration>());
        }
    }
}
