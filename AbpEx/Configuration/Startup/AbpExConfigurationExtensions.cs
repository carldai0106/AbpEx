namespace Abp.Configuration.Startup
{
    public static class AbpExConfigurationExtensions
    {
        public static IAbpExConfiguration AbpEx(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.AbpEx", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpExConfiguration>());
        }
    }
}
