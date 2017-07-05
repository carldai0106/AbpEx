using Abp.Configuration.Startup;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Abp.Dapper.Filters.Actions
{
    public abstract class DapperActionFilterBase
    {
        /// <summary>
        /// Gets or sets the abp session.
        /// </summary>
        /// <value>
        /// The abp session.
        /// </value>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Gets or sets the current unit of work provider.
        /// </summary>
        /// <value>
        /// The current unit of work provider.
        /// </value>
        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier generator.
        /// </summary>
        /// <value>
        /// The unique identifier generator.
        /// </value>
        public IGuidGenerator GuidGenerator { get; set; }

        /// <summary>
        /// Reference to multi tenancy configuration.
        /// </summary>
        public IMultiTenancyConfig MultiTenancyConfig { get; set; }

        /// <summary>
        /// Can be used to suppress automatically setting TenantId on SaveChanges.
        /// Default: false.
        /// </summary>
        public bool SuppressAutoSetTenantId { get; set; }

        protected DapperActionFilterBase()
        {
            AbpSession = NullAbpSession.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
        }     

        protected virtual long? GetAuditUserId()
        {
            if (AbpSession.UserId.HasValue && CurrentUnitOfWorkProvider?.Current != null)
            {
                return AbpSession.UserId;
            }

            return null;
        }

        protected virtual void CheckAndSetId(object entityAsObj)
        {
            var entity = entityAsObj as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                var idProperty = entityAsObj.GetType().GetProperty("Id");
                var dbGeneratedAttr = default(DatabaseGeneratedAttribute);
                if (idProperty.IsDefined(typeof(DatabaseGeneratedAttribute), true))
                {
                    dbGeneratedAttr = idProperty.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).Cast<DatabaseGeneratedAttribute>().First();
                }

                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }

        protected virtual int? GetCurrentTenantIdOrNull()
        {
            if (CurrentUnitOfWorkProvider?.Current != null)
            {
                return CurrentUnitOfWorkProvider.Current.GetTenantId();
            }

            return AbpSession.TenantId;
        }
    }
}
