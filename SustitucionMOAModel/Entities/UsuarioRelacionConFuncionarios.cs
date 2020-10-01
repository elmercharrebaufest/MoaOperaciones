using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    [Table("UsuarioRelacionConFuncionarios")]
    public class UsuarioRelacionConFuncionarios
    {
        [Key]
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public string NombreFirma { get; set; }
        public string CargoFirma { get; set; }
        public string NombreFuncionario { get; set; }
        public string CargoFuncionario { get; set; }
        public string Vinculo { get; set; }

        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
    }
}
