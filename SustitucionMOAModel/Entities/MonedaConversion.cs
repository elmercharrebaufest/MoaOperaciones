using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class MonedaConversion
    {
        [Key]
        public int Id { get; set; }

        public string MonedaCodigo { get; set; }
        public int CantidadDecimal { get; set; }


    }
}
