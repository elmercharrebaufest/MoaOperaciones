using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOfertaArchivo
    {
        [Key]
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int Archivo_Id { get; set; }
        public DateTime Fecha { get; set; }       

        [ForeignKey("PeticionDeOferta_Id")]
        public virtual PeticionDeOferta PeticionDeOferta { get; set; }

        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }

    }
}
