using System;
using System.Threading.Tasks;

namespace Abp.Dapper.DbContext
{
    public interface IDapperDbContext : IDisposable
    {       
        Database Database { get; }       

        void SaveChanges();

        Task SaveChangesAsync();      
    }
}
