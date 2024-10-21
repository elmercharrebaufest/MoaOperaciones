using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using SustitucionMOA.Utils;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Permiso;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Entidades = SustitucionMOAModel.Entities;

namespace SustitucionMOA
{
    public partial class Startup
    {
        private static IAzureB2CService AzureB2CService
        {
            get { return DependencyResolver.Current.GetService<IAzureB2CService>(); }
        }

        /*
		* Configure the OWIN middleware
		*/

        public void ConfigureAuth(IAppBuilder app)
        {
            // Required for Azure webapps, as by default they force TLS 1.2 and this project attempts 1.0
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // ASP.NET web host compatible cookie manager
                CookieManager = new SystemWebChunkingCookieManager(),
                //ExpireTimeSpan = TimeSpan.FromDays(1)
            });

            var options = new OpenIdConnectAuthenticationOptions
            {
                // Generate the metadata address using the tenant and policy information
                MetadataAddress = String.Format(Globals.WellKnownMetadata, Globals.Tenant, Globals.DefaultPolicy),


                // These are standard OpenID Connect parameters, with values pulled from web.config
                ClientId = Globals.ClientId,
                RedirectUri = Globals.RedirectUri,
                PostLogoutRedirectUri = Globals.RedirectUri,

                // Specify the callbacks for each type of notifications
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                    AuthenticationFailed = OnAuthenticationFailed,
                    SecurityTokenValidated = OnSecurityTokenValidated

                },

