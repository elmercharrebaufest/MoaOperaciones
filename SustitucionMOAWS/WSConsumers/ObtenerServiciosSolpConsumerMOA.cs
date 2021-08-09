using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ObtenerServiciosSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerServiciosSolpConsumerMOA
    {
        SI_MMRFC_OBTENER_SERVICIOSClient service = new SI_MMRFC_OBTENER_SERVICIOSClient();

        public object request()
        {
            try
            {
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
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

            foreach (ZMPES5710 servicioSolp in IM_SERVICELIST)
            {
                result.Servicios.Add(new Servicio()
                {
                    Id = servicioSolp.SERVICE,
                    Descripcion = servicioSolp.SHORT_TEXT,
                    NroGrupo = servicioSolp.MATL_GROUP,
                    Serv = servicioSolp.SERV_CAT,
                    Ser = servicioSolp.SERV_TYPE,
                    Edit = servicioSolp.EDITION,
                    Bas = servicioSolp.BASE_UOM,
                    SSCItem = servicioSolp.SSC_ITEM
                });
            }
            result.error = error;

            return result;
        }
    }
}
