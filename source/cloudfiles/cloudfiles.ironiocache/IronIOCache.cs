using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cloudfiles.contract;

namespace cloudfiles.ironiocache
{
    public class IronIOCache : IKeyValueStore
    {
        public IronIOCache(IronIOCredentials ironIoCredentials)
        {
            
        }

        public void Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void ReplaceOrAdd(string key, string value)
        {
            throw new NotImplementedException();
        }

        public int Increment(string key, int amount)
        {
            throw new NotImplementedException();
        }

        public string Get(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(string key, out string value)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
