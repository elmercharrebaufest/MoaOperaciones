using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using System;
using System.Collections.Generic;
using SustitucionMOAWS.GoogleDrive.Models;
using SustitucionMOAWS.GoogleDrive.Interfaces;
using Google.Apis.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace SustitucionMOAWS.GoogleDrive
{
    public sealed class GoogleDriveHelper : IGoogleDriveHelper
    {
        private DriveService DriveService { get; set; }

        public void SetCredentials(GoogleDriveHelperGenerator generator)
        {
            var credential = CreateCredentials(generator);

            SetDriveService(credential, generator);
        }
        public void SetCredentials(GoogleDriveHelperGeneratorWithService generator)
        {
            var credential = CreateCredentials(generator);

            SetDriveService(credential, generator);
        }

        private void SetDriveService(IConfigurableHttpClientInitializer credential, BaseGoogleDriveHelperGenerator generator)
        {
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
        private GoogleCredential CreateCredentials(GoogleDriveHelperGeneratorWithService generator)
        {
            string[] scopes = { DriveService.Scope.Drive };


            using (var stream = new FileStream(generator.ServiceAccountKeyPath, FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream)
                    .CreateScoped(scopes);
            }
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
                if (uploadFileRequest.Bytes is null)
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
        public MemoryStream DownloadFile(GoogleDriveFileDownloadRequest downloadFileRequest)
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
            var stream = new MemoryStream();
            request.Download(stream);

            if (stream.Length == 0)
            {
                throw new ArgumentNullException("Stream when fetching google drive file cannot be null or empty.");
            }

            // Guarda el archivo descargado localmente si la propiedad esta seteada
            if (!string.IsNullOrEmpty(downloadFileRequest.FilePath))
            {
                SaveFile(stream, downloadFileRequest);
            }
            return stream;
        }
        public void SaveFile(MemoryStream stream, GoogleDriveFileDownloadRequest downloadFileRequest)
        {
            using (var fileStream = new FileStream(downloadFileRequest.FilePath, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(fileStream);
            }
        }
        public async Task<T>  DownloadFileAs<T>(GoogleDriveFileDownloadRequest downloadFileRequest)
        {
            var streamDownload = DownloadFile(downloadFileRequest);

            streamDownload.Seek(0, SeekOrigin.Begin);

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy() // or PascalCaseNamingStrategy()
                }
            };
            using (var reader = new StreamReader(streamDownload))
            {
                string json = await reader.ReadToEndAsync();
                using (var jsonReader = new JsonTextReader(new StringReader(json)))
                {
                    var serializer = JsonSerializer.Create(settings);
                    return serializer.Deserialize<T>(jsonReader);
                }
            }
        }
        public IList<Google.Apis.Drive.v3.Data.File> GetFolderFiles(string folderId)
        {
            return GetFiles($"parents = '{folderId}'");
        }
    }
}

