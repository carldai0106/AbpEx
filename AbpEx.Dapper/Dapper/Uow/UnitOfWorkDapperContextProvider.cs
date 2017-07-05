using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Dapper.DbContext;

namespace Abp.Dapper.Uow
{
    public class UnitOfWorkDapperContextProvider : IDapperDbContextProvider      
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        public UnitOfWorkDapperContextProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public DapperDbContext GetDapperDbContext()
        {
            return GetDapperDbContext(null);
        }

        public DapperDbContext GetDapperDbContext(MultiTenancySides? multiTenancySide)
        {
            return _currentUnitOfWorkProvider.Current.GetDapperDbContext(multiTenancySide);
        }
    }
}
