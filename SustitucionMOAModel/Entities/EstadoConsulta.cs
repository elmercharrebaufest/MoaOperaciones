using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class EstadoConsulta
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }
    }
}
