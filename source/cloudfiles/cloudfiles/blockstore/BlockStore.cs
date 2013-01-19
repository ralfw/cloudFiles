using System;
using System.IO;
using cloudfiles.contract;

namespace cloudfiles.blockstore
{
    internal class BlockStore
    {
        private const int DEFAULT_BLOCK_SIZE = 100*1024;

        private readonly BlockGroup_operations _groupOps;
        private readonly BlockGroupHead_operations _headOps;
        private readonly int _blockSize;


        public BlockStore(IKeyValueStore cache) : this(cache, DEFAULT_BLOCK_SIZE) {}
        public BlockStore(IKeyValueStore cache, int blockSize)
        {
            _groupOps = new BlockGroup_operations(cache, blockSize);
            _headOps = new BlockGroupHead_operations(cache);
            _blockSize = blockSize;
        }


        public void Store_blocks(Stream source)
        {
            var blockGroupId = Guid.NewGuid();
            var summary = new BlockUploadSummary {BlockGroupId = blockGroupId, BlockSize =  _blockSize};
            _groupOps.Stream_blocks(source, block0 => 
                    Store_block(blockGroupId, block0, block1 => 
                        summary.Aggregate(block1, _ =>
                        {
                            _headOps.Write_number_of_blocks(_.BlockGroupId, _.NumberOfBlocks);
                            On_blocks_stored(_);
                        })));
        }

        internal void Store_block(Guid blockGroupId, Tuple<byte[], int> block, Action<Tuple<byte[], int>> on_block)
        {
            var blockKey = blockGroupId.Build_block_key_for_index(block.Item2);
            _groupOps.Upload_block(blockKey, block.Item1);
            on_block(block);
        }



        public void Load_blocks(Guid blockGroupId, Stream destination)
        {
            var numberOfBlocks = _headOps.Read_number_of_blocks(blockGroupId);
            _groupOps.Stream_block_keys(blockGroupId, numberOfBlocks, blockKey =>
            {
                var blockContent = _groupOps.Download_block(blockKey);
                _groupOps.Write_content(blockContent, destination);
            });
        }


        public event Action<BlockUploadSummary> On_blocks_stored;
    }
}
