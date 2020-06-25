using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICartaPorteService
    {
        string DownloadAplicaciones(string proveedor, string fechaInicio, string fechaFin);
        string DownloadDescargas(string proveedor, string fechaInicio, string fechaFin);
        string DownloadDetalle(string proveedor, string cartaporteId);
        Pdf DownloadPDFCalidad(string proveedor, string cartaporteId);
        CartaPorteViewModel GetAplicaciones(string proveedor, string fechaInicio, string fechaFin);
        List<CartaPorteFoto> GetFotos(string cartaPorteId);
        List<CartaPorteFoto> GetFotos(List<string> cartaPorteId);
        byte[] GetCompletedPDFTemplate(CCPPFormulario formulario, int paginaSeleccionada, byte[] archivoBytes);
        CartaPorteCTGWSMOAResponse GetDataCTG(string valor);
        CartaPorteDescargaViewModel GetDescargas(string proveedor, string fechaInicio, string fechaFin);
        CartaPorteDetalleWSMOAResponse GetDetalle(string proveedor, string cartaporteId);
        CartaPorteFormularioDropdownsWSMOAResponse GetFormularioDropdowns();
        byte[] GetTemplate(CCPPFormulario formulario);
    }
}