using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Dapper.Extensions;
using Abp.Domain.Entities;
using System.Linq;
using Abp.Dapper.Filters.Actions;
using Abp.Dapper.Filters.Queries;
using Abp.Dapper.DbContext;
using Abp.Extensions;

namespace Abp.Dapper.Repositories
{
    public class DapperRepositoryBase<TEntity, TPrimaryKey> : AbpDapperRepositoryBase<TEntity, TPrimaryKey>     
        where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly IDapperDbContextProvider _dapperContextProvider;

        public IDapperDbContext Context => _dapperContextProvider.GetDapperDbContext(MultiTenancySide);
      
        public IDapperQueryFilterExecuter DapperQueryFilterExecuter { get; set; }        

        public IDapperActionFilterExecuter DapperActionFilterExecuter { get; set; }

        public int? CommandTimeout { get { return Database.CommandTimeout; } }

        public Database Database { get { return Context.Database; } }

        public DapperRepositoryBase(IDapperDbContextProvider dapperContextProvider)
        {
            _dapperContextProvider = dapperContextProvider;         
            DapperQueryFilterExecuter = NullDapperQueryFilterExecuter.Instance;
            DapperActionFilterExecuter = NullDapperActionFilterExecuter.Instance;
        }

        public override void BatchInsert(IEnumerable<TEntity> entities)
        {
            entities.ForEach((entity) => {
                DapperActionFilterExecuter.ExecuteCreationAuditFilter<TEntity, TPrimaryKey>(entity);                
            });
            Database.Insert(entities, CommandTimeout);
        }

        public override void BatchUpdate(Expression<Func<TEntity, bool>> predicate, Action<TEntity> update)
        {
            var list = GetList(predicate);
            list.ForEach((e) => {
                DapperActionFilterExecuter.ExecuteModificationAuditFilter<TEntity, TPrimaryKey>(e);
                update?.Invoke(e);

                Database.Update<TEntity>(e, CommandTimeout);
                //todo : if no entity is in database, do we need insert to database?
                //if (!Context.Update(entity))
                //{
                //    var filter = Context.ActionFilters.FirstOrDefault(x => x.GetType() == typeof(CreationAuditActionFilter));
                //    filter?.ExecuteFilter<TEntity, TPrimaryKey>(entity);
                //    Context.Insert(entity);
                //}   
            });                    
        }       

        public override int Count(Expression<Func<TEntity, bool>> predicate = null)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Database.Count<TEntity>(pg, CommandTimeout);
        }

        public override void Delete(TPrimaryKey id)
        {
            var entity = Get(id);
            Delete(entity);
        }

        public override void Delete(TEntity entity)
        {
            if (entity is ISoftDelete)
            {
                DapperActionFilterExecuter.ExecuteDeletionAuditFilter<TEntity, TPrimaryKey>(entity);
                Database.Update(entity, CommandTimeout);
            }
            else
            {
                Database.Delete(entity, CommandTimeout);
            }
        }

        public override void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> items = GetList(predicate);
            foreach (TEntity entity in items)
            {
                Delete(entity);
            }
        }

        public override TEntity Get(TPrimaryKey id)
        {
            var expression = Utils.ExpressionUtils.MakePredicate<TEntity>("Id", id, typeof(TPrimaryKey));
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(expression);
            return Database.GetList<TEntity>(pg, null, CommandTimeout, true).FirstOrDefault();
        }

        public override IEnumerable<TEntity> GetAll()
        {          
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
            return Database.GetList<TEntity>(pg, null, CommandTimeout, true);
        }

        public override IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Database.GetList<TEntity>(pg, null, CommandTimeout, true);
        }

        public override IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            var sorts = new List<DapperExtensions.ISort> { new DapperExtensions.Sort { Ascending = ascending, PropertyName = sortingProperty } };
            return Database.GetPage<TEntity>(pg, sorts, pageNumber, itemsPerPage, CommandTimeout, true);
        }

        public override IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            var sorts = sortingExpression.ToSortable(ascending);
            return Database.GetPage<TEntity>(pg, sorts, pageNumber, itemsPerPage, CommandTimeout, true);
        }

        public override IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            var sorts = new List<DapperExtensions.ISort> { new DapperExtensions.Sort { Ascending = ascending, PropertyName = sortingProperty } };
            return Database.GetSet<TEntity>(pg, sorts, firstResult, maxResults, CommandTimeout, true);
        }

        public override IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            var sorts = sortingExpression.ToSortable(ascending);
            return Database.GetSet<TEntity>(pg, sorts, firstResult, maxResults, CommandTimeout, true);
        }

        public override void Insert(TEntity entity)
        {
            InsertAndGetId(entity);
        }

        public override TPrimaryKey InsertAndGetId(TEntity entity)
        {           
            DapperActionFilterExecuter.ExecuteCreationAuditFilter<TEntity, TPrimaryKey>(entity);
            var id = Database.Insert(entity, CommandTimeout);
            return id;
        }       

        public override IEnumerable<TEntity> Query(string query, object parameters)
        {
            return Database.Query<TEntity>(query, parameters);
        }

        public override IEnumerable<TAny> Query<TAny>(string query, object parameters)
        {
            return Database.Query<TAny>(query, parameters);
        }

        public override IEnumerable<TAny> Query<TAny>(string query)
        {
            return Database.Query<TAny>(query);
        }

        public override void Update(TEntity entity)
        {
            DapperActionFilterExecuter.ExecuteModificationAuditFilter<TEntity, TPrimaryKey>(entity);
            Database.Update<TEntity>(entity, CommandTimeout);
        }

        public override int Execute(string sql, object parameters = null)
        {
            return Database.Execute(sql, parameters, CommandTimeout);
        }

        public override TEntity Single(TPrimaryKey id)
        {
            var expression = Utils.ExpressionUtils.MakePredicate<TEntity>("Id", id, typeof(TPrimaryKey));
            return Single(expression);
        }

        public override TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Database.GetList<TEntity>(pg, null, CommandTimeout, true).Single();
        }

        public override TEntity FirstOrDefault(TPrimaryKey id)
        {
            var expression = Utils.ExpressionUtils.MakePredicate<TEntity>("Id", id, typeof(TPrimaryKey));
            return FirstOrDefault(expression);
        }

        public override TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            var pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Database.GetList<TEntity>(pg, null, CommandTimeout, true).FirstOrDefault();
        }
    }
}
