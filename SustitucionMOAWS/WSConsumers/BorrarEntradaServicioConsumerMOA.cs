using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOARepositorio;
using SustitucionMOAWS.BorrarEntradaServicioNuevoWebServiceMOA;
using SustitucionMOAWS.BorrarEntradaServicioWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using System;
using System.Linq;
using System.Security.Policy;

namespace SustitucionMOAWS.WSConsumers
{
    public class BorrarEntradaServicioConsumerMOA : IBorrarEntradaServicioConsumerMOA
    {
        SI_MMRFC_BORRAR_HESClient service;
        //private const string COMP_CODE = "MOA";
        //private readonly IRepositorio repositorio;

        public BorrarEntradaServicioConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_BORRAR_HES&amp;interfaceNamespace=urn:OPERACIONES";
            service = new SI_MMRFC_BORRAR_HESClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            //this.repositorio = repositorio;
        }

        public string BorrarEntradaServicio(string nroES, string fechaContabilizacion)
        {
            try
            {
                string ENTRYSHEET = nroES;
                BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] RETURN = new BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] { };

                service.SI_MMRFC_BORRAR_HES(nroES,fechaContabilizacion, ref RETURN);

                return Map(RETURN);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string Map(BorrarEntradaServicioNuevoWebServiceMOA.BAPIRET2[] RETURN)
        {
            //string result = RETURN[0].MESSAGE;
            string result = string.Join(Environment.NewLine, RETURN.Select(r => r.MESSAGE));
            //string[] messages = RETURN.Select(r => r.MESSAGE).ToArray();

            return result;
        }

    }

    public interface IBorrarEntradaServicioConsumerMOA
    {
    }
}
