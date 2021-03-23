using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class SubCategoria
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Nombre { get; set; }
        public int Categoria_Id { get; set; }

        [ForeignKey("Categoria_Id")]
        public virtual Categoria Categoria { get; set; }
    }
}
