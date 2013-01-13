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
            Directory.CreateDirectory(_cacheDirectoryPath);
        }


        public void Add(string key, string value)
        {
            var entry_filename = Build_entry_filename(key);
            if (File.Exists(entry_filename)) return;

            File.WriteAllText(entry_filename, value);
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

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            Directory.Delete(_cacheDirectoryPath, true);   
        }


        public void Dispose()
        {
            
        }



        private string Build_entry_filename(string key)
        {
            return Path.Combine(_cacheDirectoryPath, key) + ".txt";
        }
    }
}
