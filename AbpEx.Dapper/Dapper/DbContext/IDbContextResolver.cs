using DapperExtensions.Sql;
using System.Data;

namespace Abp.Dapper.DbContext
{
    public interface IDbContextResolver
    {
        DapperDbContext Resolve(IDbConnection connection, ISqlGenerator sqlGenerator);
    }
}
