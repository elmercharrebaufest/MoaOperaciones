using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICampoSustentableService
    {
        string AdjuntarDeclaracionFirmada(string mailUsuario, int proveedorId, int cosechaId, string CUITDeclaracion, HttpPostedFileBase fileSubido);
        Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz, bool UsarArchivoId);
        string Borrar(string mailUsuario, int campoCosechaId, int proveedorId);
        Resultado Editar(string mailUsuario, CampoProveedor campoProveedorObj, HttpPostedFileBase archivoKmz);
        string ExportarCamposProveedores(string mailUsuario);
        byte[] GenerarDeclaracionProveedor(string mailUsuario, int proveedorId, int cosechaId, double hectareasTotales, string CUITDeclaracion, string razonSocialDeclaracion);
        byte[] ImprimirDeclaracion(int proveedorId, int cosechaId, string CUIT);
        List<CampoProveedorListadoDto> Listar(string mailUsuario);
        CampoProveedorDto ObtenerCampo(string mailUsuario, int proveedorId, int campoCosechaId);
        List<Cosecha> ObtenerCosechas(bool incluirInactivas);
        EstadoDeclaracionSustentableDto VerificarDeclaracion(int proveedorId, int cosechaId, string CUITDeclaracion);
        string ObtenerRutaArchivoKMZ(int campoCosechaId, int proveedorId);
        Task DescargarArchivosDeGoogleDrive(ArchivoCampoSustentable archivoSinDescargar);
    }
}
