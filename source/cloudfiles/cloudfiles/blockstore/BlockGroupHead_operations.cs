using System;
using cloudfiles.contract;

namespace cloudfiles.blockstore
{
    internal class BlockGroupHead_operations
    {
        private readonly IKeyValueStore _cache;

        public BlockGroupHead_operations(IKeyValueStore cache)
        {
            _cache = cache;
        }


        public void Write_number_of_blocks(Guid blockGroupId, int numberOfBlocks)
        {
            _cache.Add(blockGroupId.ToString(), numberOfBlocks.ToString());      
        }

        public int Read_number_of_blocks(Guid blockGroupId)
        {
            return int.Parse(_cache.Get(blockGroupId.ToString()));
        }
    }
}