using Microsoft.Extensions.Caching.Memory;

namespace CurrencyRatesApi.Services.Utils.MemoryCacheManager
{
    /// <summary>
    /// Custom memory cache which implements the <see cref="IMemoryCache"/>
    /// </summary>
    public class MemoryCacheManager : IMemoryCache
    {
        private IMemoryCache memoryCache;
        
        /// <summary>
        /// Initializes the Custom memory cache
        /// </summary>
        public MemoryCacheManager()
        {
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Creates new cache
        /// </summary>
        /// <param name="key">The key for cache</param>
        /// <returns></returns>
        public ICacheEntry CreateEntry(object key)
        {
            return this.memoryCache.CreateEntry(key);
        }

        /// <summary>
        /// Disposes the memory cache
        /// </summary>
        public void Dispose()
        {
            this.memoryCache.Dispose();
        }

        /// <summary>
        /// Removes the cache
        /// </summary>
        /// <param name="key">the key for the cache</param>
        public void Remove(object key)
        {
            this.memoryCache.Remove(key);
        }

        /// <summary>
        /// Gets cache if exists 
        /// </summary>
        /// <param name="key">The key for the cache</param>
        /// <param name="value">The value of the cache</param>
        /// <returns></returns>
        public bool TryGetValue(object key, out object value)
        {
            return this.memoryCache.TryGetValue(key, out value);
        }
    }
}
