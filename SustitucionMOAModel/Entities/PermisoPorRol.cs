using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


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
