using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle
{
    public class CartaPorteDetalleWSMOAResponse
    {
        public string ccpp { get; set; }

        public ErrorWS error { get; set; }

        public List<Calidad> datosCalidad { get; set; }

        public List<AplicacionView> aplicaciones { get; set; }

        public List<EntregaDescargaView> entregasDescargas { get; set; }

        public decimal aplicacionesTotalAplicados { get; set; }

        public string aplicacionesTotalAplicadosUnidad { get; set; }

        public string aplicacionesTotalAplicadosString { get; set; }

        public decimal NetoDescontadoTotal { get; set; }

        public decimal aplicacionesTotalExcedentes { get; set; }

        public string aplicacionesTotalExcedentesUnidad { get; set; }

        public string aplicacionesTotalExcedentesString { get; set; }

        public decimal calidadTotalNetos { get; set; }
        public decimal calidadTotalNetosDescontados { get; set; }
        public string calidadTotalNetosDescontadosString { get; set; }

        public string calidadTotalNetosUnidad { get; set; }

        public string calidadTotalNetosString { get; set; }

        public decimal calidadTotalAplicados { get; set; }

        public string calidadTotalAplicadosUnidad { get; set; }

        public string calidadTotalAplicadosString { get; set; }

        public string camaraAPresent { get; set; }

        public CartaPorteDetalleWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.datosCalidad = new List<Calidad>() { };
            this.aplicaciones = new List<AplicacionView>() { };
            this.entregasDescargas = new List<EntregaDescargaView>() { };
        }
    }

    public class CartaPorteDetalleExcelWSMOAResponse
    {
        public string ccpp { get; set; }

        public ErrorWS error { get; set; }

        public List<Calidad> datosCalidad { get; set; }

        public List<Aplicacion> aplicaciones { get; set; }

        public List<EntregaDescarga> entregasDescargas { get; set; }

        public decimal aplicacionesTotalAplicados { get; set; }

        public string aplicacionesTotalAplicadosUnidad { get; set; }

        public string aplicacionesTotalAplicadosString { get; set; }

        public decimal NetoDescontadoTotal { get; set; }

        public decimal aplicacionesTotalExcedentes { get; set; }

        public string aplicacionesTotalExcedentesUnidad { get; set; }

        public string aplicacionesTotalExcedentesString { get; set; }

        public decimal calidadTotalNetos { get; set; }

        public string calidadTotalNetosUnidad { get; set; }

        public string calidadTotalNetosString { get; set; }

        public decimal calidadTotalAplicados { get; set; }

        public string calidadTotalAplicadosUnidad { get; set; }

        public string calidadTotalAplicadosString { get; set; }

        public CartaPorteDetalleExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.datosCalidad = new List<Calidad>() { };
            this.aplicaciones = new List<Aplicacion>() { };
            this.entregasDescargas = new List<EntregaDescarga>() { };
        }
    }

    public class CartaPorteDetallePDFWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<CalidadPDF> datosCalidad { get; set; }

        public CartaPorteDetallePDFWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.datosCalidad = new List<CalidadPDF>() { };
        }
    }
}
