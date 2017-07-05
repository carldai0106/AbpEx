using Abp.Dapper.Common;
using Abp.Dependency;
using System;
using DapperExtensions.Sql;
using System.Data;

namespace Abp.Dapper.DbContext
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        public DefaultDbContextResolver(IIocResolver iocResolver, IDbContextTypeMatcher dbContextTypeMatcher)
        {
            _iocResolver = iocResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
        }   
       
        public DapperDbContext Resolve(IDbConnection connection, ISqlGenerator sqlGenerator)            
        {
            var dbContextType = GetConcreteType<DapperDbContext>();
            return (DapperDbContext)_iocResolver.Resolve(dbContextType, new
            {
                connection = connection,
                sqlGenerator = sqlGenerator
            });
        }

        protected virtual Type GetConcreteType<TDbContext>()
        {
            var dbContextType = typeof(TDbContext);
            return !dbContextType.IsAbstract
                ? dbContextType
                : _dbContextTypeMatcher.GetConcreteType(dbContextType);
        }
    }
}
