using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class CampoCosechaNormativa
    {
        [Key]
        public int Id { get; set; }
        public int CampoCosecha_Id { get; set; }
        [ForeignKey("CampoCosecha_Id")]
        public virtual CampoCosecha CampoCosecha { get; set; }
        public int TipoNormativa_Id { get; set; }
        [ForeignKey("TipoNormativa_Id")]
        public virtual TipoNormativa TipoNormativa { get; set; }
        public float ToneladasAprobadas { get; set; }   
        public string MotivoRechazo { get; set; }
        public bool Validado { get; set; }
        public int ValidadoPor { get; set; }
        public DateTime ValidadoFecha { get; set; }
    }
}