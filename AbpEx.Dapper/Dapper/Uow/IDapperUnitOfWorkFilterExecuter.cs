using Abp.Dapper.DbContext;
using Abp.Domain.Uow;

namespace Abp.Dapper.Uow
{
    public interface IDapperUnitOfWorkFilterExecuter : IUnitOfWorkFilterExecuter
    {
        void ApplyCurrentFilters(IUnitOfWork unitOfWork, DapperDbContext dbContext);
    }
}
