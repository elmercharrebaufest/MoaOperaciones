using System;

namespace SustitucionMOAModel.Dto
{
    public class LegajoDto
    {        
        public int SolpId { get; set; }
        public int PeticionDeOfertaId { get; set; }        
        public DateTime Fecha { get; set; }
        public string Observacion { get; set; }
        public int? UsuarioId { get; set; }
        public UsuarioDto Usuario { get; set; }
        public int? ArchivoId { get; set; }
        public string FechaFormateado { get; set; }
        public bool Leido { get; set; } = true;
        public string Tipo { get; set; }
        public int? PeticionDeOfertaUsuarioId { get; set; }
    }
}
