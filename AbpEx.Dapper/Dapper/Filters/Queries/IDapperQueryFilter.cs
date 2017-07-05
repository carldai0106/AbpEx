using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using DapperExtensions;
using Abp.Dependency;

namespace Abp.Dapper.Filters.Queries
{
    public interface IDapperQueryFilter : ITransientDependency
    {
        string FilterName { get; }

        bool IsEnabled { get; }      

        IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>;

        IList<TEntity> ExecuteFilter<TEntity, TPrimaryKey>(IList<TEntity> source) where TEntity : class, IEntity<TPrimaryKey>;

        Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>;
    }
}
