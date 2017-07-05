using Abp.Dapper.DbContext;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using System;

namespace Abp.Dapper.Uow
{
    internal static class UnitOfWorkExtensions
    {
        public static DapperDbContext GetDapperDbContext(this IActiveUnitOfWork unitOfWork)
        {
            return GetDapperDbContext(unitOfWork, null);
        }

        public static DapperDbContext GetDapperDbContext(this IActiveUnitOfWork unitOfWork, MultiTenancySides? multiTenancySide)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (!(unitOfWork is DapperUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(DapperUnitOfWork).FullName, nameof(unitOfWork));
            }

            return (unitOfWork as DapperUnitOfWork).GetOrCreateDbContext(multiTenancySide);
        }
    }
}
