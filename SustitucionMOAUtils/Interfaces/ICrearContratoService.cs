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
    }
}
