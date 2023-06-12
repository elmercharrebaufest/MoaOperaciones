using SustitucionMOAModel.Enums;
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
        public virtual string Mail { get; set; }
        public string CUITRegistro { get; set; }
        public bool Habilitado { get; set; }
        public string SeccionesVisitadas { get; set; }

        public virtual TipoUsuario TipoUsuario { get; set; }

        public DateTime? UltimoLogin { get; set; }

        [InverseProperty("UsuariosAsociados")]
        public virtual ICollection<Proveedor> Proveedores { get; set; }
        [InverseProperty("Usuarios")]
        public virtual ICollection<Rol> Roles { get; set; }
        public bool AceptoTyC { get; set; }
        public DateTime? AceptoTyCFecha { get; set; }
        public string ApiKey { get; set; }
        //public virtual ICollection<Archivo> Archivos { get; set; }
        public string UsuarioSap { get; set; }

        [InverseProperty("Usuario")]
        public virtual ICollection<PeticionDeOferta> Peticiones { get; set; }

        public Rol ObtenerRolPrincipal()
        {
            return Roles.FirstOrDefault();
        }

        public Proveedor ObtenerProveedor()
        {
            //Por ahora los usuarios van a tener solo un proveedor. Devolvemos ese
            // ya no son mas uno solo. :(
            if (Proveedores == null)
                return null;

            Proveedor proveedor = null;
            try
            {
                proveedor = Proveedores.Where(p => p.CUIT == this.CUITRegistro && this.TipoUsuario.Id == p.TipoProveedor.Id).FirstOrDefault();
            }
            catch (Exception)
            {
                proveedor = Proveedores.Where(p => p.CUIT == this.CUITRegistro).FirstOrDefault();
            }

            if (proveedor == null)
            {
                proveedor = Proveedores.FirstOrDefault();
            }

            return proveedor;
        }

        public Proveedor ObtenerCorredor()
        {
            return Proveedores.Where(p => p.CUIT == this.CUITRegistro && p.TipoProveedor.Id == (int)TipoUsuarioEnum.Corredor).FirstOrDefault();
        }

        public Proveedor ObtenerProveedorPorId(int proveedorId)
        {
            if (proveedorId > 0)
                return Proveedores.Where(p => p.Id == proveedorId).FirstOrDefault();
            else
                return Proveedores.FirstOrDefault();
        }
        public Proveedor ObtenerProveedorPorCodigo(string codigoProveedor)
        {
            return Proveedores.Where(p => p.CodigoProveedor == codigoProveedor).FirstOrDefault();
        }

        public Proveedor ObtenerProveedorPorCUIT(string CUIT)
        {
            return Proveedores.Where(p => p.CUIT == CUIT).FirstOrDefault();
        }

        public bool TieneProveedor(string codigoProveedor)
        {
            //Los administradores pueden elegir impersonarse como cualquier proveedor
            if (Roles.Where(r => r.Codigo == "ADM").Any())
            {
                return true;
            }

            return Proveedores.Where(p => p.CodigoProveedor == codigoProveedor).Any();
        }

        public string ObtenerRazonSocial()
        {
            if (Proveedores.Count >= 1)
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
                Roles.Where(r => r.Codigo.Equals("NUENOGRAN")).Any() ||
                Roles.Where(r => r.Codigo.Equals("NUECLI")).Any() ||
                !Habilitado;
        }

        public bool EsCorredor()
        {
            return TipoUsuario.NombreCorto == "CORR";
        }

        public void RemoverRoles()
        {
            Roles.Clear();
        }

        public void RemoverRolesEditables()
        {
            Roles = Roles.Where(r => !r.EsEditable).ToList();
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


        public bool EsAdmin()
        {
            return Roles.Where(r => r.Codigo == "ADM").Any()
                    || Roles.Where(r => r.Codigo == "TODOS").Any();
        }


        public virtual bool TienePermiso(string permiso)
        {
            var permisosUsuario = ObtenerPermisos();

            return permisosUsuario.Contains(permiso);
        }

        public virtual bool TieneRol(string codigo)
        {

            return Roles.Any(r => r.Codigo == codigo);
        }
    }
}
