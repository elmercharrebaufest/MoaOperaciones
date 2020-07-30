using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;

namespace SustitucionMOAModel.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Mail { get; set; }
        public string CUITRegistro { get; set; }
        public bool Habilitado { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        [InverseProperty("UsuariosAsociados")]
        public virtual ICollection<Proveedor> Proveedores { get; set; }
        [InverseProperty("Usuarios")]
        public virtual ICollection<Rol> Roles { get; set; }

        public Usuario()
        {
            Proveedores = new List<Proveedor>();
            Roles = new List<Rol>();
        }
        public Usuario(string mail, string CUIT)
        {
            Mail = mail;
            CUITRegistro = CUIT;
            Proveedores = new List<Proveedor>();
            Roles = new List<Rol>();
        }

        public string ObtenerRazonSocial()
        {
            if (Proveedores.Count >= 1 )
            {
                return Proveedores.First().RazonSocial;
            }
            else
            {
                return "";
            }
        }

        public string ObtenerCodigoProveedor()
        {
            if (Proveedores.Count >= 1)
            {
                return Proveedores.First().CodigoProveedor;
            }
            else
            {
                return "";
            }
        }

        public List<string> ObtenerPermisos()
        {
            List<string> permisosUsuario = new List<string>();
            foreach (Rol Rol in Roles)
            {
                permisosUsuario.AddRange(Rol.ObtenerPermisos());
            }

            return permisosUsuario;
        }

        public bool EstaHabilitado()
        {
            return Habilitado;
        }

        public bool EsNuevoUsuario()
        {
            return Roles.Where(r => r.Nombre.Equals("Nuevo Usuario")).Any();
        }
    }
}
