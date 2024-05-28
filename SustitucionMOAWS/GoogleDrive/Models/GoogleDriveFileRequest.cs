namespace SustitucionMOAWS.GoogleDrive.Models
{
    public class GoogleDriveFileRequest
    {
        public string FolderId { get; set; }
        /// <summary>
        /// Local path. Where the files is located (Upload) or where the file will be saved (Download)
        /// </summary>
        public string FilePath { get; set; }

    }

    public class GoogleDriveFileDownloadRequest : GoogleDriveFileRequest
    {
        /// <summary>
        /// Name, in drive, for the wanted file to be downloaded.
        /// </summary>
        public string FileName { get; set; }
        
        public GoogleDriveFileDownloadRequest WithFileName(string fileName)
        {
            FileName = fileName;
            return this;
        }
        public GoogleDriveFileDownloadRequest WithFolderId(string folderId)
        {
            FolderId = folderId;
            return this;
        }
        public GoogleDriveFileDownloadRequest WithFilePath(string filePath)
        {
            FilePath = filePath;
            return this;
        }
    }
    public class GoogleDriveFileUploadRequest : GoogleDriveFileRequest
    {
        /// <summary>
        /// The name that the file will have in the drive. If null/not set, we use the name of the local file.
        /// </summary>
        public string FileUploadName { get; set; }
        public string MimeType { get; set; }
        public byte[] Bytes { get; set; }

        public GoogleDriveFileUploadRequest WithFileUploadName(string fileUploadName)
        {
            FileUploadName = fileUploadName;
            return this;
        }
        public GoogleDriveFileUploadRequest WithMimeType(string mimeType)
        {
            MimeType = mimeType;
            return this;
        }

        public GoogleDriveFileUploadRequest WithFolderId(string folderId)
        {
            FolderId = folderId;
            return this;
        }
        public GoogleDriveFileUploadRequest WithFilePath(string filePath)
        {
            FilePath = filePath;
            return this;
        }
        public GoogleDriveFileUploadRequest WithBytes(byte[] bytes)
        {
            Bytes = bytes;
            return this;
        }
    }
}
