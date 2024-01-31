using System.Configuration;

namespace SustitucionMOA.Utils
{
    public static class Globals
    {
        public static bool EsLocal = bool.Parse(ConfigurationManager.AppSettings["EsLocal"]);

        // App config settings
        public static string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static string AadInstance = ConfigurationManager.AppSettings["ida:AadInstance"];
        public static string Tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        public static string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        public static string RedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];
        //public static string ServiceUrl = ConfigurationManager.AppSettings["api:TaskServiceUrl"];

        // B2C policy identifiers
        public static string SignUpAndSignInPolicyId = ConfigurationManager.AppSettings["ida:SignUpAndSignInPolicyId"];
        
        public static string EditProfilePolicyId = ConfigurationManager.AppSettings["ida:EditProfilePolicyId"];
        public static string ResetPasswordPolicyId = ConfigurationManager.AppSettings["ida:ResetPasswordPolicyId"];

        public static string DefaultPolicy = SignUpAndSignInPolicyId;

        //// API Scopes
        //public static string ApiIdentifier = ConfigurationManager.AppSettings["api:ApiIdentifier"];
        //public static string ReadTasksScope = ApiIdentifier + ConfigurationManager.AppSettings["api:ReadScope"];
        //public static string WriteTasksScope = ApiIdentifier + ConfigurationManager.AppSettings["api:WriteScope"];
        public static string[] Scopes = new string[] { };

        // OWIN auth middleware constants
        public const string ObjectIdElement = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        // Authorities
        public static string B2CAuthority = string.Format(AadInstance, Tenant, DefaultPolicy);
        public static string WellKnownMetadata = $"{AadInstance}/v2.0/.well-known/openid-configuration";

        public const string ClaimsUserNameType = "userName";
        public const string ClaimsNombreType = "nombre";
        public const string ClaimsPermisosType = "permisos";
        public const string ClaimsProveedorType = "proveedor";
        public const string ClaimsGranosFlagType = "granosFlag";
        public const string ClaimsSociedadType = "sociedad";
        public const string ClaimsNoticiasType = "noticias";
        public const string ClaimsTipoUsuarioType = "tipoUsuario";
        public const string ClaimsEsNuevoUsuarioType = "esNuevoUsuario";
        public const string ClaimsSeccionesVisitadas = "seccionesVisitadas";
        public const string ClaimsProveedorId = "proveedorId";
        public const string ClaimsCuit= "cuit";

    }
}