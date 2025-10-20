using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class TipoNormativa
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}
