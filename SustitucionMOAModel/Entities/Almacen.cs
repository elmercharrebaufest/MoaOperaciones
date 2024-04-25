using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Almacen
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        [InverseProperty("Almacenes")]
        public virtual ICollection<Material> Materiales { get; set; }
    }
}
