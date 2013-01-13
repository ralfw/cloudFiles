using System;

namespace cloudfiles.contract
{
    public interface IFileMetadata
    {
        Guid Id { get; }

        string Filename { get; }
        int FileSize { get; }
        int BlockSize { get; }

        DateTime CreatedAt { get; }
        string CreatedBy { get; }
        DateTime LastChangedAt { get; }
        string LastChangedBy { get; }

        int Version { get; }
    }
}