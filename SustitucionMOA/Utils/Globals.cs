using System.Configuration;

namespace SustitucionMOA.Utils
{
    public static class Globals
    {
        public readonly static bool EsLocal = bool.Parse(ConfigurationManager.AppSettings["EsLocal"]);

        // App config settings
        public readonly static string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public readonly static string ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public readonly static string AadInstance = ConfigurationManager.AppSettings["ida:AadInstance"];
        public readonly static string Tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        public readonly static string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        public readonly static string RedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

        // B2C policy identifiers
        public readonly static string SignUpAndSignInPolicyId = ConfigurationManager.AppSettings["ida:SignUpAndSignInPolicyId"];

        public readonly static string EditProfilePolicyId = ConfigurationManager.AppSettings["ida:EditProfilePolicyId"];
        public readonly static string ResetPasswordPolicyId = ConfigurationManager.AppSettings["ida:ResetPasswordPolicyId"];

        public readonly static string DefaultPolicy = SignUpAndSignInPolicyId;

        // API Scopes
        public readonly static string[] Scopes = new string[] { };

        // OWIN auth middleware constants
        public const string ObjectIdElement = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

        // Authorities
        public readonly static string B2CAuthority = string.Format(AadInstance, Tenant, DefaultPolicy);
        public readonly static string WellKnownMetadata = $"{AadInstance}/v2.0/.well-known/openid-configuration";

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
        public const string ClaimsCuit = "cuit";
        public const string ClaimsEsCodigoCorredorType = "esCodigoCorredor";

    }
}