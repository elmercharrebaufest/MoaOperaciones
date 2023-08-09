//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class PeticionDeOfertaSolpPosicion
    {
        [Key]
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int SolpPosicion_Id { get; set; }    

        [ForeignKey("PeticionDeOferta_Id")]
        public virtual PeticionDeOferta PeticionDeOferta { get; set; }

        [ForeignKey("SolpPosicion_Id")]
        public virtual SolpPosicion SolpPosicion { get; set; }

    }
}
