using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using cloudfiles.contract;
using io.iron.ironcache;

namespace cloudfiles.ironiocache
{
    public class IronIOCache : IKeyValueStore
    {
        private readonly string _cacheName;
        private readonly IronCache _cache;

        public IronIOCache(string cacheName, IronIOCredentials ironIoCredentials)
        {
            _cacheName = cacheName;
            _cache = new IronCache(ironIoCredentials.ProjectId, ironIoCredentials.Token);
        }

        public void Add(string key, string value)
        {
            _cache.Add(_cacheName, key, value);
        }

        public void ReplaceOrAdd(string key, string value)
        {
            try
            {
                _cache.Replace(_cacheName, key, value);
            }
            catch(HttpException)
            {
                Add(key, value);
            }
        }

        public int Increment(string key, int amount)
        {
            return _cache.Increment(_cacheName, key, amount);
        }

        public string Get(string key)
        {
            var value = _cache.Get<string>(_cacheName, key);
            if (value != null) return value;
            throw new KeyValueStoreException(string.Format("No entry found for key {0}", key));
        }

        public bool TryGet(string key, out string value)
        {
            value = _cache.Get<string>(_cacheName, key);
            return value != null;
        }

        public void Remove(string key)
        {
            try
            {
                _cache.Remove(_cacheName, key);
            }
            catch(HttpException) {}
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Dispose() {}
    }
}
