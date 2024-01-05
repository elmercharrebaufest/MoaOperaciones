using SustitucionMOAWS.GoogleDrive.Models;
using SustitucionMOAWS.GoogleDrive.Interfaces;
using System.Configuration;

namespace SustitucionMOAWS.GoogleDrive
{
    public sealed class CampoSustentableGoogleDrive
    {
        private static readonly CampoSustentableGoogleDrive instance = new CampoSustentableGoogleDrive();
        private GoogleDriveHelper _GoogleDriveHelper { get; set; }

        public IGoogleDriveHelper Helper { get {
                return _GoogleDriveHelper;
            } }

        public string InputFolderId
        {
            get
            {
                return ConfigurationManager.AppSettings["DriveCampoSustentablesInputFolderId"];
            }
        }
        public string OutputFolderId
        {
            get
            {
                return ConfigurationManager.AppSettings["DriveCampoSustentablesOutputFolderId"];
            }
        }

        private CampoSustentableGoogleDrive() {
            _GoogleDriveHelper = new GoogleDriveHelper(
                    new GoogleDriveHelperGenerator(
                        applicationName: ConfigurationManager.AppSettings["DriveCampoSustentablesApplicationName"],
                        clientId: ConfigurationManager.AppSettings["DriveCampoSustentablesClientId"],
                        clientSecret: ConfigurationManager.AppSettings["DriveCampoSustentablesClientSecret"],
                        user: ConfigurationManager.AppSettings["DriveCampoSustentablesUsuario"]
                        ));
        }

        public static CampoSustentableGoogleDrive Instance
        {
            get
            {
                return instance;
            }
        }

    }
}
