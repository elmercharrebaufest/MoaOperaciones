using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    [Table("UsuarioRelacionConEmpleados")]
    public class UsuarioRelacionConEmpleados
    {
        [Key]
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public string NombreProveedora { get; set; }
        public string CargoProveedora { get; set; }
        public string NombreMolinos { get; set; }
        public string Vinculo { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }

    }
}
