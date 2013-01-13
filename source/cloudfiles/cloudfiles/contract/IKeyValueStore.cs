using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cloudfiles.contract
{
    public interface IKeyValueStore : IDisposable
    {
        void Add(string key, string value);
        void ReplaceOrAdd(string key, string value);
        int Increment(string key, int amount);

        string Get(string key);
        bool TryGet(string key, out string value);

        void Remove(string key);
        void Clear();
    }
}
