using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerServiciosSolpWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerServiciosSolpConsumerMOA : IObtenerServiciosSolpConsumerMOA
    {

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public ObtenerServiciosSolpConsumerMOA()
        {

        }

        public object request(string codigo)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new Z_MMRFC_OBTENER_SERVICIOS()
                    {
                        IM_SERVICESELECTION = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIASNRAN[] { },
                        IM_SRVSHORTTEXTSELECTION = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIASKRAN[] { }
                    };

                    if (!string.IsNullOrEmpty(codigo))
                    {
                        var IM_ServiceSelectionList = new List<WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIASNRAN>();
                        IM_ServiceSelectionList.Add(new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIASNRAN { SIGN = "I", OPTION = "EQ", SERVICE_LOW = codigo });
                        WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIASNRAN[] IM_SERVICESELECTION = IM_ServiceSelectionList.ToArray();
                        request.IM_SERVICESELECTION = IM_SERVICESELECTION;
                    }

                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_SERVICIOS request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MMRFC_OBTENER_SERVICIOS(request);
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_SERVICIOS response");
                    Log.Info(response.ToXml());
                    return MapSinPI(response);


                }
                else
                {
                    SI_MMRFC_OBTENER_SERVICIOSClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_SERVICIOS&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_OBTENER_SERVICIOSClient(SAPCredential.CrearSapLongBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    ObtenerServiciosSolpWebServiceMOA.BAPIASNRAN[] EX_SERVICESELECTION = new ObtenerServiciosSolpWebServiceMOA.BAPIASNRAN[] { };

                    if (!string.IsNullOrEmpty(codigo))
                    {
                        var IM_ServiceSelectionList = new List<ObtenerServiciosSolpWebServiceMOA.BAPIASNRAN>();
                        IM_ServiceSelectionList.Add(new ObtenerServiciosSolpWebServiceMOA.BAPIASNRAN { SIGN = "I", OPTION = "EQ", SERVICE_LOW = codigo });
                        EX_SERVICESELECTION = IM_ServiceSelectionList.ToArray();
                    }

                    ObtenerServiciosSolpWebServiceMOA.BAPIASKRAN[] EX_SRVSHORTTEXTSELECTION = new ObtenerServiciosSolpWebServiceMOA.BAPIASKRAN[] { };
                    ObtenerServiciosSolpWebServiceMOA.BAPIRET2[] IM_RETURN = new ObtenerServiciosSolpWebServiceMOA.BAPIRET2[] { };
                    ObtenerServiciosSolpWebServiceMOA.ZMPES5710[] IM_SERVICELIST = new ObtenerServiciosSolpWebServiceMOA.ZMPES5710[] { };

                    string error = service.SI_MMRFC_OBTENER_SERVICIOS(EX_SERVICESELECTION, EX_SRVSHORTTEXTSELECTION, out IM_RETURN, out IM_SERVICELIST);

                    return map(error, IM_SERVICELIST, IM_RETURN);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private object MapSinPI(Z_MMRFC_OBTENER_SERVICIOSResponse response)
        {
            ServicioWSMOAResponse result = new ServicioWSMOAResponse();
            result.Servicios = new List<Servicio> { };

            if (response.EX_EXITO == "200")
            {
                foreach (WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5710 servicioSolp in response.EX_SERVICELIST)
                {
                    result.Servicios.Add(new Servicio()
                    {
                        Codigo = servicioSolp.SERVICE,
                        Descripcion = servicioSolp.SHORT_TEXT,
                        NroGrupo = servicioSolp.MATL_GROUP,
                        Serv = servicioSolp.SERV_CAT,
                        Ser = servicioSolp.SERV_TYPE,
                        Edit = servicioSolp.EDITION,
                        Bas = servicioSolp.BASE_UOM,
                        SSCItem = servicioSolp.SSC_ITEM
                    });
                }
            }

            result.error = response.EX_EXITO;

            return result;
        }

        protected virtual object map(string error, ObtenerServiciosSolpWebServiceMOA.ZMPES5710[] IM_SERVICELIST, ObtenerServiciosSolpWebServiceMOA.BAPIRET2[] IM_RETURN)
        {
            ServicioWSMOAResponse result = new ServicioWSMOAResponse();
            result.Servicios = new List<Servicio> { };

            if (error == "200")
            {
                foreach (ObtenerServiciosSolpWebServiceMOA.ZMPES5710 servicioSolp in IM_SERVICELIST)
                {
                    result.Servicios.Add(new Servicio()
                    {
                        Codigo = servicioSolp.SERVICE,
                        Descripcion = servicioSolp.SHORT_TEXT,
                        NroGrupo = servicioSolp.MATL_GROUP,
                        Serv = servicioSolp.SERV_CAT,
                        Ser = servicioSolp.SERV_TYPE,
                        Edit = servicioSolp.EDITION,
                        Bas = servicioSolp.BASE_UOM,
                        SSCItem = servicioSolp.SSC_ITEM
                    });
                }
            }

            result.error = error;

            return result;
        }
    }
}
