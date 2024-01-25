namespace SustitucionMOAWS.GoogleDrive.Models
{
    public class BaseGoogleDriveHelperGenerator
    {
        /// <summary>
        /// User with allowed access to the application
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Name of the project in Google Cloud
        /// </summary>
        public string ApplicationName { get; set; }

        public BaseGoogleDriveHelperGenerator(string applicationName, string user)
        {
            ApplicationName=applicationName;
            User=user;
        }
    }
    public sealed class GoogleDriveHelperGenerator: BaseGoogleDriveHelperGenerator
    {
        /// <summary>
        /// Credentials client ID, most commonly with OAuth
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Credentials client secret, related with the ClientId provided
        /// </summary>
        public string ClientSecret { get; set; }
        
        public GoogleDriveHelperGenerator(string applicationName, string clientId, string clientSecret, string user = null)
            : base(applicationName, user)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }

    public sealed class GoogleDriveHelperGeneratorWithService: BaseGoogleDriveHelperGenerator
    {
        /// <summary>
        /// Service account email of the like "your-service-account-email@your-project-id.iam.gserviceaccount.com"
        /// </summary>
        public string ServiceAccountEmail { get; set; }
        /// <summary>
        /// Local path to the service account key file (JSON or PM12).
        /// </summary>
        public string ServiceAccountKeyPath { get; set; }
        

        public GoogleDriveHelperGeneratorWithService(string applicationName, string serviceAccountEmail, string serviceAccountKeyPath, string user = null)
            :base(applicationName, user)
        {
            ServiceAccountEmail = serviceAccountEmail;
            ServiceAccountKeyPath = serviceAccountKeyPath;
        }
    }
}
