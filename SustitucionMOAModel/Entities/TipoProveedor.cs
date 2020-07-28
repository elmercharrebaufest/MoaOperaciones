using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SustitucionMOAModel.Entities
{
    public class TipoProveedor
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
}
