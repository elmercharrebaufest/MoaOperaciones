using DocumentFormat.OpenXml.Spreadsheet;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.CampoSustentable;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICampoSustentableService
    {
        string AdjuntarDeclaracionFirmada(string mailUsuario, int proveedorId, int cosechaId, string CUITDeclaracion, HttpPostedFileBase fileSubido);
        Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz, bool UsarArchivoId, HttpPostedFileBase archivoEPA);
        string Borrar(string mailUsuario, int campoCosechaId, int proveedorId);
        string Rechazar(string mailUsuario, int campoCosechaId, int proveedorId, int tipoNormativaId, string motivoRechazo);
        string Aprobar(string mailUsuario, int campoCosechaId, int proveedorId, int tipoNormativaId);
        Resultado Editar(string mailUsuario, CampoProveedor campoProveedorObj, HttpPostedFileBase archivoKmz, HttpPostedFileBase archivoEPA);
        string ExportarCamposProveedores(string mailUsuario);
        byte[] GenerarDeclaracionProveedor(string mailUsuario, int proveedorId, int cosechaId, double hectareasTotales, string CUITDeclaracion, string razonSocialDeclaracion);
        byte[] ImprimirDeclaracion(int proveedorId, int cosechaId, string CUIT);
        List<CampoProveedorListadoDto> Listar(string mailUsuario);
        CampoProveedorDto ObtenerCampo(string mailUsuario, int proveedorId, int campoCosechaId);
        List<Cosecha> ObtenerCosechas(bool incluirInactivas);
        EstadoDeclaracionSustentableDto VerificarDeclaracion(int proveedorId, int cosechaId, string CUITDeclaracion);
        string ObtenerRutaArchivoKMZ(int campoCvosechaId, int proveedorId);
        Task DescargarArchivosDeGoogleDrive(ArchivoCampoSustentable archivoSinDescargar);
        bool RenspaExiste(string renspa, string cuit, int cosechaId, bool epa, bool bsvs2, bool eudr);
        List<SugerenciaCampoDto> ObtenerSugerenciaCamposNuevaCosecha(int proveedorId, int cosechaId, string cuitTitularCP);
        void AgregarCamposSugeridos(List<SugerenciaCampoDto> camposProveedorDto, List<HttpPostedFileBase> archivosKmz, List<HttpPostedFileBase> archivosEPA, string mailUsuario);
        string ExportarCamposSugeridos(int proveedorId, int cosechaId, string cuitTitularCP);
        List<TipoNormativa> ObtenerNormativas();
        string ObtenerRutaArchivoEPA(int campoCosechaId, int proveedorId);
        string AdjuntarEPAValidado(string mailUsuario, int campoCosechaId, int proveedorId, HttpPostedFileBase archivoEPA);
    }
}
