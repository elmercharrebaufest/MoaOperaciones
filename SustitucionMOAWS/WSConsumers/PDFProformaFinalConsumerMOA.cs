using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.PDFProformaFinalWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class PDFProformaFinalConsumerMOA
    {
        SI_MPMF_MOAOP_PDF_PROFORMAClient service = new SI_MPMF_MOAOP_PDF_PROFORMAClient();

        public PDFResponse request(string contrato, string pedido)
        {
            try
            {
                byte[] pdf = new byte[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                pdf = service.SI_MPMF_MOAOP_PDF_PROFORMA(contrato, pedido);
                return map(pdf);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private PDFResponse map(byte[] pdf)
        {
            PDFResponse result = new PDFResponse() { };

            if (pdf != null && pdf.Length > 0)
            {
                result.Pdf = new Pdf()
                {
                    Data = pdf
                };
            }

            return result;
            
        }
    }
}
