using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Entities
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoProveedor { get; set; }
        public string Mail { get; set; }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public string Observaciones { get; set; }
        public int? IdDataAgro { get; set; }
        public int? IdComercialDataAgro { get; set; }
        public string EstadoSIPER { get; set; }

        public DateTime? FechaSolicitud { get; set; }


        public virtual ICollection<Archivo> Archivos { get; set; }

        [InverseProperty("Proveedores")]
        public virtual ICollection<Usuario> UsuariosAsociados { get; set; }

        [InverseProperty("Proveedor")]
        public virtual ICollection<ProveedorHistorialAprobacion> HistorialAprobaciones { get; set; } = new List<ProveedorHistorialAprobacion>();

        
        public bool? VinculoConEmpleadosDeMolinos { get; set; }
        public bool? VinculoConFuncionariosPublicos { get; set; }

        [InverseProperty("Proveedor")]
        public virtual ICollection<ProveedorRelacionConEmpleados> RelacionConEmpleados { get; set; }

        [InverseProperty("Proveedor")]
        public virtual ICollection<ProveedorRelacionConFuncionarios> RelacionConFuncionarios { get; set; }

        public string Comercial { get; set; }

    }
}
