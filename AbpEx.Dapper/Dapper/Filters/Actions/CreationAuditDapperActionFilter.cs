using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Abp.Timing;
using Abp.Domain.Uow;

namespace Abp.Dapper.Filters.Actions
{
    public class CreationAuditDapperActionFilter : DapperActionFilterBase, IDapperActionFilter
    {       
        public void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            long? userId = GetAuditUserId();
            CheckAndSetId(entity);
            var entityWithCreationTime = entity as IHasCreationTime;
            if (entityWithCreationTime == null)
            {
                return;
            }

            if (entityWithCreationTime.CreationTime == default(DateTime))
            {
                entityWithCreationTime.CreationTime = Clock.Now;
            }

            CheckAndSetMustHaveTenantIdProperty(entity);
            CheckAndSetMayHaveTenantIdProperty(entity);

            if (userId.HasValue && entity is ICreationAudited)
            {
                var record = entity as ICreationAudited;
                if (record.CreatorUserId == null)
                {
                    if (entity is IMayHaveTenant || entity is IMustHaveTenant)
                    {
                        //Sets CreatorUserId only if current user is in same tenant/host with the given entity
                        if (entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == AbpSession.TenantId ||
                            entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == AbpSession.TenantId)
                        {
                            record.CreatorUserId = userId;
                        }
                    }
                    else
                    {
                        record.CreatorUserId = userId;
                    }
                }
            }

            if (entity is IHasModificationTime)
            {
                entity.As<IHasModificationTime>().LastModificationTime = null;
            }

            if (entity is IModificationAudited)
            {
                var record = entity.As<IModificationAudited>();
                record.LastModifierUserId = null;
            }
        }

        protected virtual void CheckAndSetMustHaveTenantIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetTenantId)
            {
                return;
            }

            //Only set IMustHaveTenant entities
            if (!(entityAsObj is IMustHaveTenant))
            {
                return;
            }

            var entity = entityAsObj.As<IMustHaveTenant>();

            //Don't set if it's already set
            if (entity.TenantId != 0)
            {
                return;
            }

            var currentTenantId = GetCurrentTenantIdOrNull();

            if (currentTenantId != null)
            {
                entity.TenantId = currentTenantId.Value;
            }
            else
            {
                throw new AbpException("Can not set TenantId to 0 for IMustHaveTenant entities!");
            }
        }

        protected virtual void CheckAndSetMayHaveTenantIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetTenantId)
            {
                return;
            }

            //Only set IMayHaveTenant entities
            if (!(entityAsObj is IMayHaveTenant))
            {
                return;
            }

            var entity = entityAsObj.As<IMayHaveTenant>();

            //Don't set if it's already set
            if (entity.TenantId != null)
            {
                return;
            }

            //Only works for single tenant applications
            if (MultiTenancyConfig?.IsEnabled ?? false)
            {
                return;
            }

            if (CurrentUnitOfWorkProvider.Current != null)
            {
                //Don't set if MayHaveTenant filter is disabled
                if (!CurrentUnitOfWorkProvider.Current.IsFilterEnabled(AbpDataFilters.MayHaveTenant))
                {
                    return;
                }
            }

            entity.TenantId = GetCurrentTenantIdOrNull();
        }

    }
}
