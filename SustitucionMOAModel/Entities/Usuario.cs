using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SustitucionMOAModel.Entities
{
    class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Mail { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public Proveedor Proveedor { get; set; }
        public Rol Rol { get; set; }
        public int Estado { get; set; }
    }
}
