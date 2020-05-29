using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;

namespace SustitucionMOAValidator
{
    public static class InputValidator
    {
        private static List<string> _FORMATOSACEPTADOS = new List<string>() { ".pdf", ".doc", ".docx", ".dotx", ".xls", ".ppt", ".pptx" };
        private static string _FORMATOPDF = ".pdf";
        private static int _MAXFILEATTACHED = 3145728;
        private static int _MAXFILEFORMULARIOCCPP = 3 * 3145728;
        private static string _REGEXEMAIL = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

        public static void rangoFechas(DateTime fechaIncio, DateTime fechaFin) {
            if (fechaIncio > fechaFin)
                throw new ValidationCustomException(ErrorMsg.ErrorRangoFechas);
        }

        public static void notEmptyOrNull(string valor, string label) {
            if (valor == "" || valor == null) {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
            }

        }

        public static string notEmptyOrNullString(string valor, string label)
        {
            try
            {
                string valorOk = valor.Trim();
                if (valorOk.Equals(""))
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
                }
                else
                {
                    return valorOk;
                }
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
            }

        }

        public static string validarCampoFecha(string valor, string label)
        {
            try
            {
                string valorOk = valor.Trim();
                if (valorOk.Equals(""))
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
                }
                else
                {
                    return valorOk;
                }
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
            }
        }

        public static decimal validarCampoDecimal(string valor, string label)
        {
            try
            {
                string valorOk = valor.Trim();
                if (valorOk.Equals(""))
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
                }
                else
                {
                    try {
                        return Convert.ToDecimal(valorOk);
                    } catch {
                        throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, label));
                    }
                }
            }
            catch (ValidationCustomException e) {
                throw e;
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
            }
        }

        public static string validarEmail(string valor, string label)
        {
            try
            {
                string valorOk = valor.Trim();
                if (valorOk.Equals(""))
                {
                    throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
                }
                else
                {
                    try
                    {
                        if (Regex.IsMatch(valor, _REGEXEMAIL)) {
                            return valor;
                        }
                        throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, label));
                    }
                    catch
                    {
                        throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorIncorrecto, label));
                    }
                }
            }
            catch (ValidationCustomException e)
            {
                throw e;
            }
            catch
            {
                throw new ValidationCustomException(String.Format(ErrorMsg.ErrorValorNuloVacio, label));
            }
        }

        public static void emailAttatchment(HttpPostedFileBase file) {

            if (file.ContentLength > _MAXFILEATTACHED) {
                throw new ValidationCustomException(ErrorMsg.ErrorArchivoTamanio);
            }

            try
            {
                if (!_FORMATOSACEPTADOS.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    throw new Exception();
                }
            }
            catch{
                throw new ValidationCustomException(ErrorMsg.ErrorArchivoFormato);
            }
        }

        public static void formularioPDF(HttpPostedFileBase file)
        {

            if (file.ContentLength > _MAXFILEFORMULARIOCCPP)
            {
                throw new ValidationCustomException(ErrorMsg.ErrorArchivoTamanio);
            }

            try
            {
                if (!_FORMATOPDF.Equals(Path.GetExtension(file.FileName).ToLower()))
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new ValidationCustomException(ErrorMsg.ErrorArchivoFormato);
            }
        }
    }
}
