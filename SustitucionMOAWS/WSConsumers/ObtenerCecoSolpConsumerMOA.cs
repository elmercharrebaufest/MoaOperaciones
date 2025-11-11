using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerCecoSolpWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerCecoSolpConsumerMOA : IObtenerCecoSolpConsumerMOA
    {
        private const string COMP_CODE = "MOA";
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];

        public ObtenerCecoSolpConsumerMOA()
        {

        }

        public object request()
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    string IM_COMP_CODE = COMP_CODE;
                    string IM_COSTCENTER = "";

                    var request = new Z_MMRFC_OBTENER_CECO()
                    {
                        IM_COMP_CODE = IM_COMP_CODE,
                        IM_COSTCENTER = IM_COSTCENTER
                    };
                    Log.Info($"SAP sin PI Z_MMRFC_OBTENER_CECO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MMRFC_OBTENER_CECO(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MMRFC_OBTENER_CECO");
                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_OBTENER_CECOClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_CECO&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_OBTENER_CECOClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    string IM_COMP_CODE = COMP_CODE;
                    string IM_COSTCENTER = "";
                    string EX_EXITO = "";
                    ObtenerCecoSolpWebServiceMOA.BAPIRETURN EX_RETURN = new ObtenerCecoSolpWebServiceMOA.BAPIRETURN();
                    ObtenerCecoSolpWebServiceMOA.BAPI0012_2[] error = service.SI_MMRFC_OBTENER_CECO(IM_COMP_CODE, IM_COSTCENTER, out EX_EXITO, out EX_RETURN);
                    return map(error, EX_EXITO);
                }


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private object MapSinPI(Z_MMRFC_OBTENER_CECOResponse response)
        {
            CecoWSMOAResponse result = new CecoWSMOAResponse();
            result.Cecos = new List<Ceco> { };
            if (response.EX_EXITO == "200")
            {
                foreach (WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI0012_2 ceco in response.EX_COSTCENTER_LIST)
                {
                    result.Cecos.Add(new Ceco()
                    {
                        CostCenter = ceco.COSTCENTER,
                        Descripcion = ceco.COCNTR_TXT
                    });
                }
            }
            return result;
        }

        protected virtual object map(ObtenerCecoSolpWebServiceMOA.BAPI0012_2[] error, string EX_EXITO)
        {
            CecoWSMOAResponse result = new CecoWSMOAResponse();
            result.Cecos = new List<Ceco> { };

            if (EX_EXITO == "200")
            {
                foreach (ObtenerCecoSolpWebServiceMOA.BAPI0012_2 ceco in error)
                {
                    result.Cecos.Add(new Ceco()
                    {
                        CostCenter = ceco.COSTCENTER,
                        Descripcion = ceco.COCNTR_TXT
                    });
                }
            }

            return result;
        }

    }
}
