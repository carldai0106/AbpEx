using Abp.MultiTenancy;

namespace Abp.Dapper.DbContext
{
    public interface IDapperDbContextProvider
    { 
        DapperDbContext GetDapperDbContext();

        DapperDbContext GetDapperDbContext(MultiTenancySides? multiTenancySide);
    }
}
