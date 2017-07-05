using System.Collections.Generic;
using Abp.Dapper.DbContext;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Collections.Extensions;
using DapperExtensions.Sql;
using Abp.Transactions.Extensions;
using System.Data;

namespace Abp.Dapper.Uow
{
    public class DapperTransactionStrategy : IDapperTransactionStrategy
    {
        protected UnitOfWorkOptions Options { get; private set; }

        protected IDictionary<string, ActiveTransactionInfo> ActiveTransactions { get; }

        public DapperTransactionStrategy()
        {
            ActiveTransactions = new Dictionary<string, ActiveTransactionInfo>();
        }

        public void InitOptions(UnitOfWorkOptions options)
        {
            Options = options;
        }


        public void Commit()
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                activeTransaction.DbTransaction.Commit();
            }
        }

        public DapperDbContext CreateDbContext(IDbConnection connection, ISqlGenerator sqlGenerator, IDbContextResolver dbContextResolver)
        {
            DapperDbContext dbContext;

            var connectionString = connection.ConnectionString;
            var activeTransaction = ActiveTransactions.GetOrDefault(connection.ConnectionString);
            if (activeTransaction == null)
            {
                dbContext = dbContextResolver.Resolve(connection, sqlGenerator);
                var isolationLevel = Options.IsolationLevel ?? System.Transactions.IsolationLevel.ReadUncommitted;

                var dbtransaction = dbContext.Database.BeginTransaction((isolationLevel).ToSystemDataIsolationLevel());                
                activeTransaction = new ActiveTransactionInfo(dbtransaction, dbContext);
                ActiveTransactions[connectionString] = activeTransaction;
            }
            else
            {
                dbContext = dbContextResolver.Resolve(connection, sqlGenerator);
                dbContext.Database.UseTransaction(activeTransaction.DbTransaction);
                activeTransaction.AttendedDbContexts.Add(dbContext);
            }

            return dbContext;
        }

        public void Dispose(IIocResolver iocResolver)
        {
            foreach (var activeTransaction in ActiveTransactions.Values)
            {
                foreach (var attendedDbContext in activeTransaction.AttendedDbContexts)
                {
                    iocResolver.Release(attendedDbContext);
                }
                
                activeTransaction.DbTransaction.Dispose();
                iocResolver.Release(activeTransaction.StarterDbContext);
            }

            ActiveTransactions.Clear();
        }
    }
}
