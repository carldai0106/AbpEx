using Abp.Dapper.DbContext;
using System.Collections.Generic;
using System.Data;

namespace Abp.Dapper.Uow
{
    public class ActiveTransactionInfo
    {
        public IDbTransaction DbTransaction { get; }

        public DapperDbContext StarterDbContext { get; }

        public List<DapperDbContext> AttendedDbContexts { get; }

        public ActiveTransactionInfo(IDbTransaction dbTransaction, DapperDbContext starterDbContext)
        {
            DbTransaction = dbTransaction;
            StarterDbContext = starterDbContext;

            AttendedDbContexts = new List<DapperDbContext>();
        }
    }
}
