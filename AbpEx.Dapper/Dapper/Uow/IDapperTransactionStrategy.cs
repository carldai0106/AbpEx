using Abp.Dapper.DbContext;
using Abp.Dependency;
using Abp.Domain.Uow;
using DapperExtensions.Sql;
using System.Data;

namespace Abp.Dapper.Uow
{
    public interface IDapperTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        DapperDbContext CreateDbContext(IDbConnection connection, ISqlGenerator sqlGenerator, IDbContextResolver dbContextResolver);

        void Commit();

        void Dispose(IIocResolver iocResolver);
    }
}
