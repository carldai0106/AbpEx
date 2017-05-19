using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework.Batch;
using Abp.EntityFramework.Repositories;
using Abp.Extensions;
using Abp.Reflection.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using EntityFramework.DynamicFilters;
using EntityFramework.Extensions;
using EntityFramework.Mapping;

namespace Abp.EntityFramework.Extensions
{
    public static class BatchOperationExtensions
    {
        public static IEnumerable<TEntity> BatchInsert<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> entities)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var iocResolver = ((AbpRepositoryBase<TEntity, TPrimaryKey>)repository).IocResolver;
            if (iocResolver.IsRegistered<IAbpSession>())
            {
                using (var obj = iocResolver.ResolveAsDisposable<IAbpSession>())
                {
                    var abpSession = obj.Object;
                    var dbContext = repository.GetDbContext();
                    SetCreationAuditProperties(entities, abpSession);
                    return dbContext.Set<TEntity>().AddRange(entities);
                }
            }
            throw new AbpException("IAbpSession can not be resolved.");
        }

        public static Task<IEnumerable<TEntity>> BatchInsertAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> entities)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            return Task.FromResult(BatchInsert(repository, entities));
        }

        public static int BatchDelete<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate)
             where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var iocResolver = ((AbpRepositoryBase<TEntity, TPrimaryKey>)repository).IocResolver;
            if (!iocResolver.IsRegistered<IAbpSession>())
            {
                throw new AbpException("IAbpSession can not be resolved.");
            }

            using (var obj = iocResolver.ResolveAsDisposable<IAbpSession>())
            using (var runner = iocResolver.ResolveAsDisposable<IAbpBatchRunner>())
            {
                var dbContext = repository.GetDbContext();
                var abpSession = obj.Object;
                var table = dbContext.Set<TEntity>();
                var sourceQuery = table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                if (!typeof(TEntity).IsInheritsOrImplements(typeof(ISoftDelete)))
                {
                    return runner.Object.Delete(dbContext, abpSession, entityMap, sourceQuery);
                }

                if (typeof(TEntity).IsInheritsOrImplements(typeof(ISoftDelete)) &&
                    !dbContext.IsFilterEnabled(AbpDataFilters.SoftDelete))
                {
                    return runner.Object.Delete(dbContext, abpSession, entityMap, sourceQuery);
                }

                var lambda = GetDeleteExpression<TEntity>(abpSession);
                return runner.Object.Update(dbContext, abpSession, entityMap, sourceQuery, lambda);
            }
        }

        public static  Task<int> BatchDeleteAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var iocResolver = ((AbpRepositoryBase<TEntity, TPrimaryKey>)repository).IocResolver;
            if (!iocResolver.IsRegistered<IAbpSession>())
            {
                throw new AbpException("IAbpSession can not be resolved.");
            }

            using (var obj = iocResolver.ResolveAsDisposable<IAbpSession>())
            using (var runner = iocResolver.ResolveAsDisposable<IAbpBatchRunner>())
            {
                var dbContext = repository.GetDbContext();
                var abpSession = obj.Object;
                var table = dbContext.Set<TEntity>();
                var sourceQuery = table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                if (!typeof(TEntity).IsInheritsOrImplements(typeof(ISoftDelete)))
                {
                    return runner.Object.DeleteAsync(dbContext, abpSession, entityMap, sourceQuery);
                }

                if (typeof(TEntity).IsInheritsOrImplements(typeof(ISoftDelete)) &&
                   !dbContext.IsFilterEnabled(AbpDataFilters.SoftDelete))
                {
                    return runner.Object.DeleteAsync(dbContext, abpSession, entityMap, sourceQuery);
                }

                var lambda = GetDeleteExpression<TEntity>(abpSession);
                return runner.Object.UpdateAsync(dbContext, abpSession, entityMap, sourceQuery, lambda);
            }
        }

        public static int BatchUpdate<TEntity, TPrimaryKey>(
           this IRepository<TEntity, TPrimaryKey> repository,
           Expression<Func<TEntity, bool>> predicate,
           Expression<Func<TEntity, TEntity>> updateExpression)
           where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var iocResolver = ((AbpRepositoryBase<TEntity, TPrimaryKey>)repository).IocResolver;
            if (!iocResolver.IsRegistered<IAbpSession>())
            {
                throw new AbpException("IAbpSession can not be resolved.");
            }

            using (var obj = iocResolver.ResolveAsDisposable<IAbpSession>())
            using (var runner = iocResolver.ResolveAsDisposable<IAbpBatchRunner>())
            {
                var dbContext = repository.GetDbContext();
                var abpSession = obj.Object;
                var table = dbContext.Set<TEntity>();
                var sourceQuery = table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                updateExpression = GetUpdateExpression(abpSession, updateExpression);
                return runner.Object.Update(dbContext, abpSession, entityMap, sourceQuery, updateExpression);
            }
        }

        public static Task<int> BatchUpdateAsync<TEntity, TPrimaryKey>(
           this IRepository<TEntity, TPrimaryKey> repository,
           Expression<Func<TEntity, bool>> predicate,
           Expression<Func<TEntity, TEntity>> updateExpression)
           where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var iocResolver = ((AbpRepositoryBase<TEntity, TPrimaryKey>)repository).IocResolver;
            if (!iocResolver.IsRegistered<IAbpSession>())
            {
                throw new AbpException("IAbpSession can not be resolved.");
            }

            using (var obj = iocResolver.ResolveAsDisposable<IAbpSession>())
            using (var runner = iocResolver.ResolveAsDisposable<IAbpBatchRunner>())
            {
                var dbContext = repository.GetDbContext();
                var abpSession = obj.Object;
                var table = dbContext.Set<TEntity>();
                var sourceQuery = table.Where(predicate).ToObjectQuery();
                var entityMap = sourceQuery.GetEntityMap<TEntity>();

                updateExpression = GetUpdateExpression(abpSession, updateExpression);
                return runner.Object.UpdateAsync(dbContext, abpSession, entityMap, sourceQuery, updateExpression);
            }
        }

        private static void SetCreationAuditProperties<TEntity>(IEnumerable<TEntity> entities, IAbpSession abpSession)
        {
            foreach (var item in entities)
            {
                var entityWithCreationTime = item as IHasCreationTime;
                if (entityWithCreationTime == null)
                {
                    continue;
                }

                if (entityWithCreationTime.CreationTime == default(DateTime))
                {
                    entityWithCreationTime.CreationTime = Clock.Now;
                }

                if (abpSession?.UserId != null && item is ICreationAudited)
                {
                    var entity = item as ICreationAudited;
                    if (entity.CreatorUserId == null)
                    {
                        if (entity is IMayHaveTenant || entity is IMustHaveTenant)
                        {
                            //Sets CreatorUserId only if current user is in same tenant/host with the given entity
                            if ((entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == abpSession.TenantId) ||
                                (entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == abpSession.TenantId))
                            {
                                entity.CreatorUserId = abpSession?.UserId;
                            }
                        }
                        else
                        {
                            entity.CreatorUserId = abpSession?.UserId;
                        }
                    }
                }
            }
        }

        private static Expression<Func<TEntity, TEntity>> GetUpdateExpression<TEntity>(IAbpSession abpSession, Expression<Func<TEntity, TEntity>> updateExpression)
        {
            var list = new List<MemberAssignment>();

            var pars = updateExpression.Parameters;
            if (pars.Count != 1)
            {
                throw new ArgumentException("The number of parameters of update expression allows only one.",
                    nameof(updateExpression));
            }

            var parameterName = pars.ElementAt(0).Name;
            var paramExp = Expression.Parameter(typeof(TEntity), parameterName);
            var newEntity = Expression.New(typeof(TEntity));

            if (typeof(TEntity).IsInheritsOrImplements(typeof(IHasModificationTime)))
            {
                var propLastModificationTime = typeof(TEntity).GetProperty("LastModificationTime");
                if (propLastModificationTime != null)
                {
                    var bindLastModificationTime = Expression.Bind(propLastModificationTime,
                        Expression.Constant(Clock.Now, propLastModificationTime.PropertyType));
                    list.Add(bindLastModificationTime);
                }
            }

            if (abpSession?.UserId != null && typeof(TEntity).IsInheritsOrImplements(typeof(IModificationAudited)))
            {
                var propLastModifierUserId = typeof(TEntity).GetProperty("LastModifierUserId");
                if (propLastModifierUserId != null)
                {
                    var bindLastModifierUserId = Expression.Bind(propLastModifierUserId,
                        Expression.Constant(abpSession?.UserId, propLastModifierUserId.PropertyType));
                    list.Add(bindLastModifierUserId);
                }
            }

            var memberUpdateExpression = updateExpression.Body as MemberInitExpression;
            if (memberUpdateExpression == null)
            {
                throw new ArgumentException("The update expression must be of type MemberInitExpression.",
                    nameof(updateExpression));
            }

            //Get all MemeberAssignment from updateExpression and Merged updateMemeber to new List.
            var updateMemebers = memberUpdateExpression.Bindings.Cast<MemberAssignment>().ToList();
            list.AddRange(updateMemebers);

            //Memeber init using merged MemeberAssignment
            var memberInitExpression = Expression.MemberInit(newEntity, list);
            var lambda = Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, paramExp);

            return lambda;
        }

        private static Expression<Func<TEntity, TEntity>> GetDeleteExpression<TEntity>(IAbpSession abpSession)
        {
            var list = new List<MemberAssignment>();
            var paramExp = Expression.Parameter(typeof(TEntity), "x");
            var newEntity = Expression.New(typeof(TEntity));

            var propIsDeleted = typeof(TEntity).GetProperty("IsDeleted");
            if (propIsDeleted != null)
            {
                var bindIsDeleted = Expression.Bind(propIsDeleted, Expression.Constant(true, propIsDeleted.PropertyType));
                list.Add(bindIsDeleted);
            }

            if (typeof(TEntity).IsInheritsOrImplements(typeof(IHasDeletionTime)))
            {
                var propDeletionTime = typeof(TEntity).GetProperty("DeletionTime");
                if (propDeletionTime != null)
                {
                    var bindDeletionTime = Expression.Bind(propDeletionTime,
                        Expression.Constant(Clock.Now, propDeletionTime.PropertyType));
                    list.Add(bindDeletionTime);
                }
            }

            if (abpSession?.UserId != null && typeof(TEntity).IsInheritsOrImplements(typeof(IDeletionAudited)))
            {
                var propDeleterUserId = typeof(TEntity).GetProperty("DeleterUserId");
                if (propDeleterUserId != null)
                {
                    var bindDeleterUserId = Expression.Bind(propDeleterUserId,
                        Expression.Constant(abpSession?.UserId, propDeleterUserId.PropertyType));
                    list.Add(bindDeleterUserId);
                }
            }

            var memberInitExpression = Expression.MemberInit(newEntity, list);
            var lambda = Expression.Lambda<Func<TEntity, TEntity>>(memberInitExpression, paramExp);
            return lambda;
        }
    }
}
