using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOfertaRevisionTecnica
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime Fecha { get; set; }
        public bool RecotizacionEconomica { get; set; }
        public bool? ModificacionSolp { get; set; }
        public string ObservacionRecotizacion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public bool Finalizada { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}