using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Nombre { get; set; }

        [InverseProperty("Categorias")]
        public virtual ICollection<Rol> Roles { get; set; }

    }
}
