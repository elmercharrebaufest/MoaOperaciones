using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Rubro
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

    }
}
