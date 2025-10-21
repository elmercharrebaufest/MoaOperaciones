using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PDFProformaFinalWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class PDFProformaFinalConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public PDFResponse request(string contrato, string pedido)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new Z_MPMF_MOAOP_PDF_PROFORMA()
                    {
                        IM_CONTRATO = contrato ?? string.Empty,
                        IM_PEDIDO = pedido ?? string.Empty
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PDF_PROFORMA request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_PDF_PROFORMA(request);
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PDF_PROFORMA response");
                    Log.Info(response.ToXml());

                    return Map(response.EX_BASE64);
                }
                else
                {
                    var service = new SI_MPMF_MOAOP_PDF_PROFORMAClient();
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    var pdf = service.SI_MPMF_MOAOP_PDF_PROFORMA(contrato, pedido);
                    return Map(pdf);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al obtener PDF Proforma de SAP con contrato: {contrato} y pedido: {pedido}.");
                throw;
            }
        }

        private PDFResponse Map(byte[] pdf)
        {
            PDFResponse result = new PDFResponse() { };

            if (pdf != null && pdf.Length > 0)
            {
                result.Pdf = new Pdf()
                {
                    data = pdf
                };
            }

            return result;
        }
    }
}
