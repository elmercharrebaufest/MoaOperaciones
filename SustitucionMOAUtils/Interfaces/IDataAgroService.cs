using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.DataAgro;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IDataAgroService
    {
        DataAgroAuthWSMOAResponse goToDataAgro(string proveedor, string nombre);
        bool ValidarCUITProveedorGranos(ref UsuarioGranos usuario, SustitucionMOAModel.Entities.Proveedor proveedor);
        string ObtenerCBUProveedor(string CUITproveedor);
        SustitucionMOAWS.DataAgroServices.ResultadoValidarProveedorComercial ObtenerValidarCUITProveedorGranos(string CUIT, bool? corredor = false);
        void ValidarNuevoProveedorMultifirma(ref SustitucionMOAModel.Entities.Proveedor nuevoProveedor);
        string VerificarEstadoProveedor(int proveedorID, string usuarioMail);
        bool ProveedorApocrifo(string CUIT);
        decimal TraerTipoDeCambio();
        SustitucionMOAWS.DataAgroServices.ResultadoAltaCampoSustentable AltaCampoSustentable(CampoProveedor campo, string kmz);
        List<EstadoProveedorDto> ObtenerEstadoProveedores(string[] cuits);
        SustitucionMOAWS.DataAgroServices.DatosIniContrato InicializarContrato(int tipoNegocioId);
        SustitucionMOAWS.DataAgroServices.DatosCompraNetDto ObtenerDatosCompraNet(int id);
        SustitucionMOAWS.DataAgroServices.DatosFijacionDeContratoDto[] ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual);
        SustitucionMOAWS.DataAgroServices.AltaTempranaNRCODto ValidarProveedor(int proveedorId);        
    }
}
