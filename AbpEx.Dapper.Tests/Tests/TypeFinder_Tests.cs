using Abp.Domain.Entities;
using Abp.Reflection;
using Abp.TestBase;
using Abp.Reflection.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Abp.Dapper.Tests.Tests
{
    public class TypeFinder_Tests : AbpIntegratedTestBase<AbpDapperTestModule>
    {
        public ITestOutputHelper Output { get; private set; }
        
        public TypeFinder_Tests(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Get_All_IEntity_Tests()
        {
            var finder = Resolve<ITypeFinder>();            

            var list = finder.Find(x => x.IsInheritsOrImplements(typeof(IEntity<>)));

            var ss = finder.Find(x => typeof(IEntity<>).IsAssignableFrom(x));

            Output.WriteLine(list.Length.ToString());

            Output.WriteLine(ss.Length.ToString());
        }
    }
}
