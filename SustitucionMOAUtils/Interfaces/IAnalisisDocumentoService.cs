using SustitucionMOAModel.Models;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAnalisisDocumentoService
    {
        List<ValidationResult> AnalizarFacturaCertificacionServicios(List<string> documento, string cuitProveedor, string fileName);
        List<ValidationResult> AnalizarPoderCampoSustentable(List<string> documento, string cuitProveedor, string fileName);
        List<ValidationResult> AnalizarEstatutoCampoSustentable(List<string> documento, string cuitProveedor, string fileName);
        List<ValidationResult> AnalizarDNICampoSustentable(List<string> documento, string cuitProveedor, string fileName);
    }
}
