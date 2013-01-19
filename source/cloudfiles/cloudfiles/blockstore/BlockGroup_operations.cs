using System;
using System.IO;
using System.Linq;
using cloudfiles.contract;

namespace cloudfiles.blockstore
{
    internal class BlockGroup_operations
    {
        private readonly IKeyValueStore _cache;
        private readonly int _blockSize;
        private readonly byte[] _buffer;


        public BlockGroup_operations(IKeyValueStore cache, int blockSize)
        {
            _cache = cache;
            _blockSize = blockSize;
            _buffer = new byte[_blockSize];
        }


        public void Stream_blocks(Stream source, Action<Tuple<byte[], int>> on_block)
        {
            var blockIndex = 0;
            int nBytesRead;
            while ((nBytesRead = source.Read(_buffer, 0, _buffer.Length)) == _buffer.Length)
                on_block(Create_block(_buffer, nBytesRead, blockIndex++));
            if (nBytesRead > 0)
                on_block(Create_block(_buffer, nBytesRead, blockIndex++));
            on_block(new Tuple<byte[], int>(null, blockIndex));
        }

        private Tuple<byte[], int> Create_block(byte[] buffer, int nBytesRead, int blockIndex)
        {
            var bufferCopy = new byte[nBytesRead];
            Array.Copy(buffer, bufferCopy, bufferCopy.Length);
            return new Tuple<byte[], int>(bufferCopy, blockIndex);
        }
        
        
        public void Stream_block_keys(Guid blockGroupId, int numberOfBlocks, Action<string> on_blockKey)
        {
            Enumerable.Range(0, numberOfBlocks)
                      .ToList()
                      .ForEach(i => on_blockKey(blockGroupId.Build_block_key_for_index(i)));
            on_blockKey(null);
        }




        public void Upload_block(string blockKey, byte[] blockContent)
        {
            if (blockContent == null) return;

            var serialized_content = Convert.ToBase64String(blockContent);
            _cache.Add(blockKey, serialized_content);
        }

        public byte[] Download_block(string blockKey)
        {
            if (blockKey == null) return null;

            var serializedContent = _cache.Get(blockKey);
            return Convert.FromBase64String(serializedContent);
        }


        public void Write_content(byte[] blockContent, Stream destination)
        {
            if (blockContent == null) return;

            destination.Write(blockContent, 0, blockContent.Length);
        }
    }
}