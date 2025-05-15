using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerOrdenSolpWebServiceMOA;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerOrdenSolpConsumerMOA : IObtenerOrdenSolpConsumerMOA
    {
        private const string COMP_CODE = "MOA";
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ObtenerOrdenSolpConsumerMOA()
        {

        }


        public object request(string idOrder = "")
        {
            try
             {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    string IM_NAME = "";
                    string IM_ORDER = idOrder;
                    string IM_TYPE = "";
                    var req = new Z_MMRFC_OBTENER_ORDEN()
                    {
                        IM_NAME = IM_NAME,
                        IM_ORDER = IM_ORDER,
                        IM_TYPE = IM_TYPE,
                        IM_COMP_CODE = COMP_CODE
                    };
                    Log.Info($"Sin PI Z_MMRFC_OBTENER_ORDEN request: {new { IM_NAME, IM_ORDER, IM_TYPE, COMP_CODE }}");
                    var response = agent.Z_MMRFC_OBTENER_ORDEN(req);
                    Log.Info($"Sin PI Z_MMRFC_OBTENER_ORDEN Response: {response.EX_ORDER_LIST}");
                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_OBTENER_ORDENClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_ORDEN&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_OBTENER_ORDENClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string IM_NAME = "";
                    string IM_ORDER = idOrder;
                    string IM_TYPE = "";
                    ObtenerOrdenSolpWebServiceMOA.ZMPES5650[] EX_ORDER_LIST = new ObtenerOrdenSolpWebServiceMOA.ZMPES5650[] { };
                    ObtenerOrdenSolpWebServiceMOA.BAPIRETURN[] EX_RETURN = new ObtenerOrdenSolpWebServiceMOA.BAPIRETURN[] { };
                    string error = service.SI_MMRFC_OBTENER_ORDEN(COMP_CODE, IM_NAME, IM_ORDER, IM_TYPE, out EX_ORDER_LIST, out EX_RETURN);
                    return map(error, EX_ORDER_LIST, EX_RETURN);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private object MapSinPI(Z_MMRFC_OBTENER_ORDENResponse response)
        {
            OrdenWSMOAResponse result = new OrdenWSMOAResponse();
            result.Ordenes = new List<Orden> { };

            //if (response.EX_EXITO == "200"){
            if (response.EX_ORDER_LIST.Length > 0)
            {
                foreach (var orden in response.EX_ORDER_LIST)
                {
                    result.Ordenes.Add(new Orden()
                    {
                        Descripcion = orden.NAME,
                        Tipo = orden.TYPE,
                        Clase = orden.CLASS,
                        CompCode = orden.COMP_CODE,
                        Codigo = orden.ORDER
                    });
                }
            }
            //}
            //result.Error = response.EX_EXITO;

            return result;
        }

        protected virtual object map(string error, ObtenerOrdenSolpWebServiceMOA.ZMPES5650[] EX_ORDER_LIST, ObtenerOrdenSolpWebServiceMOA.BAPIRETURN[] EX_RETURN)
        {
            OrdenWSMOAResponse result = new OrdenWSMOAResponse();
            result.Ordenes = new List<Orden> { };

            if (error == "200")
            {
                foreach (ObtenerOrdenSolpWebServiceMOA.ZMPES5650 orden in EX_ORDER_LIST)
                {
                    result.Ordenes.Add(new Orden()
                    {
                        Descripcion = orden.NAME,
                        Tipo = orden.TYPE,
                        Clase = orden.CLASS,
                        CompCode = orden.COMP_CODE,
                        Codigo = orden.ORDER
                    });
                }
            }
            result.Error = error;

            return result;
        }
    }
}
