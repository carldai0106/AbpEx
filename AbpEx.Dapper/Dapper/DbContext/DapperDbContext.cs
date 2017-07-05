using System.Data;
using System.Threading.Tasks;
using Abp.Dependency;
using DapperExtensions.Sql;

namespace Abp.Dapper.DbContext
{
    public class DapperDbContext : IDapperDbContext, ITransientDependency
    {
        private Database _database;

        public Database Database
        {
            get
            {
                return _database;
            }
        }

        public DapperDbContext(IDbConnection connection, ISqlGenerator sqlGenerator)
        {
            _database = new Database(connection, sqlGenerator);           
        }     

        public void SaveChanges()
        {
            
        }

        public async Task SaveChangesAsync()
        {
            SaveChanges();
            await Task.FromResult(0);
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}
