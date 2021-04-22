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
        Resultado Agregar(string mailUsuario, CampoProveedor campoProveedor, HttpPostedFileBase archivoKmz);
        string Borrar(string mailUsuario, int campoCosechaId, int proveedorId);
        Resultado Editar(string mailUsuario, CampoProveedor campoProveedorObj, HttpPostedFileBase archivoKmz);
        List<CampoProveedorListadoDto> Listar(string mailUsuario);
        CampoProveedorDto ObtenerCampo(string mailUsuario, int proveedorId, int campoCosechaId);
        List<Cosecha> ObtenerCosechas();
        string FirmarDeclaracion(string mailUsuario, int proveedorId, double hectareasTotales, int cosechaId);
        byte[] GenerarDeclaracionProveedor(string mailUsuario, int proveedorId, int cosechaId, double hectareasTotales);
        EstadoDeclaracionSustentableDto VerificarDeclaracion(int proveedorId, int cosechaId);
        byte[] ImprimirDeclaracion(int proveedorId, int cosechaId);
        string AdjuntarDeclaracionFirmada(string mailUsuario, int proveedorId, int cosechaId, HttpPostedFileBase fileSubido);
    }
}
