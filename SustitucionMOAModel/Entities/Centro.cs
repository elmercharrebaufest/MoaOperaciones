using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Centro
    {
        [Key]
        public int Id { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }
        public string Nombre { get; set; }
        public string Calle { get; set; }
        public string Cp { get; set; }
        public string Pais { get; set; }
    }
}
