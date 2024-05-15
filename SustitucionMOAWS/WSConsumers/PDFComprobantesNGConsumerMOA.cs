using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PDFComprobantesNGWebServiceMOA;
using SustitucionMOAWS.PDFWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class PDFComprobantesNGConsumerMOA
    {
        SI_MPMF_MOAOP_COMP_NOGRANOSPDFClient service = new SI_MPMF_MOAOP_COMP_NOGRANOSPDFClient();


        public PDFResponse request(string CodigoProveedorSAP, string FechaDocumento, string NumeroLegalDocumento)
        {
            try
            {
                byte[] pdf = new byte[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                pdf = service.SI_MPRFC_MOAOP_COMP_NOGRANOSPDF(FechaDocumento, CodigoProveedorSAP, NumeroLegalDocumento);
                return map(pdf);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private PDFResponse map(byte[] pdf)
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
                result.pdf = new Pdf()
                {
                    data = pdf
                };

            }

            return result;

        }
    }
}
