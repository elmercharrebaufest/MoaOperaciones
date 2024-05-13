using System;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerPDFOrdenCompraWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerPDFOrdenCompraConsumerMOA: IObtenerPDFOrdenCompraConsumerMOA
    {
        readonly SI_MMRFC_ENVIAR_PDF_OCClient service;

        public ObtenerPDFOrdenCompraConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_ENVIAR_PDF_OC&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_ENVIAR_PDF_OCClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public byte[] Request(string nroOrdenCompra)
        {
            try
            {
                byte[] pdf = new byte[] { };
                var response = service.SI_MMRFC_ENVIAR_PDF_OC(nroOrdenCompra, out pdf);

                return pdf;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
