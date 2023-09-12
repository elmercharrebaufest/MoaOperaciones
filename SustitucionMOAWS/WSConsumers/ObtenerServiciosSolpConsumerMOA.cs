using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerServiciosSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerServiciosSolpConsumerMOA : IObtenerServiciosSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_SERVICIOSClient service;

        public ObtenerServiciosSolpConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_SERVICIOS&amp;interfaceNamespace=urn%3AOPERACIONES";

            service = new SI_MMRFC_OBTENER_SERVICIOSClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public object request()
        {
            try
            {
                BAPIASNRAN[] EX_SERVICESELECTION = new BAPIASNRAN[] { };
                BAPIASKRAN[] EX_SRVSHORTTEXTSELECTION = new BAPIASKRAN[] { };
                BAPIRET2[] IM_RETURN = new BAPIRET2[] { };
                ZMPES5710[] IM_SERVICELIST = new ZMPES5710[] { };

                string error = service.SI_MMRFC_OBTENER_SERVICIOS(EX_SERVICESELECTION, EX_SRVSHORTTEXTSELECTION, out IM_RETURN, out IM_SERVICELIST);

                return map(error, IM_SERVICELIST, IM_RETURN);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected virtual object map(string error, ZMPES5710[] IM_SERVICELIST, BAPIRET2[] IM_RETURN)
        {
            ServicioWSMOAResponse result = new ServicioWSMOAResponse();
            result.Servicios = new List<Servicio> { };

            if (error == "200")
            {
                foreach (ZMPES5710 servicioSolp in IM_SERVICELIST)
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
