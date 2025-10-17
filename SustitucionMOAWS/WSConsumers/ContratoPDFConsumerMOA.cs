using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.ContratoPDFWebServiceMOA;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class ContratoPDFConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserS4"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassS4"];

        public PDFResponse request(string proveedor, string contrato)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var request = new Z_MPMF_MOAOP_PDF_CONTRATO()
                    {
                        IM_CONTRATO = contrato,
                        IM_PROVEEDOR = proveedor,
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PDF_CONTRATO request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_PDF_CONTRATO(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_PDF_CONTRATO"); Log.Info(response.ToXml());
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_PDF_CONTRATOClient service = new SI_MPMF_MOAOP_PDF_CONTRATOClient();
                    byte[] pdf = new byte[] { };
                    ContratoPDFWebServiceMOA.ITCOO[] itcoo = new ContratoPDFWebServiceMOA.ITCOO[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    ContratoPDFWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_PDF_CONTRATO(contrato, proveedor, out itcoo, out pdf);
                    return Map(pdf, error);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private PDFResponse Map(byte[] pdf, ContratoPDFWebServiceMOA.ZMPES4910 error)
        {
            PDFResponse result = new PDFResponse() { };
            if (error != null)
            {
                result.Error.codigo = error.CODIGO;
                result.Error.descripcion = error.DESCRIPCION;
                result.Error.tipo = error.TIPO;
            }
            if (pdf != null && pdf.Length > 0)
            {
                result.Pdf = new Pdf()
                {
                    data = pdf
                };
            }
            return result;
        }
        private PDFResponse MapSinPI(Z_MPMF_MOAOP_PDF_CONTRATOResponse response)
        {
            PDFResponse result = new PDFResponse() { };
            if (response.EX_MENSAJE_ERROR != null)
            {
                result.Error.codigo = response.EX_MENSAJE_ERROR.CODIGO;
                result.Error.descripcion = response.EX_MENSAJE_ERROR.DESCRIPCION;
                result.Error.tipo = response.EX_MENSAJE_ERROR.TIPO;
            }
            if (response.EX_PDF != null && response.EX_PDF.Length > 0)
            {
                result.Pdf = new Pdf()
                {
                    data = response.EX_PDF
                };
            }
            return result;
        }
    }
}
