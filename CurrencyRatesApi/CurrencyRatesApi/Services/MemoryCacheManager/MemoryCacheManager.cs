using Microsoft.Extensions.Caching.Memory;

namespace CurrencyRatesApi.Services.MemoryCacheManager
{
    public class MemoryCacheManager : IMemoryCache
    {
        private IMemoryCache memoryCache;
        public MemoryCacheManager()
        {
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
        }
        public ICacheEntry CreateEntry(object key)
        {
            return this.memoryCache.CreateEntry(key);
        }

        public void Dispose()
        {
            this.memoryCache.Dispose();
        }

        public void Remove(object key)
        {
            this.memoryCache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return this.memoryCache.TryGetValue(key, out value);
        }
    }
}
