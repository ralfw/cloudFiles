using System;
using System.IO;
using cloudfiles.contract;

namespace cloudfiles.blockstore
{
    internal class BlockStore_upload_operations
    {
        private readonly IKeyValueStore _cache;
        private readonly int _blockSize;


        public BlockStore_upload_operations(IKeyValueStore cache, int blockSize)
        {
            _cache = cache;
            _blockSize = blockSize;
        }


        public void Stream_blocks(Stream source, Action<Tuple<byte[], int>> on_block)
        {
            var buffer = new byte[_blockSize];
            var blockIndex = 0;
            int nBytesRead;
            while ((nBytesRead = source.Read(buffer, 0, buffer.Length)) == buffer.Length)
                on_block(Create_block(buffer, nBytesRead, blockIndex++));
            if (nBytesRead > 0)
                on_block(Create_block(buffer, nBytesRead, blockIndex++));
            on_block(new Tuple<byte[], int>(null, blockIndex));
        }


        private Tuple<byte[], int> Create_block(byte[] buffer, int nBytesRead, int blockIndex)
        {
            var bufferCopy = new byte[nBytesRead];
            Array.Copy(buffer, bufferCopy, bufferCopy.Length);
            return new Tuple<byte[], int>(bufferCopy, blockIndex);
        }


        public string Build_block_key(Guid blockGroupId, int blockIndex)
        {
            return string.Format("{0}-{1}", blockIndex, blockGroupId);
        }

        public void Upload_block(string blockKey, byte[] blockContent)
        {
            if (blockContent == null) return;

            var serialized_content = Convert.ToBase64String(blockContent);
            _cache.Add(blockKey, serialized_content);
        }


        public void Summarize_blocks(BlockUploadSummary summary, Tuple<byte[], int> block, Action<BlockUploadSummary> on_end_of_block_stream)
        {
            if (block.Item1 != null)
                summary.TotalNumberOfBytes += block.Item1.Length;
            else
            {
                summary.NumberOfBlocks = block.Item2;
                on_end_of_block_stream(summary);
            }
        }


        public void Store_head(Guid blockGroupId, int numberOfBlocks)
        {
            _cache.Add(blockGroupId.ToString(), numberOfBlocks.ToString());
        }


        public int BlockSize { get { return _blockSize; } }

    }
}