using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using cloudfiles.blockstore;
using cloudfiles.filesystemcache;

namespace cloudfiles.tests
{
    [TestFixture]
    public class test_BlockStore_upload_operations
    {
        private const string CACHE_NAME = "testcache";
        private BlockStore_upload_operations _sut;

        [SetUp]
        public void Setup()
        {
            var cache = new FilesystemCache(CACHE_NAME);
            cache.Clear();
            _sut = new BlockStore_upload_operations(cache, 3);
        }


        [Test]
        public void Upload_a_single_block()
        {
            var content = new byte[] { 1, 2, 3 };

            _sut.Upload_block("myblockid", content);

            Assert.IsTrue(File.Exists(CACHE_NAME + @"\myblockid.txt"));
        }

        [Test]
        public void Summarize_blocks()
        {
            var summary = new BlockUploadSummary { BlockGroupId = Guid.NewGuid(), BlockSize = 3 };
            BlockUploadSummary result = null;

            _sut.Summarize_blocks(summary, new Tuple<byte[], int>(new byte[] { 1, 2, 3 }, 0), null);
            _sut.Summarize_blocks(summary, new Tuple<byte[], int>(new byte[] { 4, 5 }, 1), null);
            _sut.Summarize_blocks(summary, new Tuple<byte[], int>(null, 2), _ => result = _);

            Assert.AreSame(summary, result);
            Assert.AreEqual(summary.BlockGroupId, result.BlockGroupId);
            Assert.AreEqual(5, result.TotalNumberOfBytes);
            Assert.AreEqual(3, result.BlockSize);
            Assert.AreEqual(2, result.NumberOfBlocks);
        }

        [Test]
        public void Stream_blocks()
        {
            var result = new List<Tuple<byte[], int>>();
            var source = new MemoryStream(Encoding.ASCII.GetBytes("12345678"));

            _sut.Stream_blocks(source, result.Add);

            Assert.AreEqual(4, result.Count);
            Assert.That(result[0].Item1, Is.EqualTo(new byte[] { 49, 50, 51 }));
            Assert.AreEqual(0, result[0].Item2);
            Assert.That(result[1].Item1, Is.EqualTo(new byte[] { 52, 53, 54 }));
            Assert.AreEqual(1, result[1].Item2);
            Assert.That(result[2].Item1, Is.EqualTo(new byte[] { 55, 56 }));
            Assert.AreEqual(2, result[2].Item2);
            Assert.IsNull(result[3].Item1);
        }

        [Test]
        public void Store_head()
        {
            var id = Guid.NewGuid();

            _sut.Store_head(id, 42);

            Assert.AreEqual("42", File.ReadAllText(CACHE_NAME + @"\" + id + ".txt"));
        }
    }
}