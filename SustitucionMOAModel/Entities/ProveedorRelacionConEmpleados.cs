using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    [Table("ProveedorRelacionConEmpleados")]
    public class ProveedorRelacionConEmpleados
    {
        [Key]
        public int Id { get; set; }
        public int Proveedor_Id { get; set; }
        public string NombreProveedora { get; set; }
        public string CargoProveedora { get; set; }
        public string NombreMolinos { get; set; }
        public string Vinculo { get; set; }

        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }

    }
}
