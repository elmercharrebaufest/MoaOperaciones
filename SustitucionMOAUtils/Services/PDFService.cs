using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.PDF;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;

namespace SustitucionMOAUtils.Services
{
    public class PDFService: IPDFService
    {
        public PDFService()
        {

        }
        public Pdf DescargarDocumentPDF(string documento, string ejercicio, string proveedor, string sociedad)
        {
            try
            {
                PDFResponse data = new PDFConsumerMOA().request(documento, ejercicio, proveedor, sociedad);

                if (data == null || data.Pdf == null)
                {
                    throw new InfoCustomException(InfoMsg.DocumentoNoExiste);
                }
                return data.Pdf;
            }
            catch (InfoCustomException e)
            {
                throw e;
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
        }
    }
}
