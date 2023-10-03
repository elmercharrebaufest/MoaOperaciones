using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOAModel.Models.ViewModel.Pago;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Comprobante;
using SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IPagoService
    {
        PagoViewModel ObtenerEmitidos(string proveedor, string fechaInicio, string fechaFin);
        PagoNGViewModel ObtenerEmitidosNG(string proveedor, string fechaInicio, string fechaFin, string sociedad);
        PagoViewModel ObtenerPagos(string proveedor, string tipo, string fechaInicio, string fechaFin);

        PagoNGViewModel ObtenerPagosNG(string proveedor, string tipo, string fechaInicio, string fechaFin, string sociedad);
        PagoComprobanteWSMOAResponse ObtenerComprobantes(string documento, string fecha, string sociedad, string fiscalYear);

        string DescargarEmitidos(string proveedor, string fechaInicio, string fechaFin);
        string DescargarEmitidosNG(string proveedor, string fechaInicio, string fechaFin, string sociedad);
        PagoDetalleWSMOAResponse ObtenerDetalle(string proveedor, string numeroPago);

        string DescargarDetalle(string proveedor, string numeroPago);
    }
}
