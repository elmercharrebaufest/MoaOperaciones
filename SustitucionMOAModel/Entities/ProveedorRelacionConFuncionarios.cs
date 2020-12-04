using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    [Table("ProveedorRelacionConFuncionarios")]
    public class ProveedorRelacionConFuncionarios
    {
        [Key]
        public int Id { get; set; }
        public int Proveedor_Id { get; set; }
        public string NombreFirma { get; set; }
        public string CargoFirma { get; set; }
        public string NombreFuncionario { get; set; }
        public string CargoFuncionario { get; set; }
        public string Vinculo { get; set; }

        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }
    }
}
