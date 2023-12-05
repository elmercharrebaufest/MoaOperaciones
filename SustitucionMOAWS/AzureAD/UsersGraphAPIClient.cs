using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAWS.AzureAD.Model;
using SustitucionMOAWS.Logger;
using System.Configuration;

namespace SustitucionMOAWS.AzureAD
{
    public class UsersGraphAPIClient : IUsersGraphAPIClient
    {
        private readonly string GraphEndpoint = "https://graph.microsoft.com/";
        private readonly string GraphVersion = "v1.0";

        private static readonly string TenantId = ConfigurationManager.AppSettings["AzureADTenantId"];
        private static readonly string AppId = ConfigurationManager.AppSettings["AzureADAppId"];
        private static readonly string AppSecret = ConfigurationManager.AppSettings["AzureADAppSecret"];

        private string AccessToken { get; }

        public UsersGraphAPIClient()
        {
            Log.Info("Inicializa cliente API Graph [Users]");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(AppId)
                .WithTenantId(TenantId)
                .WithClientSecret(AppSecret)
                .Build();

            var authResult = confidentialClientApplication
                .AcquireTokenForClient(new string[] { "https://graph.microsoft.com/.default" })
                .ExecuteAsync().GetAwaiter().GetResult();

            AccessToken = authResult.AccessToken;
        }

        public virtual ObtenerUsuarioResponse ObtenerUsuarioPorDisplayName(string displayName)
        {
            Log.Info($"Obtener usuario por DisplayName: {displayName}");
            string url = string.Empty, apiResponse = string.Empty;

            try
            {
                url = ObtenerUrl("users", $"$select=id,userPrincipalName,businessPhones,displayName," +
                    $"givenName,jobTitle,mail,mobilePhone,officeLocation,preferredLanguage,surname " +
                    "&$filter=identities/any(c:c/issuerAssignedId " +
                    $"eq '{displayName}' and c/issuer eq '{displayName}')");

                apiResponse = SendGraphRequest(HttpMethod.Get, url, null).GetAwaiter().GetResult();

                var obtenerUsuarioResponse = JsonConvert.DeserializeObject<ObtenerUsuarioResponse>(apiResponse);
                return obtenerUsuarioResponse;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ocurrió un error al obtener usuario por DisplayName {displayName}. Url: {url}, response: {apiResponse}");
                throw new Exception("Error al obtener usuario Azure", ex);
            }
        }

        public virtual void BorrarUsuario(string idUsuario)
        {
            Log.Info($"Borrar usuario con ID {idUsuario}");
            string url = string.Empty, apiResponse = string.Empty;

            try
            {
                url = ObtenerUrl($"users/{idUsuario}", null);

                apiResponse = SendGraphRequest(HttpMethod.Delete, url, null).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Ocurrió un error al borrar el usuario ID {idUsuario}. Url: {url}, response: {apiResponse}");
                throw new Exception("Error al borrar usuario Azure", ex);
            }
        }

        public string PruebaCrearUsuario()
        {
            try
            {
                var url = ObtenerUrl("users", null);
                string data = "{\"identities\":[{\"signInType\":\"emailAddress\",\"issuer\":\"moagroqa.onmicrosoft.com\",\"issuerAssignedId\":\"333adminitracion@eryrytrut.com.ar\"}],\"displayName\":\"333adminitracion@eryrytrut.com.ar\",\"passwordPolicies\":\"DisablePasswordExpiration\",\"passwordProfile\":{\"password\":\"Ingreso11\",\"forceChangePasswordNextSignIn\":false,\"@odata.type\":\"microsoft.graph.passwordProfile\"},\"@odata.type\":\"microsoft.graph.user\",\"extension_e01fedc5a82f4cc1b1c0f3626cc123d2_Tipodeproveedor\":\"No Granos\",\"extension_e01fedc5a82f4cc1b1c0f3626cc123d2_CUIT\":\"33715621440\"}";

                var task = SendGraphRequest(HttpMethod.Post, url, data);

                task.Wait();

                var result = task.Result;

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> SendGraphRequest(HttpMethod method, string url, string data)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                using (HttpRequestMessage request = new HttpRequestMessage(method, url))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                    if (!string.IsNullOrEmpty(data))
                    {
                        request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                    }

                    using (HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        var error = await response.Content.ReadAsStringAsync();

                        if (!response.IsSuccessStatusCode)
                        {
                            //if (response.StatusCode == (HttpStatusCode)429)
                            //{
                            //}
                            throw new Exception(error);
                        }

                        var resContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        return resContent;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ocurrió un error al enviar request a Microsoft Graph");
                throw new Exception("Error en la comunicación con Azure");
            }
        }

        private string ObtenerUrl(string resource, string query)
        {
            var url = $"{this.GraphEndpoint}{this.GraphVersion}/{resource}";

            if (!string.IsNullOrEmpty(query))
            {
                url += "?" + query;
            }

            return url;
        }
    }
}
