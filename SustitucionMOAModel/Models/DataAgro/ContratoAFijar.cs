using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class ContratoAFijar
    {
        public int CampañaID { get; set; }
        public int TipoNegocioId { get; set; }
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioNeto { get; set; }
        public int CampanaId { get; set; }
        public string MonedaId { get; set; }
        public int ComercialId { get; set; }
        public string ContratoSAP { get; set; }
        public int PorcentajeDePago { get; set; }
        public int? ComercialCreadorId { get; set; }
        public int EstadoId { get; set; }
        public int ClasificacionId { get; set; }
        public string Observacion { get; set; }
        public int DestinoId { get; set; }
        public int BoletoId { get; set; }
        public int BolsaId { get; set; }
        public int ProveedorId { get; set; }
        public int? CorredorId { get; set; }
        public int LocalidadId { get; set; }
        public int ProvinciaId { get; set; }
        public DateTime FechaOperacion { get; set; }
        public DateTime DesdeFijacion { get; set; }
        public DateTime HastaFijacion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaHasta { get; set; }
        public int ProveedorCreadorId { get; set; }
        public int StandardDeCalidadId { get; set; }
        public int? CondicionFijacionId { get; set; }
        public int? CantidadCamiones { get; set; }
        public int? ImporteSustentable { get; set; }
        public string MonedaSustentable { get; set; }
        public bool? EstablecimientoPropio { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public int? ZonaId { get; set; }

        public string ObservacionTercero { get; set; }
        public bool? CalidadTercero { get; set; }
        public bool? SustentableTercero { get; set; }
        public string ContratoCorredor { get; set; }
        public string ContratoVendedor { get; set; }

    }

}
