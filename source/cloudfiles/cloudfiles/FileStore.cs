using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cloudfiles.contract;

namespace cloudfiles
{
    public class FileStore
    {
        private readonly IKeyValueStore _keyValueStore;

        public FileStore(IKeyValueStore keyValueStore)
        {
            _keyValueStore = keyValueStore;
        }
    }
}
