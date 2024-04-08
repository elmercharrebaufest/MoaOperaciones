using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class OrdenResiduos
    {
        [Key]
        public long Id { get; set; }

        public int? CorredorId { get; set; }
        [ForeignKey("CorredorId")]
        public virtual Proveedor Corredor { get; set; }

        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public virtual Proveedor Cliente { get; set; }

        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Producto { get; set; }

        public int EstadoId { get; set; }
        [ForeignKey("EstadoId")]
        public virtual EstadoOrdenResiduos Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaRetiro { get; set; }
        public int Cantidad { get; set; }
        public string PatenteChasis { get; set; }
        public string PatenteAcoplado { get; set; }
        public string ChoferNombre { get; set; }
        public string ChoferCuil { get; set; }
        public string TransporteRazonSocial { get; set; }
        public string TransporteCuit { get; set; }
        public string Observacion { get; set; }

        public int LocalidadId { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }

        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public int DistanciaKm { get; set; }
        public string IntermediarioFleteCuit { get; set; }
        public string IntermediarioFleteRazonSocial { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public string MotivoRechazo { get; set; }

        public string DescripcionMercaderia()
        {

            try
            {
                var split = Producto.Nombre.Split('-');

                return split.LastOrDefault()?.Trim();
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
