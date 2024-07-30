using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioCabeceraDto
    {
        public int ID { get; set; }
        public string EntradaServicio { get; set; }
        public DateTime? FechaCreacionDateTime { get; set; }
        public string FechaCreacion { get; set; }
        public string OrdenCompra { get; set; }
        public string Proveedor { get; set; }
        public string Descripcion { get; set; }
        public string MontoTotal { get; set; }
        public string CUIT { get; set; }
        public List<EntradaServicioDetalleDto> entradaServicioDetalle { get; set; }
        public string Fiscal { get; set; }
        public string Area { get; set; }
        public string Suplente { get; set; }
        // DATOS PROVENIENTES DE TABLA APROBACIONES NECESARIOS PARA LA PANTALLA.
        public string Aprobador { get; set; }
        public string MotivoRechazo { get; set; }
        public string Estado { get; set; }
        public string NumeroCertificacion { get; set; }
        public string Ingresante { get; set; }
        public bool DesdeSap { get; set; }
        public string FechaAprobacion { get; set; }
        public string FechaRechazo { get; set; }
        public string Moneda { get; set; }
        public string FechaContabilizacion { get; set; }
        public string FechaDocumento { get; set; }
        public string NroPosicion { get; set; }
        public string AnuladaPor { get; set; }
        public EntradaServicioCabeceraDto()
        {
            // Inicializa la lista entradaServicioDetalle en el constructor
            entradaServicioDetalle = new List<EntradaServicioDetalleDto>();
        }
    }
}
