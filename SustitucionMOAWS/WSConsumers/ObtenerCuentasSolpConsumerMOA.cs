using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ObtenerCuentasSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using System.Net;
using SustitucionMOAWS.Interfaces;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerCuentasSolpConsumerMOA : IObtenerCuentasSolpConsumerMOA
    {
        private const string COMP_CODE = "MOA";

        SI_MMRFC_OBTENER_CUENTASClient service;

        public ObtenerCuentasSolpConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_CUENTAS&amp;interfaceNamespace=urn%3AOPERACIONES";

            service = new SI_MMRFC_OBTENER_CUENTASClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public object request()
        {
            try
            {
                string IM_COMP_CODE = COMP_CODE;
                string IM_GL_ACCOUNT = "";
                ZMPES5760[] EX_GL_ACCOUNT_LIST = new ZMPES5760[] { };
                BAPIRETURN[] EX_RETURN = new BAPIRETURN[] { };

                string error = service.SI_MMRFC_OBTENER_CUENTAS(IM_COMP_CODE, IM_GL_ACCOUNT, out EX_GL_ACCOUNT_LIST, out EX_RETURN);

                return map(error, EX_GL_ACCOUNT_LIST, EX_RETURN);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual object map(string error, ZMPES5760[] EX_GL_ACCOUNT_LIST, BAPIRETURN[] EX_RETURN)
        {
            CuentaWSMOAResponse result = new CuentaWSMOAResponse();
            result.Cuentas = new List<Cuenta> { };

            if(error == "200")
            {
                foreach (ZMPES5760 cuentaSolp in EX_GL_ACCOUNT_LIST)
                {
                    result.Cuentas.Add(new Cuenta()
                    {
                        Codigo = cuentaSolp.GL_ACCOUNT,
                        Descripcion = cuentaSolp.SHORT_TEXT
                    });
                }
                result.error = error;
            }

            return result;
        }
    }
}
