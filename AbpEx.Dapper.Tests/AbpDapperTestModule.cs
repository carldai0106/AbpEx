using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.TestBase;

namespace Abp.Dapper.Tests
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(AbpDapperModule), 
        typeof(AbpTestBaseModule))]
    public class AbpDapperTestModule : AbpModule
    {
        public override void Initialize()
        {
            Configuration.DefaultNameOrConnectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=DapperTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }
    }
}
