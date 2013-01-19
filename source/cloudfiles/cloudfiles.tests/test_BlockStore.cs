using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using cloudfiles.blockstore;
using cloudfiles.contract;
using cloudfiles.filesystemcache;
using cloudfiles.ironiocache;

namespace cloudfiles.tests
{
    [TestFixture]
    public class test_BlockStore
    {
        private const string CACHE_NAME = "testcache";
        private BlockStore _sut;

        [SetUp]
        public void Setup()
        {
            var cache = new FilesystemCache(CACHE_NAME);
            cache.Clear();
            _sut = new BlockStore(cache, 3);
        }


        [Test]
        public void Store_block()
        {
            Guid id = Guid.NewGuid();
            Tuple<byte[],int> result = null;

            var block = new Tuple<byte[], int>(new byte[] {1, 2, 3}, 0);
            _sut.Store_block(id, block, _ => result = _);

            Assert.AreEqual(1, Directory.GetFiles(CACHE_NAME).Length);
            Assert.AreEqual(block, result);
        }


        [Test]
        public void Store_blocks()
        {
            var source = new MemoryStream(Encoding.ASCII.GetBytes("1234567890"));
            BlockUploadSummary result = null;
            _sut.On_blocks_stored += _ => result = _;

            _sut.Store_blocks(source);

            Assert.AreEqual(5, Directory.GetFiles(CACHE_NAME).Length);
            Assert.AreNotEqual(Guid.Empty, result.BlockGroupId);
            Assert.AreEqual(10, result.TotalNumberOfBytes);
            Assert.AreEqual(3, result.BlockSize);
            Assert.AreEqual(4, result.NumberOfBlocks);
        }


        [Test]
        public void Load_blocks()
        {
            var source = new MemoryStream(Encoding.ASCII.GetBytes("the quick brown fox"));

            var id = Guid.Empty;
            _sut.On_blocks_stored += _ =>
            {
                id = _.BlockGroupId;
                Console.WriteLine("bytes stored: {0}", _.TotalNumberOfBytes);
            };
            _sut.Store_blocks(source);

            var result = new MemoryStream();
            _sut.Load_blocks(id, result);

            var content = Encoding.ASCII.GetString(result.ToArray());
            Assert.AreEqual("the quick brown fox", content);
        }


        [Test, Explicit]
        public void Real_files_to_disc()
        {
            Console.WriteLine("disk cache");
            var cache = new FilesystemCache(CACHE_NAME);
            cache.Clear();
            Real_files(cache);
        }

        [Test, Explicit]
        public void Real_files_to_cloud()
        {
            Console.WriteLine("cloud cache");
            var cache = new IronIOCache(CACHE_NAME, IronIOCredentials.LoadFrom(@"..\..\..\..\..\unversioned\ironcache credentials.txt"));
            cache.Clear();
            Real_files(cache);
        }

        private void Real_files(IKeyValueStore cache)
        {
            var start = DateTime.Now;

            _sut = new BlockStore(cache);

            var results = new List<BlockUploadSummary>();
            _sut.On_blocks_stored += _ =>
            {
                results.Add(_);
                Console.WriteLine("stored: {0} bytes to {1} - {2}", _.TotalNumberOfBytes, _.BlockGroupId, DateTime.Now.Subtract(start));
            };

            using (var fs = new FileStream(@"files\doc.pdf", FileMode.Open)) { _sut.Store_blocks(fs); }
            using (var fs = new FileStream(@"files\music.mp3", FileMode.Open)) { _sut.Store_blocks(fs); }

            using (var fs = new FileStream(@"docrestored.pdf", FileMode.Create))
            {
                _sut.Load_blocks(results[0].BlockGroupId, fs);
                Assert.AreEqual(new FileInfo(@"files\doc.pdf").Length, new FileInfo("docrestored.pdf").Length);
                Console.WriteLine("doc restored - {0}", DateTime.Now.Subtract(start));
            }
            using (var fs = new FileStream(@"musicrestored.mp3", FileMode.Create))
            {
                _sut.Load_blocks(results[1].BlockGroupId, fs);
                Assert.AreEqual(new FileInfo(@"files\music.mp3").Length, new FileInfo("musicrestored.mp3").Length);
                Console.WriteLine("music restored - {0}", DateTime.Now.Subtract(start));
            }
        }
    }
}
