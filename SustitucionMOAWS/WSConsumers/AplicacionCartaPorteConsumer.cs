using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSRequests.AplicacionCartaPorte;


namespace SustitucionMOAWS.WSConsumers
{
    public class AplicacionCartaPorteConsumer : IAplicacionCartaPorteConsumer
    {
        readonly SI_MPMF_MOAOP_PENDIENTE_APLICARClient serviceAppCartasPortePendienteAplicacion;
        public AplicacionCartaPorteConsumer()
        {
            serviceAppCartasPortePendienteAplicacion = new SI_MPMF_MOAOP_PENDIENTE_APLICARClient();
            serviceAppCartasPortePendienteAplicacion.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            serviceAppCartasPortePendienteAplicacion.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ZMPES7070[] ObtenerAplicacionesPendientes(AppCartasPortePendienteRequest request)
        {
            ZMPES7070[] listaApplicaciones;
            var result = serviceAppCartasPortePendienteAplicacion.SI_MPMF_MOAOP_PENDIENTE_APLICAR(
                IM_CORREDOR: request.Corredor,
                IM_MATERIAL: request.Material,
                IM_PROVEEDOR: request.Proveedor,
                out listaApplicaciones
                );

            return listaApplicaciones;
        }
    }
}
