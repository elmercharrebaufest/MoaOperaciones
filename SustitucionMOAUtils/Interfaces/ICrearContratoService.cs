using Kendo.DynamicLinq;
using SustitucionMOAModel.Models.DataAgro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ICrearContratoService
    {
        string ObteneDatosContrato(int tiponegocio);
        string CrearContratoAPrecio(ContratoAPrecio contratoAPrecio);
        string CrearContratoAFijar(ContratoAFijar contratoAPrecio);
        string ObtenerDatosCompraNet(int proveedorId);
        string ValidarDirecto(string cuit);
        string BuscarProveedoresConCorredor(string filtro, string cuitCorredor);
        string HabilitarPizarra(int material, int tiponegocio);
        string HabilitarCampaña(int material);
        string TraerPrecioMoa(int material, int tiponegocio);
        string ObtenerFijacionesAutomaticas(string cuitProveedor, string cuitCorredor, int materialId, string filtro, int fijacionId);
        string CrearContratoFijacion(ContratoFijacion contratoFijacion);
        string GetContratos(DataSourceRequest request);
        string ValidarProveedor(string proveedorId);
        string TraerPrecioMoaMateriales(int tipoNegocioId);
        string AnularNegocio(int negocioId, int tipoNegocioId, string motivo);
        string TraerContratoCompleto(int negocioId, int tipoNegocioId);
        string TraerPagosDiferido(int material, int tiponegocio);
        List<MaterialDto> BuscarMateriales();
        List<CentroDto> BuscarCentros();
        List<CampaniaDto> BuscarCampanias();
        string ObteneContratosAcuerdo(int idDataAgro);
        BasicoContrato TraerContratoCompleto(int id, string tipo);
        List<GrabarContratoResult> CrearContratoMasivo(List<BasicoContrato> contratos);
        string ConfiguracionBolsaAutomatica();
        byte[] ExcelModeloAltaMasiva();
        string TraerHabilitarSustentable();
    }
}
