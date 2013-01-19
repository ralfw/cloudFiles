using System;

namespace cloudfiles.blockstore
{
    internal class BlockUploadSummary
    {
        public Guid BlockGroupId;
        public int TotalNumberOfBytes;
        public int BlockSize;
        public int NumberOfBlocks;


        public void Aggregate(Tuple<byte[], int> block, Action<BlockUploadSummary> on_end_of_block_stream)
        {
            if (block.Item1 != null)
                TotalNumberOfBytes += block.Item1.Length;
            else
            {
                NumberOfBlocks = block.Item2;
                on_end_of_block_stream(this);
            }
        }
    }
}