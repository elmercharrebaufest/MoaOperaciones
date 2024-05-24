using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WSRequests.AplicacionCartaPorte;
using System.Collections.Generic;

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

            Log.Debug($"SI_MPMF_MOAOP_PENDIENTE_APLICAR Result: {result}. Listado: {listaApplicaciones.ToJson()}.");
            return listaApplicaciones;
        }

        public AppCartasPortePendienteResponse ObtenerPendientesDeAplicar(AppCartasPortePendienteRequest request)
        {
            var aplicacionesPendientes = ObtenerAplicacionesPendientes(request);
            return MapearAplicacionesPendientes(aplicacionesPendientes);
        }

        private AppCartasPortePendienteResponse MapearAplicacionesPendientes(ZMPES7070[] listadoPendientes)
        {
            var response = new AppCartasPortePendienteResponse
            {
                CartasDePorte = new List<AplicacionPendienteCartaPorte>(),
                Contratos = new List<AplicacionPendienteContrato>()
            };

            foreach (var aplicPend in listadoPendientes)
            {
                if (!string.IsNullOrEmpty(aplicPend.CCPP))
                {
                    response.CartasDePorte.Add(new AplicacionPendienteCartaPorte
                    {
                        Cantidad = aplicPend.CANTIDAD,
                        Material = aplicPend.MATERIAL,
                        NumeroCartaPorte = aplicPend.CCPP
                    });
                }

                if (!string.IsNullOrEmpty(aplicPend.CONTRATO))
                {
                    response.Contratos.Add(new AplicacionPendienteContrato
                    {
                        CodigoProveedor = aplicPend.PROVEEDOR,
                        Material = aplicPend.MATERIAL,
                        NumeroContrato = aplicPend.CONTRATO,
                        TieneAnticipo = aplicPend.CD_CG == "X" || aplicPend.WARRANT == "X"
                    });
                }
            }

            return response;
        }
    }
}
