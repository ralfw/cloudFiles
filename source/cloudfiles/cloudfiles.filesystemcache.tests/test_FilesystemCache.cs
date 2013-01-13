﻿using System;
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

            var sut = new FilesystemCache(CACHE_PATH);
            File.Delete(entry_filename);

            sut.Add("mykey", "hello");

            Assert.AreEqual("hello", File.ReadAllText(entry_filename));
        }


        [Test]
        public void Adding_existing_key_does_not_overwrite_value()
        {
            const string entry_filename = CACHE_PATH + @"\mykey.txt";

            var sut = new FilesystemCache(CACHE_PATH);
            File.Delete(entry_filename);

            sut.Add("mykey", "hello");
            sut.Add("mykey", "world");

            Assert.AreEqual("hello", File.ReadAllText(entry_filename));   
        }
    }
}
