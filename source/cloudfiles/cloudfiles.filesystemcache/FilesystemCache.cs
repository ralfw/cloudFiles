using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using cloudfiles.contract;

namespace cloudfiles.filesystemcache
{
    public class FilesystemCache : IKeyValueStore
    {
        private readonly string _cacheDirectoryPath;

        public FilesystemCache(string cacheDirectoryPath)
        {
            _cacheDirectoryPath = cacheDirectoryPath;
            Clear();
        }


        public void Add(string key, string value)
        {
            var entry_filename = Build_entry_filename(key);
            if (File.Exists(entry_filename)) return;

            File.WriteAllText(entry_filename, value);
        }


        public void ReplaceOrAdd(string key, string value)
        {
            File.WriteAllText(Build_entry_filename(key), value);
        }


        public int Increment(string key, int amount)
        {
            throw new NotImplementedException();
        }

        public string Get(string key)
        {
            var entry_filename = Build_entry_filename(key);
            if (File.Exists(entry_filename))
                return File.ReadAllText(entry_filename);
            throw new KeyValueStoreException(string.Format("No entry found in cache for key: {0}", key));
        }

        public bool TryGet(string key, out string value)
        {
            value = string.Empty;
            try
            {
                value = Get(key);
                return true;
            }
            catch(KeyValueStoreException)
            {
                return false;
            }
        }


        public void Remove(string key)
        {
            File.Delete(Build_entry_filename(key));
        }

        public void Clear()
        {
            if (Directory.Exists(_cacheDirectoryPath)) Directory.Delete(_cacheDirectoryPath, true);
            Directory.CreateDirectory(_cacheDirectoryPath);
        }


        public void Dispose() {}


        private string Build_entry_filename(string key)
        {
            return Path.Combine(_cacheDirectoryPath, key) + ".txt";
        }
    }
}
