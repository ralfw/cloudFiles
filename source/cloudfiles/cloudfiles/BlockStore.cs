using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using cloudfiles.contract;

namespace cloudfiles
{
    internal class BlockUploadSummary
    {
        public Guid BlockGroupId;
        public int TotalNumberOfBytes;
        public int BlockSize;
    }


    internal class BlockStore
    {
        private const int DEFAULT_BLOCK_SIZE = 4000;

        private readonly IKeyValueStore _cache;
        private readonly int _blockSize;


        public BlockStore(IKeyValueStore cache) : this(cache, DEFAULT_BLOCK_SIZE) {}
        public BlockStore(IKeyValueStore cache, int blockSize)
        {
            _cache = cache;
            _blockSize = blockSize;
        }


        public void Store_blocks(Stream source)
        {
            var blockGroupId = Guid.NewGuid();
            var summary = new BlockUploadSummary {BlockGroupId = blockGroupId, BlockSize = _blockSize};
            Stream_blocks(source, block => 
                Store_block(summary, block));
        }

        internal void Stream_blocks(Stream source, Action<Tuple<byte[], int>> on_block)
        {
            var buffer = new byte[_blockSize];
            var blockIndex = 0;
            int nBytesRead;
            while ((nBytesRead = source.Read(buffer, 0, buffer.Length)) == buffer.Length)
                on_block(Create_block(buffer, nBytesRead, blockIndex++));
            if (nBytesRead > 0)
                on_block(Create_block(buffer, nBytesRead, blockIndex));
            on_block(new Tuple<byte[], int>(null, 0));
        }

        private Tuple<byte[],int> Create_block(byte[] buffer, int nBytesRead, int blockIndex)
        {
            var bufferCopy = new byte[nBytesRead];
            Array.Copy(buffer, bufferCopy, bufferCopy.Length);
            return new Tuple<byte[], int>(bufferCopy, blockIndex);
        }


        internal void Store_block(BlockUploadSummary summary, Tuple<byte[], int> block)
        {
            var blockKey = Build_block_key(summary.BlockGroupId, block.Item2);
            Upload_block(blockKey, block.Item1);
            Summarize_blocks(summary, block.Item1);
        }


        private string Build_block_key(Guid blockGroupId, int blockIndex)
        {
            return string.Format("{0}-{1}", blockIndex, blockGroupId);
        }


        internal void Upload_block(string blockKey, byte[] blockContent)
        {
            if (blockContent == null) return;

            var serialized_content = Convert.ToBase64String(blockContent);
            var signed_content = string.Format("{0}", serialized_content);
            _cache.Add(blockKey, signed_content);
        }


        internal void Summarize_blocks(BlockUploadSummary summary, byte[] blockContent)
        {
            if (blockContent != null)
                summary.TotalNumberOfBytes += blockContent.Length;
            else
                On_blocks_stored(summary);
        }


        public event Action<BlockUploadSummary> On_blocks_stored;
    }
}
