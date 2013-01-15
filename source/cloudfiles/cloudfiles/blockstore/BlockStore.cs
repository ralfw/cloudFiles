using System;
using System.IO;
using cloudfiles.contract;

namespace cloudfiles.blockstore
{
    internal class BlockStore
    {
        private const int DEFAULT_BLOCK_SIZE = 40*1024;

        private readonly BlockStore_upload_operations _upload;


        public BlockStore(IKeyValueStore cache) : this(cache, DEFAULT_BLOCK_SIZE) {}
        public BlockStore(IKeyValueStore cache, int blockSize)
        {
            _upload = new BlockStore_upload_operations(cache, blockSize);
            _upload.On_blocks_stored += _ => On_blocks_stored(_);
        }


        public void Store_blocks(Stream source)
        {
            var blockGroupId = Guid.NewGuid();
            var summary = new BlockUploadSummary {BlockGroupId = blockGroupId, BlockSize = _upload.BlockSize};
            _upload.Stream_blocks(source, block => 
                    Store_block(summary, block));
        }


        internal void Store_block(BlockUploadSummary summary, Tuple<byte[], int> block)
        {
            var blockKey = _upload.Build_block_key(summary.BlockGroupId, block.Item2);
            _upload.Upload_block(blockKey, block.Item1);
            _upload.Summarize_blocks(summary, block);
        }


        public event Action<BlockUploadSummary> On_blocks_stored;
    }
}
