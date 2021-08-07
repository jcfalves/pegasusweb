using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CacheStrategy.Stores.Caching
{
    public class BusinessUnitCodeCachingStore<T> : IBusinessUnitCodeStore
        where T : IBusinessUnitCodeStore
    {
        private readonly IMemoryCache _memoryCache;
        private readonly T _inner;
        private readonly ILogger<BusinessUnitCodeCachingStore<T>> _logger;

        public BusinessUnitCodeCachingStore(IMemoryCache memoryCache, T inner, ILogger<BusinessUnitCodeCachingStore<T>> logger)
        {
            _memoryCache = memoryCache;
            _inner = inner;
            _logger = logger;
        }


        public List<string> ListBUC09()
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC09", out lista))
            {
            }
            return lista;
        }


        public List<string> ListaBUC09(List<string> buc)
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC09", out lista))
            {
                var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                              TimeSpan.FromSeconds(3600));

                _memoryCache.Set("BUC09", buc, options);

                lista = buc;
            }
            return lista;
        }

        public List<string> ListaBUC15(List<string> buc)
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC15", out lista))
            {
                var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                              TimeSpan.FromSeconds(3600));

                _memoryCache.Set("BUC15", buc, options);

                lista = buc;
            }
            return lista;
        }

        public List<string> ListaBUC17(List<string> buc)
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC17", out lista))
            {
                var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                              TimeSpan.FromSeconds(3600));

                _memoryCache.Set("BUC17", buc, options);

                lista = buc;
            }
            return lista;
        }

        public List<string> ListaBUC80(List<string> buc)
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC80", out lista))
            {
                var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                              TimeSpan.FromSeconds(3600));

                _memoryCache.Set("BUC80", buc, options);

                lista = buc;
            }
            return lista;
        }

        //------------------------------------------------------------------------
        //<PBI-1255>
        public List<string> ListaBUC27(List<string> buc)
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC27", out lista))
            {
                var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(
                              TimeSpan.FromSeconds(3600));

                _memoryCache.Set("BUC27", buc, options);

                lista = buc;
            }
            return lista;
        }
        //------------------------------------------------------------------------

        public void RemoveBUC09()
        {
            _memoryCache.Remove("BUC09");
        }

        public void RemoveBUC15()
        {
            _memoryCache.Remove("BUC15");
        }

        public void RemoveBUC17()
        {
            _memoryCache.Remove("BUC17");
        }

        public void RemoveBUC80()
        {
            _memoryCache.Remove("BUC80");
        }

        public List<string> ListBUC15()
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC15", out lista))
            {
            }
            return lista;
        }

        public List<string> ListBUC17()
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC17", out lista))
            {
            }
            return lista;
        }

        public List<string> ListBUC80()
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC80", out lista))
            {
            }
            return lista;
        }

        //------------------------------------------------------------------------
        //<PBI-1255>
        public List<string> ListBUC27()
        {
            List<string> lista = new List<string>();

            if (!_memoryCache.TryGetValue("BUC27", out lista))
            {
            }
            return lista;
        }
        //------------------------------------------------------------------------
    }
}
