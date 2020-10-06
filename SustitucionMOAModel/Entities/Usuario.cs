using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        //public virtual ICollection<Archivo> Archivos { get; set; }

        public Rol ObtenerRolPrincipal()
        {
            return Roles.FirstOrDefault();
        }

        public Proveedor ObtenerProveedor()
        {
            //Por ahora los usuarios van a tener solo un proveedor. Devolvemos ese
            return Proveedores.FirstOrDefault();
        }

        public Proveedor ObtenerCorredor()
        {
            return Proveedores.Where(p => p.CUIT == this.CUITRegistro).FirstOrDefault();
        }

        public Proveedor ObtenerProveedorPorId(int proveedorId)
        {
            if (proveedorId > 0)
                return Proveedores.Where(p => p.Id == proveedorId).FirstOrDefault();
            else
                return Proveedores.FirstOrDefault();
        }


        public string ObtenerRazonSocial()
        {
            if (Proveedores.Count >= 1 )
            {
                if (!string.IsNullOrEmpty(ObtenerProveedor().RazonSocial))
                    return ObtenerProveedor().RazonSocial;
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
                if (!string.IsNullOrEmpty(ObtenerProveedor().CodigoProveedor))
                    return ObtenerProveedor().CodigoProveedor;
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
                Roles.Where(r => r.Codigo.Equals("NOIMP")).Any() ||
                Roles.Where(r => r.Codigo.Equals("NUECORR")).Any() ||
                !Habilitado;
        }

        public void RemoverRoles()
        {
            Roles.Clear();
        }

        public void AgregarRol(Rol rol)
        {
            Roles.Add(rol);
        }

        public void RemoverRol(string rol)
        {
            var rolRemover = Roles.Where(r => r.Codigo == rol).FirstOrDefault();

            if (rolRemover != null)
                Roles.Remove(rolRemover);
        }
    }
}
