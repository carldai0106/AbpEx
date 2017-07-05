namespace Abp.Dapper.Configuration
{
    public class AbpDapperModuleConfiguration : IAbpDapperModuleConfiguration
    {
        public IDapperConfiguration DapperConfiguration { get; set; }

        public AbpDapperModuleConfiguration(IDapperConfiguration dapperConfiguration)
        {
            DapperConfiguration = dapperConfiguration;
        }
    }
}
