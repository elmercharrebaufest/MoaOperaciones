using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class OrdenResiduos
    {
        [Key]
        public long Id { get; set; }

        public int ClienteId { get; set; }
        [ForeignKey(nameof(ClienteId))]
        public virtual Proveedor Cliente { get; set; }

        public int MaterialId { get; set; }
        [ForeignKey(nameof(MaterialId))]
        public virtual Material Producto { get; set; }

        public int EstadoId { get; set; }
        [ForeignKey(nameof(EstadoId))]
        public virtual EstadoOrdenResiduos Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public string PatenteChasis { get; set; }
        public string PatenteAcoplado { get; set; }
        public string ChoferNombre { get; set; }
        public string ChoferApellido { get; set; }
        public string ChoferCuil { get; set; }
        public string TransporteRazonSocial { get; set; }
        public string TransporteCuit { get; set; }
        public string Observacion { get; set; }

        public int? LocalidadId { get; set; }

        public string LocalidadDescripcion { get; set; }

        public int? ProvinciaId { get; set; }

        public string ProvinciaDescripcion { get; set; }

        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public string MotivoRechazo { get; set; }
        public string Balanza { get; set; }
        public string NroCertificacion { get; set; }
        public double? PesadaTara { get; set; }
        public double? PesadaNeto { get; set; }
        public double? PesadaBruto { get; set; }
        public string KmsARecorrer { get; set; }
        public long? IdScato { get; set; }
        public string UniMedCant { get; set; }
        public int AlmacenId { get; set; }
        [ForeignKey(nameof(AlmacenId))]
        public virtual Almacen Almacen { get; set; }
    }
}
