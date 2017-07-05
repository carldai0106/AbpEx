using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Reflection.Extensions;
using DapperExtensions;
using Abp.Utils;

namespace Abp.Dapper.Filters.Queries
{
    public class MustHaveTenantDapperQueryFilter : IDapperQueryFilter
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public string FilterName => AbpDataFilters.MustHaveTenant;

        public bool IsEnabled
        {
            get
            {

                return _currentUnitOfWorkProvider.Current != null ? _currentUnitOfWorkProvider.Current.IsFilterEnabled(FilterName) : true;
            }
        }

        public MustHaveTenantDapperQueryFilter(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        private int? TenantId
        {
            get
            {
                DataFilterConfiguration filter = _currentUnitOfWorkProvider.Current.Filters.FirstOrDefault(x => x.FilterName == FilterName);
                if (filter.FilterParameters.ContainsKey(AbpDataFilters.Parameters.TenantId))
                {
                    return (int?)filter.FilterParameters[AbpDataFilters.Parameters.TenantId];
                }

                return null;
            }
        }

        public IFieldPredicate ExecuteFilter<TEntity, TPrimaryKey>() where TEntity : class, IEntity<TPrimaryKey>
        {
            IFieldPredicate predicate = null;
            if (typeof(TEntity).IsInheritsOrImplements(typeof(IMustHaveTenant)) && IsEnabled)
            {
                predicate = Predicates.Field<TEntity>(f => (f as IMustHaveTenant).TenantId, Operator.Eq, TenantId);
            }
            return predicate;
        }

        public IList<TEntity> ExecuteFilter<TEntity, TPrimaryKey>(IList<TEntity> source) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (typeof(TEntity).IsInheritsOrImplements(typeof(IMustHaveTenant)) && IsEnabled)
            {
                return source.Where(x =>
                {
                    var mustHaveTenant = x as IMustHaveTenant;
                    return mustHaveTenant != null && mustHaveTenant.TenantId == TenantId;
                }).ToList();
            }

            return source;
        }

        public Expression<Func<TEntity, bool>> ExecuteFilter<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (typeof(TEntity).IsInheritsOrImplements(typeof(IMustHaveTenant)) && IsEnabled)
            {
                var propType = typeof(TEntity).GetProperty(nameof(IMustHaveTenant.TenantId));
                if (predicate == null)
                {
                    predicate = ExpressionUtils.MakePredicate<TEntity>(nameof(IMustHaveTenant.TenantId), TenantId, propType.PropertyType);
                }
                else
                {
                    var paramExpr = predicate.Parameters[0];
                    var memberExpr = Expression.Property(paramExpr, nameof(IMustHaveTenant.TenantId));
                    var body = Expression.AndAlso(
                        predicate.Body,
                        Expression.Equal(memberExpr, Expression.Constant(TenantId, propType.PropertyType)));
                    predicate = Expression.Lambda<Func<TEntity, bool>>(body, paramExpr);
                }
            }
            return predicate;
        }
    }
}
