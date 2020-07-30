using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SustitucionMOAModel.Entities
{
    public class PermisoPorRol
    {
        [Key]
        public int Id { get; set; }
        public string Permiso { get; set; }

        public virtual ICollection<Rol> RolesAsociados { get; set; }
    }
}
