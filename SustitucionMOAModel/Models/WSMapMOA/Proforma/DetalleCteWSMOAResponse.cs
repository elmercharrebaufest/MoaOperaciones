using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Proforma
{
    public class DetalleCteWSMOAResponse
    {
        public List<SalidaView> salidas { get; set; }
        public SalidaView pagoACuenta { get; set; }
        public SalidaView subTotal { get; set; }
        public SalidaView saldoAPagar { get; set; }
        public string fijacion { get; set; }
        public string vendedores { get; set; }
        public CabeceraView cabecera { get; set; }
        public string error { get; set; }
        public bool LiquidacionParcialEmitida { get; set; }
        public bool CumpleEscenario1 { get; set; }

        public DetalleCteWSMOAResponse()
        {
            this.salidas = new List<SalidaView>() { };
            this.subTotal = new SalidaView()
            {
                caracteristica = "Subtotal",
                importeString = "$0,00",
                importe = 0,
                ivaString = "$0,00",
                iva = 0,
                totalString = "$0,00",
                total = 0
            };
            this.saldoAPagar = new SalidaView()
            {
                caracteristica = "Saldo a Pagar - Liquidación Final",
                importeString = "$0,00",
                importe = 0,
                ivaString = "$0,00",
                iva = 0,
                totalString = "$0,00",
                total = 0
            };
        }
    }

    public class DetalleCteExcelWSMOAResponse
    {
        public List<Salida> salidas { get; set; }
        public Salida pagoACuenta { get; set; }
        public Salida subTotal { get; set; }
        public Salida saldoAPagar { get; set; }
        public string fijacion { get; set; }
        public string vendedores { get; set; }
        public Cabecera cabecera { get; set; }
        public string error { get; set; }

        public DetalleCteExcelWSMOAResponse()
        {
            this.salidas = new List<Salida>() { };
            this.subTotal = new Salida()
            {
                caracteristica = "Subtotal",
                importe = 0,
                iva = 0,
                total = 0
            };
            this.saldoAPagar = new Salida()
            {
                caracteristica = "Saldo a Pagar - Liquidación Final",
                importe = 0,
                iva = 0,
                total = 0
            };
        }
    }
}
