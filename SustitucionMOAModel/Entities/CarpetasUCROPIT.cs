using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class CarpetasUCROPIT
    {
        [Key]
        public int Id { get; set; }
        public int Cosecha_Id { get; set; }
        [ForeignKey("Cosecha_Id")]
        public virtual Cosecha Cosecha { get; set; }
        public bool EPA { get; set; }
        public bool EUDER { get; set; }
        public bool BSVS2 { get; set; }
        public string UrlSubida { get; set; }
    }
}
