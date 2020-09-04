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

        public virtual TipoUsuario TipoUsuario { get; set; }

        [InverseProperty("UsuariosAsociados")]
        public virtual ICollection<Proveedor> Proveedores { get; set; }
        [InverseProperty("Usuarios")]
        public virtual ICollection<Rol> Roles { get; set; }

        public virtual ICollection<Archivo> Archivos { get; set; }

        //internal Usuario() { }

        //public Usuario(string mail, string CUIT)
        //{
        //    Mail = mail;
        //    CUITRegistro = CUIT;
        //    Proveedores = new List<Proveedor>();
        //    Roles = new List<Rol>();
        //}

        public Rol ObtenerRolPrincipal()
        {
            return Roles.First();
        }

        public Proveedor ObtenerProveedorActual()
        {
            //Por ahora los usuarios van a tener solo un proveedor. Devolvemos ese
            return Proveedores.First();
        }

        public string ObtenerRazonSocial()
        {
            if (Proveedores.Count >= 1 )
            {
                if (!string.IsNullOrEmpty(ObtenerProveedorActual().RazonSocial))
                    return ObtenerProveedorActual().RazonSocial;
                else
                    return "No definido";
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
                if (!string.IsNullOrEmpty(ObtenerProveedorActual().RazonSocial))
                    return ObtenerProveedorActual().CodigoProveedor;
                else
                    return "-";
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
            return
                Roles.Where(r => r.Codigo.Equals("NUEG")).Any() ||
                Roles.Where(r => r.Codigo.Equals("DDAG")).Any() ||
                Roles.Where(r => r.Codigo.Equals("NOIMP")).Any(); 
        }

        public void RemoverRoles()
        {
            Roles.Clear();
        }

        public void AgregarRol(Rol rol)
        {
            Roles.Add(rol);
        }
    }
}
