using System;

namespace SustitucionMOAModel.Dto
{
    public class PeticionDeOfertaRevisionTecnicaDto
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime Fecha { get; set; }
        public bool RecotizacionEconomica { get; set; }
        public bool? ModificacionSolp { get; set; }
        public string ObservacionRecotizacion { get; set; }
        public bool Finalizada { get; set; }
    }
}
