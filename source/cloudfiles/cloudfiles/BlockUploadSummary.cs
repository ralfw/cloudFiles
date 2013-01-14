using System;

namespace cloudfiles
{
    internal class BlockUploadSummary
    {
        public Guid BlockGroupId;
        public int TotalNumberOfBytes;
        public int BlockSize;
        public int NumberOfBlocks;
    }
}