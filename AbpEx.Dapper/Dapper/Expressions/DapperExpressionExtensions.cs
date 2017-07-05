using System;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using DapperExtensions;
using JetBrains.Annotations;

namespace Abp.Dapper.Expressions
{
    internal static class DapperExpressionExtensions
    {        
        public static IPredicate ToPredicateGroup<TEntity, TPrimaryKey>(this Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (expression == null)
                return null;

            var dev = new DapperExpressionVisitor<TEntity, TPrimaryKey>();
            IPredicate pg = dev.Process(expression);

            return pg;
        }
    }
}
