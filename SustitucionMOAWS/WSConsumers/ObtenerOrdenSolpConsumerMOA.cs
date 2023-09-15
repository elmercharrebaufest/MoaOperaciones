using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerOrdenSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerOrdenSolpConsumerMOA : IObtenerOrdenSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_ORDENClient service;
        private const string COMP_CODE = "MOA";

        public ObtenerOrdenSolpConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_ORDEN&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_OBTENER_ORDENClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }


        public object request(string idOrder = "")
        {
            try
             {
                string IM_NAME = "";
                string IM_ORDER = idOrder;
                string IM_TYPE = "";
                ZMPES5650[] EX_ORDER_LIST = new ZMPES5650[] { };
                BAPIRETURN[] EX_RETURN = new BAPIRETURN[] { };

                string error = service.SI_MMRFC_OBTENER_ORDEN(COMP_CODE, IM_NAME, IM_ORDER, IM_TYPE, out EX_ORDER_LIST, out EX_RETURN);

                return map(error, EX_ORDER_LIST, EX_RETURN);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual object map(string error, ZMPES5650[] EX_ORDER_LIST, BAPIRETURN[] EX_RETURN)
        {
            OrdenWSMOAResponse result = new OrdenWSMOAResponse();
            result.Ordenes = new List<Orden> { };

            if (error == "200")
            {
                foreach (ZMPES5650 orden in EX_ORDER_LIST)
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
