using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.Mapping;
using System.Data.Entity.Core.Objects;
using Abp.Runtime.Session;

namespace Abp.EntityFramework.Batch
{
    public interface IAbpBatchRunner
    {
        int Update<TEntity>(
           DbContext dbContext,
           IAbpSession abpSession,
           EntityMap entityMap,
           ObjectQuery<TEntity> query,
           Expression<Func<TEntity, TEntity>> updateExpression)
           where TEntity : class;

        Task<int> UpdateAsync<TEntity>(
            DbContext dbContext,
            IAbpSession abpSession,
            EntityMap entityMap,
            ObjectQuery<TEntity> query,
            Expression<Func<TEntity, TEntity>> updateExpression)
            where TEntity : class;

        int Delete<TEntity>(
           DbContext dbContext,
           IAbpSession abpSession,
           EntityMap entityMap,
           ObjectQuery<TEntity> query)
           where TEntity : class;


        Task<int> DeleteAsync<TEntity>(
            DbContext dbContext,
            IAbpSession abpSession,
            EntityMap entityMap,
            ObjectQuery<TEntity> query)
            where TEntity : class;
    }
}