                // Specify the claim type that specifies the Name property.
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    ValidateIssuer = false
                },

                // ASP.NET web host compatible cookie manager
                CookieManager = new SystemWebCookieManager(),

                // Specify the scope by appending all of the scopes requested into one string (separated by a blank space)
                Scope = $"openid profile offline_access",

                //UseTokenLifetime = false,
            };
            options.ProtocolValidator.RequireNonce = !System.Configuration.ConfigurationManager.AppSettings["SpaUrl"].ToString().Contains("localhost");
            options.ProtocolValidator.RequireState = !System.Configuration.ConfigurationManager.AppSettings["SpaUrl"].ToString().Contains("localhost");
            app.UseOpenIdConnectAuthentication(
                options
            );
        }



        //Agrego esta función del callback. Ya que esta es llamada desde el registro y desde el login. 
        private Task OnSecurityTokenValidated(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {

            ValidarLogin(notification.AuthenticationTicket.Identity);

            return Task.FromResult(0);
        }

        /*
		 *  On each call to Azure AD B2C, check if a policy (e.g. the profile edit or password reset policy) has been specified in the OWIN context.
		 *  If so, use that policy when making the call. Also, don't request a code (since it won't be needed).
		 */
        private Task OnRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            var policy = notification.OwinContext.Get<string>("Policy");

            if (!string.IsNullOrEmpty(policy) && !policy.Equals(Globals.DefaultPolicy))
            {
                notification.ProtocolMessage.UiLocales = "es-es";
                notification.ProtocolMessage.Scope = OpenIdConnectScope.OpenId;
                notification.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
                notification.ProtocolMessage.IssuerAddress = notification.ProtocolMessage.IssuerAddress.ToLower().Replace(Globals.DefaultPolicy.ToLower(), policy.ToLower());
            }

            return Task.FromResult(0);
        }

        /*
		 * Catch any failures received by the authentication middleware and handle appropriately
		 */
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page
            // because password reset is not supported by a "sign-up or sign-in policy"
            if (notification.ProtocolMessage.ErrorDescription != null && notification.ProtocolMessage.ErrorDescription.Contains("AADB2C90118"))
            {
                // If the user clicked the reset password link, redirect to the reset password route
                notification.Response.Redirect("/ResetPassword");
            }
            else if (notification.Exception.Message == "access_denied")
            {
                Log.AzureError(notification.Exception);
                notification.Response.Redirect("/");
            }
            else
            {
                Log.AzureError(notification.Exception);
                notification.Response.Redirect("/");
            }

            return Task.FromResult(0);
        }

        /*
		 * Callback function when an authorization code is received
		 */
        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            try
            {
                /*
				 The `MSALPerUserMemoryTokenCache` is created and hooked in the `UserTokenCache` used by `IConfidentialClientApplication`.
				 At this point, if you inspect `ClaimsPrinciple.Current` you will notice that the Identity is still unauthenticated and it has no claims,
				 but `MSALPerUserMemoryTokenCache` needs the claims to work properly. Because of this sync problem, we are using the constructor that
				 receives `ClaimsPrincipal` as argument and we are getting the claims from the object `AuthorizationCodeReceivedNotification context`.
				 This object contains the property `AuthenticationTicket.Identity`, which is a `ClaimsIdentity`, created from the token received from
				 Azure AD and has a full set of claims.
				 */

                IConfidentialClientApplication confidentialClient = MsalAppBuilder.BuildConfidentialClientApplication(new ClaimsPrincipal(notification.AuthenticationTicket.Identity));

                // Upon successful sign in, get & cache a token using MSAL
                AuthenticationResult result = await confidentialClient.AcquireTokenByAuthorizationCode(Globals.Scopes, notification.Code).ExecuteAsync();
            }
            catch (Exception ex)
            {
                Log.AzureError(ex);
            }
        }

        private void ValidarLogin(ClaimsIdentity notification)
        {
            try
            {
                string mail = notification.FindFirst("emails").Value;
                string CUIT = notification.FindFirst("extension_CUIT").Value;

                CUIT = CUIT.Replace("-", string.Empty);
                string GranosFlag = notification.FindFirst("extension_Tipodeproveedor").Value;

                Entidades.Usuario usuario = new Entidades.Usuario { Mail = mail, CUITRegistro = CUIT };

                usuario = AzureB2CService.LoguearUsuario(mail, CUIT, GranosFlag);

                notification.AddClaim(new Claim(Globals.ClaimsUserNameType, mail));
                notification.AddClaim(new Claim(Globals.ClaimsNombreType, usuario.ObtenerRazonSocial()));
                notification.AddClaim(new Claim(Globals.ClaimsProveedorType, usuario.ObtenerCodigoProveedor()));
                notification.AddClaim(new Claim(Globals.ClaimsProveedorId, usuario.ObtenerProveedor().Id.ToString()));
                notification.AddClaim(new Claim(Globals.ClaimsSeccionesVisitadas, usuario.SeccionesVisitadas ?? ""));
                notification.AddClaim(new Claim(Globals.ClaimsCuit, usuario.ObtenerProveedor().CUIT.ToString()));


                string tipoGranos = usuario.TipoUsuario.NombreCorto == "CORR" ? "G" : usuario.TipoUsuario.NombreCorto;

                if (usuario.TipoUsuario.NombreCorto == "CLI")
                {
                    tipoGranos = "NG";
                }

                var permisosUsuario = usuario.ObtenerPermisos();

                if (permisosUsuario.Contains(SustitucionMOASecurity.Permiso.CONSULTAR_HOME))
                    tipoGranos = "G";

                if (permisosUsuario.Contains(SustitucionMOASecurity.Permiso.CONSULTAR_HOME_NG))
                    tipoGranos = "NG";

                if (usuario.EsAdmin() ||
                   (permisosUsuario.Contains(SustitucionMOASecurity.Permiso.CONSULTAR_HOME) && permisosUsuario.Contains(SustitucionMOASecurity.Permiso.CONSULTAR_HOME_NG)))
                {
                    tipoGranos = "A";
                }

                string tipoUsuario = usuario.TipoUsuario.NombreCorto == "CORR" || usuario.TipoUsuario.NombreCorto == "CLI" ? usuario.TipoUsuario.NombreCorto : "PROV";

                notification.AddClaim(new Claim(Globals.ClaimsGranosFlagType, tipoGranos));
                notification.AddClaim(new Claim(Globals.ClaimsSociedadType, "MOA"));
                notification.AddClaim(new Claim(Globals.ClaimsEsNuevoUsuarioType, usuario.EsNuevoUsuario().ToString()));
                notification.AddClaim(new Claim(Globals.ClaimsTipoUsuarioType, tipoUsuario));

                if (usuario.EstaHabilitado())
                {
                    foreach (var permiso in permisosUsuario)
                    {
                        notification.AddClaim(new Claim(Globals.ClaimsPermisosType, permiso));
                    }
                }
                else
                {
                    notification.AddClaim(new Claim(Globals.ClaimsPermisosType, "ESTADO SOLICITUD"));
                    notification.AddClaim(new Claim(Globals.ClaimsPermisosType, "CONTACTO MAIL"));
                }
            }
            catch (Exception e)
            {
                Log.Error(System.Web.HttpContext.Current.Request.UserHostAddress, "", this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, e);
            }
        }

    }
}