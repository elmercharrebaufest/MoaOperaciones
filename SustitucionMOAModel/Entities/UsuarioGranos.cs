using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    [Table("UsuarioGranos")]
    public class UsuarioGranos : Usuario
    {
        public UsuarioGranos() : base() { }

        public UsuarioGranos(string mail, string CUIT) : base(mail, CUIT) {
            TipoUsuario = TipoUsuario.GetTipoGranos();
        }


        public string Comercial { get; set; }
    }
}
