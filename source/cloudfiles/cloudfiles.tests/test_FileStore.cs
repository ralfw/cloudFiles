using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using cloudfiles.filesystemcache;

namespace cloudfiles.tests
{
    [TestFixture]
    public class test_FileStore
    {
        private FileStore _sut;

        [SetUp]
        public void Setup()
        {
            var cache = new FilesystemCache("test");
            cache.Clear();
            _sut = new FileStore(cache);
        }
    }
}
