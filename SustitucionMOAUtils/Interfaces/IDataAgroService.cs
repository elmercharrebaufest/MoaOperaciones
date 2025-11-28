using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.DataAgro;
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
        string InicializarContrato(int tipoNegocioId);
        string ObtenerDatosCompraNet(int id);
        string ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId, bool esVirtual);
        string ValidarProveedor(int proveedorId);
        string HabilitarPizarra(int material, int tiponegocio);
        string TraerPagosDiferido();
        string HabilitarCampaña(int material);
        string TraerPrecioMoaV2(int material, int tipoNegocio);
        string TraerPrecioMoa(int tipoNegocio);
        string AnularNegocio(int negocioId, int tipoNegocioId, string motivoRechazo);
        string HabilitarSustentable();
        string BuscarProveedoresConCorredor(string filtroProveedor, string filtro, int? agenteCompraId);
        List<MaterialDto> BuscarMateriales();
        string CrearContratoAPrecio(ContratoAPrecio contrato);
        string CrearContratoAFijar(ContratoAFijar contrato);
        string ValidarDirecto(string cuit);
        string GrabarFijacion(ContratoFijacion contratoFijacion);
        string ObteneContratosAcuerdo(int corredorId);
        List<GrabarContratoResult> CrearContratoMasivo(List<BasicoContrato> contratos);
        string TraerContratoCompleto(int negocioId, int tipoNegocioId);
    }
}
