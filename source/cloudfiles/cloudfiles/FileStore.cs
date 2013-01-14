using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using cloudfiles.contract;

namespace cloudfiles
{
    struct UploadTask
    {
        public Guid FileId;
        public Guid VersionId;
    }

    struct UploadBlock
    {
        
    }

    public class FileStore : IFileStore
    {
        private readonly IKeyValueStore _keyValueStore;
        private readonly string _user;


        public FileStore(IKeyValueStore keyValueStore) : this(keyValueStore, Environment.MachineName) {}
        public  FileStore(IKeyValueStore keyValueStore, string user)
        {
            _keyValueStore = keyValueStore;
            _user = user;
        }


        public IFileMetadata Upload(string sourceFilename, Guid fileId)
        {
            throw new NotImplementedException();
        }

        public IFileMetadata Upload(Stream source, Guid fileId)
        {
            //var uploadTask = new UploadTask
            //                     {
            //                         FileId = fileId,
            //                     };
            //uploadTask.VersionId = Create_version_id();
            //Stream_blocks(uploadTask, _ => Store_block(_));
            //Create_new_file_version(uploadTask);
            //return Store_version_info(uploadTask);
            throw new NotImplementedException();
        }

        static Guid Create_version_id()
        {
            return Guid.NewGuid();
        }

        static void Stream_blocks(UploadTask uploadTask, Action<UploadBlock> onBlock)
        {
            
        }



        public IFileMetadata Download(Guid fileId, string destinationFilename)
        {
            throw new NotImplementedException();
        }

        public IFileMetadata Download(Guid fileId, Stream destination)
        {
            throw new NotImplementedException();
        }

        public IFileMetadata Download(Guid fileId, string destinationFilename, int version)
        {
            throw new NotImplementedException();
        }

        public IFileMetadata Download(Guid fileId, Stream destination, int version)
        {
            throw new NotImplementedException();
        }

        public IFileMetadata Delete(Guid fileId)
        {
            throw new NotImplementedException();
        }

        public IFileMetadata GetInfo(Guid fileId)
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            _keyValueStore.Dispose();
        }
    }
}
