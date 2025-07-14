using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class RelacionSolicitanteExternoJefeMoa
    {
        public int ID { get; set; }
        public int Usuario_Id { get; set; }
        public int JefeMoa_Id { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
        [ForeignKey("JefeMoa_Id")]
        public virtual Usuario JefeMoa { get; set; }
    }
}
