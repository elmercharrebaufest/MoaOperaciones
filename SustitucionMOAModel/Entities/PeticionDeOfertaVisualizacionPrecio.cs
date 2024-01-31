using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOfertaVisualizacionPrecio
    {
        [Key]
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int? Archivo_Id { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Observaciones { get; set; }
       

        [ForeignKey("UsuarioCreador_Id")]
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }

        [ForeignKey("PeticionDeOferta_Id")]
        public virtual PeticionDeOferta PeticionDeOferta { get; set; }

    }
}
