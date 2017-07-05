using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;

namespace Abp.Dapper.Repositories
{
    /// <summary>
    ///     Base class to implement <see cref="IDapperRepository{TEntity,TPrimaryKey}" />.
    ///     It implements some methods in most simple way.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
    /// <seealso cref="IDapperRepository{TEntity,TPrimaryKey}" />
    public abstract class AbpDapperRepositoryBase<TEntity, TPrimaryKey> : IDapperRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        public static MultiTenancySides? MultiTenancySide { get; private set; }

        static AbpDapperRepositoryBase()
        {
            var attr = typeof(TEntity).GetSingleAttributeOfTypeOrBaseTypesOrNull<MultiTenancySideAttribute>();
            if (attr != null)
            {
                MultiTenancySide = attr.Side;
            }
        }

        public abstract int Count(Expression<Func<TEntity, bool>> predicate = null);        

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return Task.FromResult(Count(predicate));
        }

        public abstract void Delete(TPrimaryKey id);

        public abstract void Delete(TEntity entity);

        public abstract void Delete(Expression<Func<TEntity, bool>> predicate);

        public Task DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
            return Task.FromResult(0);
        }

        public Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
            return Task.FromResult(0);
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
            return Task.FromResult(0);
        }

        public abstract TEntity Get(TPrimaryKey id);

        public Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return Task.FromResult(Get(id));
        }

        public abstract IEnumerable<TEntity> GetAll();

        public abstract IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null);

        public Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }

        public Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return Task.FromResult(GetList(predicate));
        }

        public abstract IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true);

        public abstract IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression);        

        public Task<IEnumerable<TEntity>> GetListPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true)
        {
            return Task.FromResult(GetListPaged(predicate, pageNumber, itemsPerPage, sortingProperty, ascending));
        }

        public Task<IEnumerable<TEntity>> GetListPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            return Task.FromResult(GetListPaged(predicate, pageNumber, itemsPerPage, ascending, sortingExpression));
        }

        public abstract IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true);

        public abstract IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression);

        public Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            return Task.FromResult(GetSet(predicate, firstResult, maxResults, sortingProperty, ascending));
        }

        public Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            return Task.FromResult(GetSet(predicate, firstResult, maxResults, ascending, sortingExpression));
        }

        public abstract void Insert(TEntity entity);

        public abstract TPrimaryKey InsertAndGetId(TEntity entity);

        public abstract void BatchInsert(IEnumerable<TEntity> entities);

        public Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {            
            return Task.FromResult(InsertAndGetId(entity));
        }

        public Task InsertAsync(TEntity entity)
        {
            Insert(entity);
            return Task.FromResult(0);
        }       

        public Task BatchInsertAsync(IEnumerable<TEntity> entities)
        {
            BatchInsert(entities);
            return Task.FromResult(0);
        }

        public IEnumerable<TEntity> Query(string query)
        {
            return Query(query, null);
        }

        public abstract IEnumerable<TEntity> Query(string query, object parameters);

        public abstract IEnumerable<TAny> Query<TAny>(string query, object parameters) where TAny : class;

        public abstract IEnumerable<TAny> Query<TAny>(string query) where TAny : class;

        public Task<IEnumerable<TAny>> QueryAsync<TAny>(string query, object parameters) where TAny : class
        {
            return Task.FromResult(Query<TAny>(query, parameters));
        }

        public Task<IEnumerable<TAny>> QueryAsync<TAny>(string query) where TAny : class
        {
            return Task.FromResult(Query<TAny>(query));
        }

        public Task<IEnumerable<TEntity>> QueryAsync(string query, object parameters)
        {
            return Task.FromResult(Query<TEntity>(query, parameters));
        }

        public abstract void Update(TEntity entity);

        public abstract void BatchUpdate(Expression<Func<TEntity, bool>> predicate, Action<TEntity> update);

        public Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.FromResult(0);
        }      

        public Task BatchUpdateAsync(Expression<Func<TEntity, bool>> predicate, Action<TEntity> update)
        {
            BatchUpdate(predicate, update);
            return Task.FromResult(0);
        }

        public abstract int Execute(string sql, object parameters = null);        

        public Task<int> ExecuteAsync(string sql, object parameters = null)
        {
            return Task.FromResult(Execute(sql, parameters));
        }

        public abstract TEntity Single(TPrimaryKey id);

        public abstract TEntity Single(Expression<Func<TEntity, bool>> predicate);

        public Task<TEntity> SingleAsync(TPrimaryKey id)
        {
            return Task.FromResult(Single(id));
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Single(predicate));
        }

        public abstract TEntity FirstOrDefault(TPrimaryKey id);

        public abstract TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        public Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return Task.FromResult(FirstOrDefault(id));
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }
    }
}
