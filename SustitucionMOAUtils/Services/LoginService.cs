using System;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class LoginService : ILoginService
    {
        public LoginService()
        {

        }
        public LoginWSMOAResponse Login(string username, string pass)
        {
            try
            {
                return new LoginConsumerMOA().request(username, pass);
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }

        public NoticiasDetallesWSMOAResponse ObtenerNoticias(string proveedor) {
            try
            {
                return new NoticiasDetalleConsumerMOA().request(proveedor, DateTime.Now.ToString("yyyy-MM-dd"));
            }
            catch (Exception e) {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
