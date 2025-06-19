using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Logger;
using SustitucionMOAWS.PDFComprobantesNGWebServiceMOA;
using SustitucionMOAWS.PDFWebServiceMOA;
using SustitucionMOAWS.ScatoComandosWebService;
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
                    string FECHA_DOCUMENTO = FechaDocumento;
                    if (FECHA_DOCUMENTO.Length < 10)
                    {
                        var FECHA_DOCUMENTO_SPLIT = FECHA_DOCUMENTO.Split('-');
                        if (FECHA_DOCUMENTO_SPLIT.Length == 3)
                        {
                            string ANIO = FECHA_DOCUMENTO_SPLIT[0];
                            string MES = Convert.ToInt32(FECHA_DOCUMENTO_SPLIT[1]) < 10 ? "0" + FECHA_DOCUMENTO_SPLIT[1] : FECHA_DOCUMENTO_SPLIT[1];
                            string DIA = Convert.ToInt32(FECHA_DOCUMENTO_SPLIT[2]) < 10 ? "0" + FECHA_DOCUMENTO_SPLIT[2] : FECHA_DOCUMENTO_SPLIT[2];
                            FECHA_DOCUMENTO = string.Format("{0}-{1}-{2}", ANIO, MES, DIA);
                        }
                    }
                    var request = new Z_MPRFC_MOAOP_COMP_NOGRANOSPDF()
                    {
                        IM_BLDAT = FECHA_DOCUMENTO,
                        IM_LIFNR = CodigoProveedorSAP,
                        IM_XBLNR = NumeroLegalDocumento
                    };
                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_COMP_NOGRANOSPDF request");
                    Log.Info(request.ToXml());

                    var response = agent.Z_MPRFC_MOAOP_COMP_NOGRANOSPDF(request);
                    Log.Info($"SAP sin PI Z_MPRFC_MOAOP_COMP_NOGRANOSPDF response");
                    Log.Info(response.ToXml());

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
