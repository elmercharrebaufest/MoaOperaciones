using System;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Noticia;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class LoginService
    {
        public LoginWSMOAResponse login(string username, string pass)
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

        public NoticiasDetallesWSMOAResponse getNoticias(string proveedor) {
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
