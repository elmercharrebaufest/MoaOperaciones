using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Contacto
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
        public string Telefonos { get; set; }
        public string Mail { get; set; }
    }
}
