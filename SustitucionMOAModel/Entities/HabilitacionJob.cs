using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class HabilitacionJob
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Habilitado { get; set; }


    }
}
