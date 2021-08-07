using Bayer.Pegasus.Entities.Api;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CacheStrategy.Stores.Caching
{
    public class LoginCachingStore<T> : ILoginStore
        where T : ILoginStore
    {

        private readonly IMemoryCache _memoryCache;
        private readonly T _inner;
        private readonly ILogger<LoginCachingStore<T>> _logger;

        public LoginCachingStore(IMemoryCache memoryCache, T inner, ILogger<LoginCachingStore<T>> logger)
        {
            _memoryCache = memoryCache;
            _inner = inner;
            _logger = logger;
        }
        public IEnumerable<ReturnModel> List()
        {
            var key = "Login";
            var item = _memoryCache.Get<IEnumerable<ReturnModel>>(key);

            if (item == null)
            {
                item = _inner.List();
                if (item != null)
                {
                   _memoryCache.Set(key, item);
                }
            }
            return item;
        }
        
        private static string GetKey(string key)
        {
            return $"{typeof(T).FullName}:{key}";
        }

        public ReturnModel Get(ReturnModel lg)
        {
            try
            {
                ReturnModel logVModel = new ReturnModel();

                if (!_memoryCache.TryGetValue("LVM", out logVModel))
                {
                    _memoryCache.Set("LVM", lg);

                    logVModel = lg;
                }
                return logVModel;
            }
            catch (Exception e)
            {
                var msg = e.Message;
                return null;
            }
        }

        public void Remove()
        {
            _memoryCache.Remove("LVM");
        }
    }
}
