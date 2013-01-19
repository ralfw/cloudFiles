using System;

namespace cloudfiles.blockstore
{
    internal static class BlockStore_extensions
    {
        public static string Build_block_key_for_index(this Guid blockGroupId, int blockIndex)
        {
            return string.Format("{0}-{1}", blockIndex, blockGroupId);
        }
    }
}