using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dapper.Configuration;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Dapper;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using Xunit.Abstractions;
using Abp.Dapper.DbContext;

namespace Abp.Dapper.Tests
{
    public class AbpDapperTestBase : AbpIntegratedTestBase<AbpDapperTestModule>
    {
        public ITestOutputHelper Output { get; private set; }

        public AbpDapperTestBase(ITestOutputHelper output)
        {
            Output = output;

            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            AbpSession.UserId = 1;
            AbpSession.TenantId = 1;
        }        

        protected override void PreInitialize()
        {
            //base.PreInitialize();
            if (!LocalIocManager.IsRegistered<IDapperConfiguration>())
            {
                LocalIocManager.Register<IDapperConfiguration, DapperConfiguration>();
            }

            var configuration = LocalIocManager.Resolve<IDapperConfiguration>();
            var connection = new SqlConnection(@"Data Source=(localdb)\ProjectsV13;Initial Catalog=DapperTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            var context = configuration
                                        .UseClassMapper(typeof(AutoClassMapper<>))
                                        .FromAssembly(new List<Assembly> { Assembly.GetExecutingAssembly() })
                                        .UseSqlDialect(new SqlServerDialect())
                                        .UseConnection(connection)
                                        .Build();

            var files = new List<string>
                                 {
                                    ReadScriptFile("CreatePersonTable")
                                 };

            foreach (var setupFile in files)
            {
                connection.Execute(setupFile);
            }
        }

        public string ReadScriptFile(string name)
        {
            string fileName = GetType().Namespace + ".Sql." + name + ".sql";
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            {
                if (resource != null)
                {
                    using (var sr = new StreamReader(resource))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            return string.Empty;
        }
    }
}
