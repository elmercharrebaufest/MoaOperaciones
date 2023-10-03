using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class FacturaService : IFacturaService
    {
        public FacturaService()
        {

        }
        public string SubirPDF(HttpPostedFileBase file, string folderPath)
        {
            try
            {
                file.SaveAs(folderPath + Path.GetFileName(file.FileName));
                return "El archivo se ha subido correctamente";
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch (InfoCustomException e)
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
