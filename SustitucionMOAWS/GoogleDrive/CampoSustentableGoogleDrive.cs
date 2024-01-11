using Google.Apis.Drive.v3.Data;
using SustitucionMOAWS.GoogleDrive.Interfaces;
using SustitucionMOAWS.GoogleDrive.Models;
using System.Collections.Generic;
using System.Configuration;

namespace SustitucionMOAWS.GoogleDrive
{
    public sealed class CampoSustentableGoogleDrive : ICampoSustentableGoogleDrive
    {

        private string ApplicationName => ConfigurationManager.AppSettings["DriveCampoSustentablesApplicationName"];
        private string ClientId => ConfigurationManager.AppSettings["DriveCampoSustentablesClientId"];
        private string ClientSecret => ConfigurationManager.AppSettings["DriveCampoSustentablesClientSecret"];
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
            Helper.SetCredentials(new GoogleDriveHelperGenerator(
                applicationName: ApplicationName,
                clientId: ClientId,
                clientSecret: ClientSecret,
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

        public IList<File> GetFiles(string query = null, string fields = "nextPageToken, files(id, name)")
        {
            return Helper.GetFiles(query, fields);
        }

        public IList<File> GetFolderFiles(string folderId)
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
