using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CotizacionDto
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public int CotizacionEstado_Id { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool? RespetaMateriales { get; set; }
        public bool? RespetaServicios { get; set; }
        public string ObservacionTecnica { get; set; }
        public string ObservacionEconomica { get; set; }
        public int Revision { get; set; }
        public string CotizacionEstadoDescripcion { get; set; }
    }
}
