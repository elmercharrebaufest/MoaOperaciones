using SustitucionMOAModel.Entities;
using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using SustitucionMOAWS.WSRequests.AplicacionCartaPorte;
using System.Collections.Generic;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class AplicacionCartaPorteConsumer : IAplicacionCartaPorteConsumer
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        public AplicacionCartaPorteConsumer()
        {

        }

        public WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES7070[] ObtenerAplicacionesPendientesSinPI(AppCartasPortePendienteRequest request)
        {
            var agent = new Z_WS_MOAOP_DIRECTClient();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var requestPendiente = new Z_MPMF_MOAOP_PENDIENTE_APLICAR()
            {
                IM_CORREDOR  = string.IsNullOrEmpty(request.Corredor) ? " " : request.Corredor,
                IM_MATERIAL  = string.IsNullOrEmpty(request.Material) ? " " : request.Material,
                IM_PROVEEDOR = request.Proveedor,
            };

            Log.Info($"SAP sin PI Z_MPMF_MOAOP_PENDIENTE_APLICAR request");
            Log.Info(requestPendiente.ToXml());
            var response = agent.Z_MPMF_MOAOP_PENDIENTE_APLICAR(requestPendiente);
            Log.Info($"SAP sin PI Z_MPMF_MOAOP_PENDIENTE_APLICAR response");
            Log.Info(response.ToXml());

            return response.EX_SALIDA;
        }

        public AplicacionCartaPortePendienteAplicarWebServiceMOA.ZMPES7070[] ObtenerAplicacionesPendientes(AppCartasPortePendienteRequest request)
        {
            Log.Info($"SI_MPMF_MOAOP_PENDIENTE_APLICAR Request: {request.ToJson()}");

            SI_MPMF_MOAOP_PENDIENTE_APLICARClient serviceAppCartasPortePendienteAplicacion;
            serviceAppCartasPortePendienteAplicacion = new SI_MPMF_MOAOP_PENDIENTE_APLICARClient();
            serviceAppCartasPortePendienteAplicacion.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            serviceAppCartasPortePendienteAplicacion.ClientCredentials.UserName.Password = SAPCredential.getPassword();

            var result = serviceAppCartasPortePendienteAplicacion.SI_MPMF_MOAOP_PENDIENTE_APLICAR(
                IM_CORREDOR: request.Corredor,
                IM_MATERIAL: request.Material,
                IM_PROVEEDOR: request.Proveedor,
                out AplicacionCartaPortePendienteAplicarWebServiceMOA.ZMPES7070[] listaApplicaciones
                );

            Log.Debug($"SI_MPMF_MOAOP_PENDIENTE_APLICAR Result: {result}. Listado: {listaApplicaciones.ToJson()}.");
            return listaApplicaciones;
        }

        public AppCartasPortePendienteResponse ObtenerPendientesDeAplicar(AppCartasPortePendienteRequest request)
        {
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                var aplicacionesPendientes = ObtenerAplicacionesPendientesSinPI(request);
                return MapearAplicacionesPendientesSinPI(aplicacionesPendientes);

            }
            else
            {
                var aplicacionesPendientes = ObtenerAplicacionesPendientes(request);
                return MapearAplicacionesPendientes(aplicacionesPendientes);
            }
        }

        private AppCartasPortePendienteResponse MapearAplicacionesPendientes(AplicacionCartaPortePendienteAplicarWebServiceMOA.ZMPES7070[] listadoPendientes)
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
                        Centro = aplicPend.CENTRO,
                        NumeroCartaPorte = aplicPend.CCPP
                    });
                }

                if (!string.IsNullOrEmpty(aplicPend.CONTRATO))
                {
                    response.Contratos.Add(new AplicacionPendienteContrato
                    {
                        CodigoProveedor = aplicPend.PROVEEDOR,
                        Material = aplicPend.MATERIAL,
                        Centro = aplicPend.CENTRO,
                        NumeroContrato = aplicPend.CONTRATO,
                        TieneAnticipo = aplicPend.CD_CG == "X" || aplicPend.WARRANT == "X"
                    });
                }
            }

            return response;
        }
        private AppCartasPortePendienteResponse MapearAplicacionesPendientesSinPI(WS_GAQ_sin_PI_DIRECT_MOAOP.ZMPES7070[] listadoPendientes)
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
                        Centro = aplicPend.CENTRO,
                        NumeroCartaPorte = aplicPend.CCPP
                    });
                }

                if (!string.IsNullOrEmpty(aplicPend.CONTRATO))
                {
                    response.Contratos.Add(new AplicacionPendienteContrato
                    {
                        CodigoProveedor = aplicPend.PROVEEDOR,
                        Material = aplicPend.MATERIAL,
                        Centro = aplicPend.CENTRO,
                        NumeroContrato = aplicPend.CONTRATO,
                        TieneAnticipo = aplicPend.CD_CG == "X" || aplicPend.WARRANT == "X"
                    });
                }
            }

            return response;
        }

    }
}
