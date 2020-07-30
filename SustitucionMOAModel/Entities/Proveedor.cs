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
        public bool Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public Contacto ContactoPagos { get; set; }
        public Contacto ContactoBoletos { get; set; }
        public TipoProveedor TipoProveedor { get; set; }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public string Observaciones { get; set; }

        [InverseProperty("Proveedores")]
        public virtual ICollection<Usuario> UsuariosAsociados { get; set; }
    }
}
