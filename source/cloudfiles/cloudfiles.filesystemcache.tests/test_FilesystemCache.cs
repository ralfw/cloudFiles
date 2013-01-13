using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace cloudfiles.filesystemcache.tests
{
    [TestFixture]
    public class test_FilesystemCache
    {
        const string CACHE_PATH = "testcache";


        [Test]
        public void Creating_cache_creates_a_directory()
        {
            if (Directory.Exists(CACHE_PATH)) Directory.Delete(CACHE_PATH, true);

            var sut = new FilesystemCache(CACHE_PATH);

            Assert.IsTrue(Directory.Exists(CACHE_PATH));
        }


        [Test]
        public void Adding_key_creates_file_and_stores_value()
        {
            const string entry_filename = CACHE_PATH + @"\mykey.txt";
            File.Delete(entry_filename);
            var sut = new FilesystemCache(CACHE_PATH);

            sut.Add("mykey", "hello");

            Assert.AreEqual("hello", File.ReadAllText(entry_filename));
        }


        [Test]
        public void Adding_existing_key_does_not_overwrite_value()
        {
            const string entry_filename = CACHE_PATH + @"\mykey.txt";
            File.Delete(entry_filename);
            var sut = new FilesystemCache(CACHE_PATH);

            sut.Add("mykey", "hello");
            sut.Add("mykey", "world");

            Assert.AreEqual("hello", File.ReadAllText(entry_filename));   
        }


        [Test]
        public void Remove_a_key()
        {
            const string entry_filename = CACHE_PATH + @"\mykey.txt";
            var sut = new FilesystemCache(CACHE_PATH);
            sut.Add("mykey", "to be deleted");

            sut.Remove("mykey");

            Assert.IsFalse(File.Exists(entry_filename));
        }


        [Test]
        public void Remoing_a_non_existing_key_has_no_effect()
        {
            var sut = new FilesystemCache(CACHE_PATH);

            sut.Remove("non existent key");
        }


        [Test]
        public void Replace_an_existing_value()
        {
            const string entry_filename = CACHE_PATH + @"\mykey.txt";
            var sut = new FilesystemCache(CACHE_PATH);
            sut.Add("mykey", "will be overwritten");

            sut.ReplaceOrAdd("mykey", "hello");

            Assert.AreEqual("hello", File.ReadAllText(entry_filename));
        }


        [Test]
        public void Replace_non_existent_key_creates_it()
        {
            const string entry_filename = CACHE_PATH + @"\mykey.txt";
            File.Delete(entry_filename);
            var sut = new FilesystemCache(CACHE_PATH);

            sut.ReplaceOrAdd("mykey", "hello");

            Assert.AreEqual("hello", File.ReadAllText(entry_filename));
        }


        [Test]
        public void Clear_removes_all_entries()
        {
            var sut = new FilesystemCache(CACHE_PATH);
            sut.Add("key1", "1");
            sut.Add("key2", "2");

            sut.Clear();

            Assert.AreEqual(0, Directory.GetFiles(CACHE_PATH).Length);
        }
    }
}
