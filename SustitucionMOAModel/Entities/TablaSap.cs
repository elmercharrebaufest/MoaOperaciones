using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class TablaSap
    {
        [Key]
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }
        public int? Padre_Id { get; set; }

        [ForeignKey("Padre_Id")]
        public TablaSap Padre { get; set; }
        //public virtual ICollection<TablaSap> Hijos { get; set; }
    }
}
