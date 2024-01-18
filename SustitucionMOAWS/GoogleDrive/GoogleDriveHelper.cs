using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using System;
using System.Collections.Generic;
using SustitucionMOAWS.GoogleDrive.Models;
using SustitucionMOAWS.GoogleDrive.Interfaces;

namespace SustitucionMOAWS.GoogleDrive
{
    public sealed class GoogleDriveHelper : IGoogleDriveHelper
    {
        private DriveService DriveService { get; set; }
        /// <summary>
        /// Setup the internal google DriveService class
        /// </summary>
        /// <param name="generator"></param>
        public void SetCredentials(GoogleDriveHelperGenerator generator)
        {
            var credential = CreateCredentials(generator);

            DriveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = generator.ApplicationName
            });
        }

        private UserCredential CreateCredentials(GoogleDriveHelperGenerator generator)
        {
            string[] scopes = { DriveService.Scope.Drive };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = generator.ClientId,
                    ClientSecret = generator.ClientSecret,
                },
                scopes,
                generator.User,
                System.Threading.CancellationToken.None).Result;

            return credential;
        }
        public string UploadFile(GoogleDriveFileUploadRequest uploadFileRequest)
        {
            // Ensure file path exists and is accessible
            if (!System.IO.File.Exists(uploadFileRequest.FilePath) && uploadFileRequest.Bytes is null)
            {
                throw new FileNotFoundException("File not found at the specified path or there wasn't bytes provided.");
            }

            // Get file metadata
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = uploadFileRequest.FileUploadName != null ? uploadFileRequest.FileUploadName : Path.GetFileName(uploadFileRequest.FilePath)
            };

            // With mimeType if provided
            if (!string.IsNullOrEmpty(uploadFileRequest.MimeType))
            {
                fileMetadata.MimeType = uploadFileRequest.MimeType;
            }
            if (!string.IsNullOrEmpty(uploadFileRequest.FolderId))
            {
                fileMetadata.Parents = new List<string> { uploadFileRequest.FolderId };
            }

            try
            {
                if(uploadFileRequest.Bytes is null) 
                {
                    using (var stream = new FileStream(uploadFileRequest.FilePath, FileMode.Open))
                    {
                        return UploadFile(fileMetadata, stream, uploadFileRequest);
                    }
                }
                using (var stream = new MemoryStream(uploadFileRequest.Bytes))
                {
                    return UploadFile(fileMetadata, stream, uploadFileRequest);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately, consider logging or retry mechanisms
                throw new Exception("File upload failed.", ex);
            }
        }
        private string UploadFile(Google.Apis.Drive.v3.Data.File fileMetadata, Stream stream, GoogleDriveFileUploadRequest uploadFileRequest)
        {
            var request = DriveService.Files.Create(fileMetadata, stream, uploadFileRequest.MimeType);
            request.Upload();
            var file = request.ResponseBody;
            return file.Id; // Return the file ID for reference
        }
        public string GetFolderIdByName(string folderName)
        {
            string folderId = null;
            var folderList = GetFiles("mimeType='application/vnd.google-apps.folder'");

            foreach (var folder in folderList)
            {
                if (folder.Name == folderName)
                {
                    folderId = folder.Id;
                    break;
                }
            }
            return folderId;
        }
        public IList<Google.Apis.Drive.v3.Data.File> GetFiles(string query = null, string fields = "nextPageToken, files(id, name)")
        {
            var files = DriveService.Files.List();
            files.Q = query;
            files.Fields = fields;

            var fileList = files.Execute().Files;

            return fileList;
        }
        public void DownloadFile(GoogleDriveFileDownloadRequest downloadFileRequest)
        {
            // Busca el archivo en la carpeta específica
            var query = $"name='{downloadFileRequest.FileName}'";
            if (downloadFileRequest.FolderId != null)
            {
                query = $"'{downloadFileRequest.FolderId}' in parents and {query}";
            }
            var files = GetFiles(query, "files(id)");

            if (files == null || files.Count == 0)
            {
                throw new FileNotFoundException("The desired file was not found on the drive");
            }

            // Obtiene la ID del archivo
            string fileId = files[0].Id;

            // Descarga el archivo
            var request = DriveService.Files.Get(fileId);
            using (var stream = new MemoryStream())
            {
                request.Download(stream);

                // Guarda el archivo descargado localmente
                using (var fileStream = new FileStream(downloadFileRequest.FilePath, FileMode.Create, FileAccess.Write))
                {
                    stream.WriteTo(fileStream);
                }
            }
        }
        public IList<Google.Apis.Drive.v3.Data.File> GetFolderFiles(string folderId)
        {
            return GetFiles($"parents = '{folderId}'");
        }
    }
}

