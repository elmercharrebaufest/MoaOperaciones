using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.PDFWebServiceMOA;

namespace SustitucionMOAWS.WSConsumers
{
    public class PDFConsumerMOA
    {
        SI_MPMF_MOAOP_PDFClient service = new SI_MPMF_MOAOP_PDFClient();

        public PDFResponse request(string documento, string ejercicio, string proveedor, string sociedad)
        {
            try
            {
                byte[] pdf = new byte[] { };
                service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
                ZMPES4910 error = service.SI_MPMF_MOAOP_PDF(documento, ejercicio, proveedor, sociedad, out pdf);
                return map(pdf, error);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private PDFResponse map(byte[] pdf, ZMPES4910 error)
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
                    Data = pdf
                };

            }

            return result;

        }
    }
}
