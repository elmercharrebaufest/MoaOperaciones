using SustitucionMOAWS.GoogleDrive.Interfaces;
using SustitucionMOAWS.GoogleDrive.Models;
using System.Collections.Generic;
using System.Configuration;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using System.IO;
using System;
using System.Threading.Tasks;

namespace SustitucionMOAWS.GoogleDrive
{
    public sealed class CampoSustentableGoogleDrive : ICampoSustentableGoogleDrive
    {

        private string ApplicationName => ConfigurationManager.AppSettings["DriveCampoSustentablesApplicationName"];
        private string ServiceAccountEmail => ConfigurationManager.AppSettings["DriveCampoSustentablesServiceAccountEmail"];
        private string ServiceAccountKey => ConfigurationManager.AppSettings["DriveCampoSustentablesServiceAccountKey"];
        private string User => ConfigurationManager.AppSettings["DriveCampoSustentablesUsuario"];

        private readonly IGoogleDriveHelper Helper;

        private string OutputFolderId => ConfigurationManager.AppSettings["DriveCampoSustentablesOutputFolderId"];


        public CampoSustentableGoogleDrive(IGoogleDriveHelper helper)
        {
            Helper = helper;
            InitGoogleDrive();
        }

        private void InitGoogleDrive()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var serviceAccountKeyPath = File.Exists(ServiceAccountKey)? ServiceAccountKey : Path.Combine(currentDir, ServiceAccountKey); 
            Helper.SetCredentials(new GoogleDriveHelperGeneratorWithService(
                applicationName: ApplicationName,
                serviceAccountEmail: ServiceAccountEmail,
                serviceAccountKeyPath: serviceAccountKeyPath,
                user: User
                ));
        }

        public void DownloadFile(GoogleDriveFileDownloadRequest downloadFileRequest)
        {
            if(downloadFileRequest.FolderId is null)
            {
                downloadFileRequest.WithFolderId(OutputFolderId);
            }
            
            Helper.DownloadFile(downloadFileRequest);
        }

        public IList<GoogleFile> GetFiles(string query = null, string fields = "nextPageToken, files(id, name)")
        {
            return Helper.GetFiles(query, fields);
        }

        public IList<GoogleFile> GetFolderFiles(string folderId)
        {
            return Helper.GetFolderFiles(folderId);
        }

        public string GetFolderIdByName(string folderName)
        {
            return Helper.GetFolderIdByName(folderName);
        }

        public string UploadFile(GoogleDriveFileUploadRequest uploadFileRequest, string inputFolderId)
        {
            if (uploadFileRequest.FolderId is null)
            {
                uploadFileRequest.WithFolderId(inputFolderId);
            }
            return Helper.UploadFile(uploadFileRequest);
        }

        public Task<T> DownloadFileAs<T>(GoogleDriveFileDownloadRequest downloadFileRequest)
        {
            if (downloadFileRequest.FolderId is null)
            {
                downloadFileRequest.WithFolderId(OutputFolderId);
            }
            return Helper.DownloadFileAs<T>(downloadFileRequest);
        }

        MemoryStream IGoogleDriveHelperUtils.DownloadFile(GoogleDriveFileDownloadRequest downloadFileRequest)
        {
            return Helper.DownloadFile(downloadFileRequest);
        }

        void IGoogleDriveHelperUtils.SaveFile(MemoryStream stream, GoogleDriveFileDownloadRequest downloadFileRequest)
        {
            Helper.SaveFile(stream,downloadFileRequest);
        }

        public string UploadFile(GoogleDriveFileUploadRequest uploadFileRequest)
        {
            throw new NotImplementedException();
        }
    }
}
