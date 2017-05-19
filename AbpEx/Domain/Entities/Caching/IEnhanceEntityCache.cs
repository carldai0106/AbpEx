using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Runtime.Caching;

namespace Abp.Domain.Entities.Caching
{
    public interface IEnhanceEntityCache<TCacheItem> : IEnhanceEntityCache<TCacheItem, int>
    {

    }

    public interface IEnhanceEntityCache<TCacheItem, TPrimaryKey> 
    {
        TCacheItem this[TPrimaryKey id] { get; }

        string CacheName { get; }

        ITypedCache<string, List<TCacheItem>> InternalCache { get; }

        TCacheItem Get(TPrimaryKey id);

        Task<TCacheItem> GetAsync(TPrimaryKey id);

        /// <summary>
        /// Gets the cache key : The key is cache list key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        string CacheKey { get; }

        List<TCacheItem> GetList();

        Task<List<TCacheItem>> GetListAsync();
    }
}
