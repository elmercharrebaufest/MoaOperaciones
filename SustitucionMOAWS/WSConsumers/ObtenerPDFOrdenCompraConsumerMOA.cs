using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.ObtenerPDFOrdenCompraWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerPDFOrdenCompraConsumerMOA: IObtenerPDFOrdenCompraConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ObtenerPDFOrdenCompraConsumerMOA()
        {

        }

        public byte[] Request(string nroOrdenCompra)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    byte[] pdf = new byte[] { };
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MMRFC_ENVIAR_PDF_OC()
                    {
                        IM_ORDEN_COMPRA = nroOrdenCompra
                    };

                    Log.Info($"SAP sin PI Z_MMRFC_ENVIAR_PDF_OC request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MMRFC_ENVIAR_PDF_OC(request);
                    pdf = response.EX_PDF;

                    Log.Info($"SAP sin PI Z_MMRFC_ENVIAR_PDF_OC response");
                    Log.Info(response.ToXml());

                    return pdf;
                }
                else
                {
                    SI_MMRFC_ENVIAR_PDF_OCClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_ENVIAR_PDF_OC&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_ENVIAR_PDF_OCClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    byte[] pdf = new byte[] { };
                    var response = service.SI_MMRFC_ENVIAR_PDF_OC(nroOrdenCompra, out pdf);
                    return pdf;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
