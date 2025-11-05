using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PDFWebServiceMOA;
using SustitucionMOAWS.Util;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Configuration;

namespace SustitucionMOAWS.WSConsumers
{
    public class PDFConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUserSinPI"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPassSinPI"];
        public PDFResponse request(string documento, string ejercicio, string proveedor, string sociedad)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPMF_MOAOP_PDF()
                    {
                        PE_DOCUMENTO = documento,
                        PE_EJERCICIO = ejercicio,
                        PE_PROVEEDOR = proveedor,
                        PE_SOCIEDAD = sociedad
                    };
                    Log.Info($"SAP sin PI Z_MPMF_MOAOP_PDF request");
                    Log.Info(request.ToXml());
                    var response = agent.Z_MPMF_MOAOP_PDF(request);
                    SapLogHelper.LogResponse(response.ToXml(), "Z_MPMF_MOAOP_PDF");
                    return MapSinPI(response);
                }
                else
                {
                    SI_MPMF_MOAOP_PDFClient service = new SI_MPMF_MOAOP_PDFClient();
                    byte[] pdf = new byte[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    PDFWebServiceMOA.ZMPES4910 error = service.SI_MPMF_MOAOP_PDF(documento, ejercicio, proveedor, sociedad, out pdf);
                    return Map(pdf, error);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private PDFResponse MapSinPI(Z_MPMF_MOAOP_PDFResponse response)
        {
            PDFResponse result = new PDFResponse();
            if (response.MENSAJE_ERROR != null)
            {
                result.Error.codigo = response.MENSAJE_ERROR.CODIGO;
                result.Error.descripcion = response.MENSAJE_ERROR.DESCRIPCION;
                result.Error.tipo = response.MENSAJE_ERROR.TIPO;
            }
            if (response.PDF != null && response.PDF.Length > 0)
            {
                result.Pdf = new Pdf()
                {
                    data = response.PDF
                };

            }
            return result;
        }
        private PDFResponse Map(byte[] pdf, PDFWebServiceMOA.ZMPES4910 error)
        {
            PDFResponse result = new PDFResponse();
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
    }
}
