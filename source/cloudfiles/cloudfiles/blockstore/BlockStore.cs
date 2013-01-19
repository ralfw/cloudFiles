using System;
using System.IO;
using cloudfiles.contract;

namespace cloudfiles.blockstore
{
    internal class BlockStore
    {
        private const int DEFAULT_BLOCK_SIZE = 40*1024;

        private readonly BlockStore_upload_operations _upload;
        private readonly BlockStore_download_operations _download;


        public BlockStore(IKeyValueStore cache) : this(cache, DEFAULT_BLOCK_SIZE) {}
        public BlockStore(IKeyValueStore cache, int blockSize)
        {
            _upload = new BlockStore_upload_operations(cache, blockSize);
            _download = new BlockStore_download_operations(cache);
        }


        public void Store_blocks(Stream source)
        {
            var blockGroupId = Guid.NewGuid();
            var summary = new BlockUploadSummary {BlockGroupId = blockGroupId, BlockSize = _upload.BlockSize};
            _upload.Stream_blocks(source, block0 => 
                    Store_block(blockGroupId, block0, block1 => 
                        _upload.Summarize_blocks(summary, block1, summary1 =>
                        {
                            _upload.Store_head(summary1.BlockGroupId, summary1.NumberOfBlocks);
                            On_blocks_stored(summary1);
                        })));
        }


        internal void Store_block(Guid blockGroupId, Tuple<byte[], int> block, Action<Tuple<byte[], int>> on_block)
        {
            var blockKey = blockGroupId.Build_block_key_for_index(block.Item2);
            _upload.Upload_block(blockKey, block.Item1);
            on_block(block);
        }



        public void Load_blocks(Guid blockGroupId, Stream destination)
        {
            var numberOfBlocks = _download.Get_number_of_blocks(blockGroupId);
            _download.Stream_block_keys(blockGroupId, numberOfBlocks, blockKey =>
            {
                var blockContent = _download.Download_block(blockKey);
                _download.Write_content(blockContent, destination);
            });
        }


        public event Action<BlockUploadSummary> On_blocks_stored;
    }
}
