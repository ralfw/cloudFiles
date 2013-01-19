using System;
using System.IO;
using System.Linq;
using cloudfiles.contract;

namespace cloudfiles.blockstore
{
    internal class BlockStore_download_operations
    {
        private readonly IKeyValueStore _cache;

        public BlockStore_download_operations(IKeyValueStore cache)
        {
            _cache = cache;
        }


        public int Get_number_of_blocks(Guid blockGroupId)
        {
            return int.Parse(_cache.Get(blockGroupId.ToString()));
        }

        public void Stream_block_keys(Guid blockGroupId, int numberOfBlocks, Action<string> on_blockKey)
        {
            Enumerable.Range(0, numberOfBlocks)
                      .ToList()
                      .ForEach(i => on_blockKey(blockGroupId.Build_block_key_for_index(i)));
            on_blockKey(null);
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