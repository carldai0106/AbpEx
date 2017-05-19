using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.ObjectMapping;
using Abp.Runtime.Caching;
using Abp.Utils;

namespace Abp.Domain.Entities.Caching
{
    public class EnhanceEntityCache<TEntity, TCacheItem, TPrimaryKey> :
        IEventHandler<EntityChangedEventData<TEntity>>,
        IEnhanceEntityCache<TCacheItem, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TCacheItem: class, IEntity<TPrimaryKey>
    {
        public IObjectMapper ObjectMapper { get; set; }
        protected ICacheManager CacheManager { get; private set; }
        protected IRepository<TEntity, TPrimaryKey> Repository { get; private set; }

        public EnhanceEntityCache(
            ICacheManager cacheManager,
            IRepository<TEntity, TPrimaryKey> repository,
            string cacheName = null)
        {
            Repository = repository;
            CacheManager = cacheManager;
            CacheName = cacheName ?? GenerateDefaultCacheName();
            CacheKey = CacheName + "List";
            ObjectMapper = NullObjectMapper.Instance;
        }

        protected virtual string GenerateDefaultCacheName()
        {
            return GetType().FullName;
        }

        public override string ToString()
        {
            return string.Format("EntityCache {0}", CacheName);
        }

        public virtual void HandleEvent(EntityChangedEventData<TEntity> eventData)
        {
            throw new NotImplementedException();
        }

        public virtual TCacheItem Get(TPrimaryKey id)
        {
            return GetList()
                .FirstOrDefault(ExpressionUtils.MakePredicate<TCacheItem>("Id", id, typeof(TPrimaryKey)).Compile());
        }

        public virtual async Task<TCacheItem> GetAsync(TPrimaryKey id)
        {
            var list = await GetListAsync();
            return list.FirstOrDefault(ExpressionUtils.MakePredicate<TCacheItem>("Id", id, typeof(TPrimaryKey))
                .Compile());
        }

        public TCacheItem this[TPrimaryKey id] => GetList().FirstOrDefault(ExpressionUtils.MakePredicate<TCacheItem>("Id", id, typeof(TPrimaryKey)).Compile());

        public string CacheName { get; }

        public string CacheKey { get; }

        public ITypedCache<string, List<TCacheItem>> InternalCache { get; }

        public virtual List<TCacheItem> GetList()
        {
            return InternalCache.Get(CacheKey, GetCacheItemsFromDataSource);
        }

        public Task<List<TCacheItem>> GetListAsync()
        {
            return InternalCache.GetAsync(CacheKey, GetCacheItemsFromDataSourceAsync);
        }

        protected virtual List<TCacheItem> GetCacheItemsFromDataSource()
        {
            return MapToCacheItems(GetEntityFromDataSource());
        }

        protected virtual async Task<List<TCacheItem>> GetCacheItemsFromDataSourceAsync()
        {
            return MapToCacheItems(await GetEntityFromDataSourceAsync());
        }

        protected virtual List<TEntity> GetEntityFromDataSource()
        {
            return Repository.GetAllList();
        }

        protected virtual Task<List<TEntity>> GetEntityFromDataSourceAsync()
        {
            return Repository.GetAllListAsync();
        }

        protected virtual List<TCacheItem> MapToCacheItems(List<TEntity> entities)
        {
            if (ObjectMapper is NullObjectMapper)
            {
                throw new AbpException(
                    string.Format(
                        "MapToCacheItem method should be overrided or IObjectMapper should be implemented in order to map {0} to {1}",
                        typeof(TEntity),
                        typeof(TCacheItem)
                    )
                );
            }

            return ObjectMapper.Map<List<TCacheItem>>(entities);
        }
    }
}
