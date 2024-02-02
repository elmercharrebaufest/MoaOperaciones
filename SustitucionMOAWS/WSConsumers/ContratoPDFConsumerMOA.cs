using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.ContratoPDFWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class ContratoPDFConsumerMOA
    {
        SI_MPMF_MOAOP_PDF_CONTRATOClient service = new SI_MPMF_MOAOP_PDF_CONTRATOClient();

        public PDFResponse request(string proveedor, string contrato)
        {
            try
            {
                byte[] pdf = new byte[] { };
                ITCOO[] itcoo = new ITCOO[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_PDF_CONTRATO(contrato, proveedor, out itcoo, out pdf);
                return map(pdf, error);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private PDFResponse map(byte[] pdf, ZMPES4910 error)
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
                    Data = pdf
                };
            }

            return result;
            
        }
    }
}
