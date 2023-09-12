using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerCecoSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerCecoSolpConsumerMOA : IObtenerCecoSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_CECOClient service;
        private const string COMP_CODE = "MOA";

        public ObtenerCecoSolpConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_CECO&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_OBTENER_CECOClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));         
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public object request()
        {
            try
            {
                string IM_COMP_CODE = COMP_CODE;
                string IM_COSTCENTER = "";
                string EX_EXITO = "";
                BAPIRETURN EX_RETURN = new BAPIRETURN();

                BAPI0012_2[] error = service.SI_MMRFC_OBTENER_CECO(IM_COMP_CODE, IM_COSTCENTER, out EX_EXITO, out EX_RETURN);

                return map(error, EX_EXITO);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual object map(BAPI0012_2[] error, string EX_EXITO)
        {
            CecoWSMOAResponse result = new CecoWSMOAResponse();
            result.Cecos = new List<Ceco> { };

            if (EX_EXITO == "200")
            {
                foreach (BAPI0012_2 ceco in error)
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
