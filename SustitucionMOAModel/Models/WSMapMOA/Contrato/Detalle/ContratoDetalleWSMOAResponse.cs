using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class ContratoDetalleWSMOAResponse
    {
        public string contrato { get; set; }

        public Boleto boleto { get; set; }

        public string error { get; set; }

        public List<AmpliacionAnulacionView> ampliacionesAnulaciones { get; set; }

        public List<AplicacionView> aplicaciones { get; set; }

        public List<Calidad> calidad { get; set; }

        public List<CalidadExcelDetalle> calidadExcelDetalle { get; set; }

        public List<CaracteristicaView> caracteristicas { get; set; }

        public List<string> condicionesPago { get; set; }

        public List<FijacionView> fijaciones { get; set; }

        public List<HijoView> hijos { get; set; }

        public List<LiquidacionView> liquidaciones { get; set; }

        public List<PagoView> pagos { get; set; }

        public List<ResumenView> resumen { get; set; }

        public decimal aplicacionesTotalBrutos { get; set; }

        public string aplicacionesTotalBrutosUnidad { get; set; }

        public string aplicacionesTotalBrutosString { get; set; }

        public decimal aplicacionesTotalAplicados { get; set; }

        public string aplicacionesTotalAplicadosUnidad { get; set; }

        public string aplicacionesTotalAplicadosString { get; set; }

        public decimal calidadTotalNetos { get; set; }

        public string calidadTotalNetosUnidad { get; set; }

        public string calidadTotalNetosString { get; set; }

        public decimal calidadTotalAplicados { get; set; }

        public string calidadTotalAplicadosUnidad { get; set; }

        public string calidadTotalAplicadosString { get; set; }

        public ContratoDetalleWSMOAResponse()
        {
            this.ampliacionesAnulaciones = new List<AmpliacionAnulacionView>() { };
            this.aplicaciones = new List<AplicacionView>() { };
            this.calidad = new List<Calidad>() { };
            this.caracteristicas = new List<CaracteristicaView>() { };
            this.condicionesPago = new List<string>() { };
            this.fijaciones = new List<FijacionView>() { };
            this.hijos = new List<HijoView>() { };
            this.liquidaciones = new List<LiquidacionView>() { };
            this.pagos = new List<PagoView>() { };
            this.resumen = new List<ResumenView>() { };

        }
    }

    public class ContratoDetalleExcelWSMOAResponse
    {
        public string contrato { get; set; }

        public Boleto boleto { get; set; }

        public string error { get; set; }

        public List<AmpliacionAnulacion> ampliacionesAnulaciones { get; set; }

        public List<Aplicacion> aplicaciones { get; set; }

        public List<Calidad> calidad { get; set; }

        public List<CalidadExcelDetalle> calidadExcelDetalle { get; set; }

        public List<Caracteristica> caracteristicas { get; set; }

        public List<string> condicionesPago { get; set; }

        public List<Fijacion> fijaciones { get; set; }

        public List<Hijo> hijos { get; set; }

        public List<Liquidacion> liquidaciones { get; set; }

        public List<Pago> pagos { get; set; }

        public List<Resumen> resumen { get; set; }

        public decimal aplicacionesTotalBrutos { get; set; }

        public string aplicacionesTotalBrutosUnidad { get; set; }

        public string aplicacionesTotalBrutosString { get; set; }

        public decimal aplicacionesTotalAplicados { get; set; }

        public string aplicacionesTotalAplicadosUnidad { get; set; }

        public string aplicacionesTotalAplicadosString { get; set; }

        public decimal calidadTotalNetos { get; set; }

        public string calidadTotalNetosUnidad { get; set; }

        public string calidadTotalNetosString { get; set; }

        public decimal calidadTotalAplicados { get; set; }

        public string calidadTotalAplicadosUnidad { get; set; }

        public string calidadTotalAplicadosString { get; set; }

        public ContratoDetalleExcelWSMOAResponse()
        {
            this.ampliacionesAnulaciones = new List<AmpliacionAnulacion>() { };
            this.aplicaciones = new List<Aplicacion>() { };
            this.calidad = new List<Calidad>() { };
            this.caracteristicas = new List<Caracteristica>() { };
            this.condicionesPago = new List<string>() { };
            this.fijaciones = new List<Fijacion>() { };
            this.hijos = new List<Hijo>() { };
            this.liquidaciones = new List<Liquidacion>() { };
            this.pagos = new List<Pago>() { };
            this.resumen = new List<Resumen>() { };

        }
    }

    public class ContratoDetallePDFWSMOAResponse
    {
        public List<CalidadPDFDetalle> calidad { get; set; }

        public ContratoDetallePDFWSMOAResponse()
        {
            this.calidad = new List<CalidadPDFDetalle>() { };
        }
    }
}
