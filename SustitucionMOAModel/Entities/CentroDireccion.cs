using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class CentroDireccion
    {
        [Key]
        public int Id { get; set; }
        public string CodigoSap { get; set; }
        public string Direccion { get; set; }
        public string Numero { get; set; }
        public string Cp { get; set; }
        public string Pais { get; set; }
    }
}
