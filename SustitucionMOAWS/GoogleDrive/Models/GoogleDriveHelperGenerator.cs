namespace SustitucionMOAWS.GoogleDrive.Models
{
    public class GoogleDriveHelperGenerator
    {
        /// <summary>
        /// Name of the project in Google Cloud
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Credentials client ID, most commonly with OAuth
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Credentials client secret, related with the ClientId provided
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// User with allowed access to eh application
        /// </summary>
        public string User { get; set; }

        public GoogleDriveHelperGenerator(string applicationName, string clientId, string clientSecret, string user = null)
        {
            ApplicationName = applicationName;
            ClientId = clientId;
            ClientSecret = clientSecret;
            User = user;
        }
    }
}
