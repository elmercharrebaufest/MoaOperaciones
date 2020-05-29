using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago.Detalle
{
    public class PagoDetalleWSMOAResponse
    {
        public string pago { get; set; }
        public List<Cabecera> cabeceras { get; set; }
        public List<Salida> salidas { get; set; }
        public List<SalidaExcelDetalle> salidasExcelDetalle { get; set; }
        public List<Vendedor> vendedores { get; set; }
        public decimal subtotalIvaCorredor { get; set; }
        public decimal subtotalIvaVendedor { get; set; }
        public string subtotalIvaCorredorString { get; set; }
        public string subtotalIvaVendedorString { get; set; }
        public decimal subtotalMercCorredor { get; set; }
        public decimal subtotalMercVendedor { get; set; }
        public string subtotalMercCorredorString { get; set; }
        public string subtotalMercVendedorString { get; set; }

        public PagoDetalleWSMOAResponse()
        {
            this.cabeceras = new List<Cabecera>() { };
            this.salidas = new List<Salida>() { };
            this.vendedores = new List<Vendedor>() { };
        }
    }

    public class PagoDetalleExcelWSMOAResponse
    {
        public string pago { get; set; }
        public List<Cabecera> cabeceras { get; set; }
        public List<Salida> salidas { get; set; }
        public List<SalidaExcelDetalle> salidasExcelDetalle { get; set; }
        public List<Vendedor> vendedores { get; set; }

        public PagoDetalleExcelWSMOAResponse()
        {
            this.cabeceras = new List<Cabecera>() { };
            this.salidas = new List<Salida>() { };
            this.vendedores = new List<Vendedor>() { };
        }
    }
}
