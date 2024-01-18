using SustitucionMOAWS.GoogleDrive.Interfaces;
using SustitucionMOAWS.GoogleDrive.Models;
using System.Collections.Generic;
using System.Configuration;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using System.IO;
using System;

namespace SustitucionMOAWS.GoogleDrive
{
    public sealed class CampoSustentableGoogleDrive : ICampoSustentableGoogleDrive
    {

        private string ApplicationName => ConfigurationManager.AppSettings["DriveCampoSustentablesApplicationName"];
        private string ServiceAccountEmail => ConfigurationManager.AppSettings["DriveCampoSustentablesServiceAccountEmail"];
        private string ServiceAccountKey => ConfigurationManager.AppSettings["DriveCampoSustentablesServiceAccountKey"];
        private string User => ConfigurationManager.AppSettings["DriveCampoSustentablesUsuario"];

        private readonly IGoogleDriveHelper Helper;

        private string InputFolderId => ConfigurationManager.AppSettings["DriveCampoSustentablesInputFolderId"];

        private string OutputFolderId => ConfigurationManager.AppSettings["DriveCampoSustentablesOutputFolderId"];


        public CampoSustentableGoogleDrive(IGoogleDriveHelper helper)
        {
            Helper = helper;
            InitGoogleDrive();
        }

        private void InitGoogleDrive()
        {
            var serviceAccountKeyPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Keys\\{ServiceAccountKey}";
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

        public string UploadFile(GoogleDriveFileUploadRequest uploadFileRequest)
        {
            if(uploadFileRequest.FolderId is null)
            {
                uploadFileRequest.WithFolderId(InputFolderId);
            }
            return Helper.UploadFile(uploadFileRequest);
        }

    }
}
