using System;
using System.Collections.Generic;
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

        public int LocalidadId { get; set; }
        [ForeignKey(nameof(LocalidadId))]
        public virtual Localidad Localidad { get; set; }

        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaEgreso { get; set; }
        public string PlantaCodigo { get; set; }
        public string DomicilioTipo { get; set; }
        public short? DomicilioOrden { get; set; }
        public string DomicilioDescr { get; set; }
        public string MotivoRechazo { get; set; }

        public int AlmacenId { get; set; }
        [ForeignKey(nameof(AlmacenId))]
        public virtual Almacen Almacen { get; set; }

        public DateTime FechaVencimiento (List<DateTime> feriados)
        {
                var dayOfWeek = FechaCreacion.DayOfWeek;
                var cantidadDiasDeMargen = (dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Thursday) ? 5 : 3;

                var fechaFinal = FechaCreacion.AddDays(cantidadDiasDeMargen);

                foreach (var fechaFeriado in feriados)
                {
                    if (fechaFeriado.DayOfWeek != DayOfWeek.Saturday &&
                        fechaFeriado.DayOfWeek != DayOfWeek.Sunday &&
                        fechaFeriado.Date >= FechaCreacion &&
                        fechaFeriado.Date <= fechaFinal)
                    {
                        cantidadDiasDeMargen++;
                    }
                }

                return FechaCreacion.AddDays(cantidadDiasDeMargen);
        } 
    }
}
