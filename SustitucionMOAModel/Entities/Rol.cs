using System.Collections.Generic;
using System.Linq;
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
        public bool EsEditable { get; set; }

        [InverseProperty("Roles")]
        public virtual ICollection<Usuario> Usuarios { get; set; }

        [InverseProperty("Roles")]
        public virtual ICollection<Categoria> Categorias { get; set; }

        [InverseProperty("RolesAsociados")]
        public virtual ICollection<PermisoPorRol> PermisosAsociados { get; set; }

        public virtual ICollection<Notificacion> NotificacionesAsociadas { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Rol rol &&
                   Id == rol.Id &&
                   Codigo == rol.Codigo &&
                   Nombre == rol.Nombre;
        }

        public override int GetHashCode()
        {
            int hashCode = 1820642526;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Codigo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nombre);
            return hashCode;
        }

        internal List<string> ObtenerPermisos()
        {
            if (PermisosAsociados == null) return new List<string>();
            return PermisosAsociados.Select(p => p.Permiso).ToList();
        }
    }
}
 