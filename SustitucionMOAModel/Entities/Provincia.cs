using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Provincia
    {
        [Key]
        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }

        public int Orden { get; set; }

    }

}
