using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services.AnalisisDocumentoServiceValidation;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Services
{
    public class AnalisisDocumentoService : IAnalisisDocumentoService
    {
        private const string _cuitMoa = "30715118773";

        public AnalisisDocumentoService()
        {
        }

        public List<ValidationResult> AnalizarFacturaCertificacionServicios(List<string> documento, string cuitProveedor, string fileName)
        {
            var handler = new AnalisisDocumentoServiceValidationHandler();
            handler.AddValidation(new CuitValidationCommand(_cuitMoa));
            handler.AddValidation(new CuitValidationCommand(cuitProveedor));
            handler.AddValidation(new NumeroFacturaValidationCommand());
            handler.AddValidation(new CodigoFacturaValidationCommand());
            handler.AddValidation(new OrdenCompraValidationCommand());

            List<ValidationResult> results = handler.ExecuteValidations(documento);
            results.ForEach(a => a.FileName = fileName);

            return results;

        }

        public List<ValidationResult> AnalizarDNICampoSustentable(List<string> documento, string cuitProveedor, string fileName)
        {
            var handler = new AnalisisDocumentoServiceValidationHandler();

            handler.AddValidation(new DniValidationCommand(cuitProveedor));

            List<ValidationResult> results = handler.ExecuteValidations(documento);
            results.ForEach(a => a.FileName = fileName);

            return results;
        }

        public List<ValidationResult> AnalizarEstatutoCampoSustentable(List<string> documento, string cuitProveedor, string fileName)
        {
            var handler = new AnalisisDocumentoServiceValidationHandler();

            handler.AddValidation(new CuitValidationCommand(cuitProveedor));
            List<ValidationResult> results = handler.ExecuteValidations(documento);
            results.ForEach(a => a.FileName = fileName);

            return results;
        }



        public List<ValidationResult> AnalizarPoderCampoSustentable(List<string> documento, string cuitProveedor, string fileName)
        {
            return AnalizarEstatutoCampoSustentable(documento, cuitProveedor, fileName);
        }
    }
}
