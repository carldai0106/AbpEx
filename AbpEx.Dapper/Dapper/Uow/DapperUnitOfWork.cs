using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using System.Data;
using Abp.MultiTenancy;
using Abp.Dapper.DbContext;
using Abp.Dapper.Common;
using System.Collections.Generic;
using Abp.Dapper.Configuration;
using Abp.Extensions;
using System.Collections.Immutable;
using Abp.Runtime.Session;

namespace Abp.Dapper.Uow
{
    public class DapperUnitOfWork : UnitOfWorkBase, ITransientDependency
    {
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;
        private readonly IDapperTransactionStrategy _transactionStrategy;
        private readonly IDbContextResolver _dbContextResolver;
        private readonly IDapperConfiguration _dapperConfiguration;

        private IIocResolver IocResolver { get; }
        protected IDictionary<string, DapperDbContext> ActiveDbContexts { get; }        

        public DapperUnitOfWork(
            IIocResolver iocResolver,
            IDbContextTypeMatcher dbContextTypeMatcher,
            IDapperTransactionStrategy transactionStrategy,
            IDbContextResolver dbContextResolver,
            IDapperConfiguration dapperConfiguration,                
            IConnectionStringResolver connectionStringResolver,
            IUnitOfWorkDefaultOptions defaultOptions,
            IUnitOfWorkFilterExecuter filterExecuter
            )
            : base(connectionStringResolver, defaultOptions, filterExecuter)
        {
            _dbContextTypeMatcher = dbContextTypeMatcher;
            _transactionStrategy = transactionStrategy;
            _dbContextResolver = dbContextResolver;
            _dapperConfiguration = dapperConfiguration;            
            IocResolver = iocResolver;

            ActiveDbContexts = new Dictionary<string, DapperDbContext>();
        }       

        protected override void BeginUow()
        {
            if (Options.IsTransactional == true)
            {
                _transactionStrategy.InitOptions(Options);
            }

            CheckAndSetMayHaveTenant();
            CheckAndSetMustHaveTenant();
        }

        protected virtual void CheckAndSetMustHaveTenant()
        {
            if (IsFilterEnabled(AbpDataFilters.MustHaveTenant))
            {
                return;
            }

            if (AbpSession.TenantId == null)
            {
                return;
            }

            ApplyEnableFilter(AbpDataFilters.MustHaveTenant); //Enable Filters
            ApplyFilterParameterValue(AbpDataFilters.MustHaveTenant, AbpDataFilters.Parameters.TenantId, AbpSession.GetTenantId()); //ApplyFilter
        }

        protected virtual void CheckAndSetMayHaveTenant()
        {
            if (IsFilterEnabled(AbpDataFilters.MayHaveTenant))
            {
                return;
            }

            if (AbpSession.TenantId == null)
            {
                return;
            }

            ApplyEnableFilter(AbpDataFilters.MayHaveTenant); //Enable Filters
            ApplyFilterParameterValue(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, AbpSession.TenantId); //ApplyFilter
        }

        public override void SaveChanges()
        {
            GetAllActiveDbContexts().ForEach(SaveChangesInDbContext);
        }

        public IReadOnlyList<DapperDbContext> GetAllActiveDbContexts()
        {
            return ActiveDbContexts.Values.ToImmutableList();
        }

        public override async Task SaveChangesAsync()
        {
            foreach (var dbContext in GetAllActiveDbContexts())
            {
                await SaveChangesInDbContextAsync(dbContext);
            }
        }      

        protected override void CompleteUow()
        {
            SaveChanges();

            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Commit();
            }
        }

        protected override async Task CompleteUowAsync()
        {
            await SaveChangesAsync();

            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Commit();
            }
        }

        public virtual DapperDbContext GetOrCreateDbContext(MultiTenancySides? multiTenancySide = null)
        {            
            var concreteDbContextType = _dbContextTypeMatcher.GetConcreteType(typeof(DapperDbContext));

            var connectionStringResolveArgs = new ConnectionStringResolveArgs(multiTenancySide);     

            connectionStringResolveArgs["DbContextType"] = typeof(DapperDbContext);
            connectionStringResolveArgs["DbContextConcreteType"] = concreteDbContextType;

            var connectionString = ResolveConnectionString(connectionStringResolveArgs);
            var dbConnection = _dapperConfiguration.DbConnection;
            var sqlGenerator = _dapperConfiguration.SqlGenerator;
            var dbContextKey = concreteDbContextType.FullName + "#" + connectionString;

            DapperDbContext dbContext;
            if (!ActiveDbContexts.TryGetValue(dbContextKey, out dbContext))
            {
                if (dbConnection.State == ConnectionState.Open)
                {
                    dbConnection.Close();                   
                }

                dbConnection.ConnectionString = connectionString;

                if (Options.IsTransactional == true)
                {
                    dbContext = _transactionStrategy.CreateDbContext(dbConnection, sqlGenerator, _dbContextResolver);
                }
                else
                {
                    dbContext = _dbContextResolver.Resolve(dbConnection, sqlGenerator);
                }

                if (Options.Timeout.HasValue && !dbContext.Database.CommandTimeout.HasValue)
                {
                    dbContext.Database.CommandTimeout = Options.Timeout.Value.TotalSeconds.To<int>();
                }              

                ActiveDbContexts[dbContextKey] = dbContext;
            }

            return dbContext;
        }       

        protected override void DisposeUow()
        {
            if (Options.IsTransactional == true)
            {
                _transactionStrategy.Dispose(IocResolver);
            }
            else
            {
                foreach (var activeDbContext in GetAllActiveDbContexts())
                {
                    Release(activeDbContext);
                }
            }

            ActiveDbContexts.Clear();
        }

        protected virtual void SaveChangesInDbContext(DapperDbContext dbContext)
        {
            dbContext.SaveChanges();
        }

        protected virtual async Task SaveChangesInDbContextAsync(DapperDbContext dbContext)
        {
            await dbContext.SaveChangesAsync();
        }

        protected virtual void Release(DapperDbContext dbContext)
        {
            dbContext.Dispose();
            IocResolver.Release(dbContext);
        }
    }
}
