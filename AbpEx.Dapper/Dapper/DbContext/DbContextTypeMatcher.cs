using Abp.Dapper.Common;
using Abp.Domain.Uow;

namespace Abp.Dapper.DbContext
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<DapperDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}
