using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
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
            Log.Info($"SI_MPMF_MOAOP_PENDIENTE_APLICAR Request: {request.ToJson()}");

            var result = serviceAppCartasPortePendienteAplicacion.SI_MPMF_MOAOP_PENDIENTE_APLICAR(
                IM_CORREDOR: request.Corredor,
                IM_MATERIAL: request.Material,
                IM_PROVEEDOR: request.Proveedor,
                out ZMPES7070[] listaApplicaciones
                );

            Log.Info($"SI_MPMF_MOAOP_PENDIENTE_APLICAR Result: {result}. Listado: {listaApplicaciones.ToJson()}.");
            return listaApplicaciones;
        }
    }
}
