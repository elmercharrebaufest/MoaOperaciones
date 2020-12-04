using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    [Table("UsuarioNoGranos")]
    public class UsuarioNoGranos : Usuario
    {
        //private UsuarioNoGranos() : base() { }

        //public UsuarioNoGranos(string mail, string CUIT) : base(mail, CUIT) { }
    }
}
