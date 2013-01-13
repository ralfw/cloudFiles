using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cloudfiles.contract
{
    public interface IFileStore : IDisposable
    {
        IFileMetadata Upload(string sourceFilename, Guid fileId);
        IFileMetadata Upload(Stream source, Guid fileId);

        IFileMetadata Download(Guid fileId, string destinationFilename);
        IFileMetadata Download(Guid fileId, Stream destination);
        IFileMetadata Download(Guid fileId, string destinationFilename, int version);
        IFileMetadata Download(Guid fileId, Stream destination, int version);

        IFileMetadata Delete(Guid fileId);

        IFileMetadata GetInfo(Guid fileId);
    }
}
