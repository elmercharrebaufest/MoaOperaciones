using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CotizacionHistorial
    {
        [Key]
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public string Log { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public int Usuario_Id { get; set; }

        [ForeignKey("Cotizacion_Id")]
        public virtual Cotizacion Cotizacion { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

    }
}
