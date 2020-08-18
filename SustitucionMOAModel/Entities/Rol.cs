using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SustitucionMOAModel.Entities
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        [InverseProperty("Roles")]
        public virtual ICollection<Usuario> Usuarios { get; set; }

        [InverseProperty("RolesAsociados")]
        public virtual ICollection<PermisoPorRol> PermisosAsociados { get; set; }

        internal List<string> ObtenerPermisos()
        {
            return PermisosAsociados.Select(p => p.Permiso).ToList();
        }
    }
}
 