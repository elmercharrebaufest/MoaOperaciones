using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PDFComprobantesNGWebServiceMOA;
using SustitucionMOAWS.PDFWebServiceMOA;
using SustitucionMOAWS.ScatoComandosWebService;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_MOAOP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAWS.WSConsumers
{
    public class PDFComprobantesNGConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public PDFResponse request(string CodigoProveedorSAP, string FechaDocumento, string NumeroLegalDocumento)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var request = new Z_MPRFC_MOAOP_COMP_NOGRANOSPDF()
                    {
                        IM_BLDAT = FechaDocumento,
                        IM_LIFNR = CodigoProveedorSAP,
                        IM_XBLNR = NumeroLegalDocumento
                    };
                    var response = agent.Z_MPRFC_MOAOP_COMP_NOGRANOSPDF(request);
                    byte[] bytes = System.Convert.FromBase64String(response.EX_BASE64);

                    return Map(bytes);

                }
                else
                {
                    SI_MPMF_MOAOP_COMP_NOGRANOSPDFClient service = new SI_MPMF_MOAOP_COMP_NOGRANOSPDFClient();
                    byte[] pdf = new byte[] { };
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                    pdf = service.SI_MPRFC_MOAOP_COMP_NOGRANOSPDF(FechaDocumento, CodigoProveedorSAP, NumeroLegalDocumento);
                    return Map(pdf);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        private PDFResponse Map(byte[] pdf)
        {
            PDFResponse result = new PDFResponse();

            //if (error != null)
            //{
            //    result.error.codigo = error.CODIGO;
            //    result.error.descripcion = error.DESCRIPCION;
            //    result.error.tipo = error.TIPO;
            //}

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
