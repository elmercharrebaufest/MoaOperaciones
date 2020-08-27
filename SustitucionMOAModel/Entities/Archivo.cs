using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Archivo
    {
        [Key]
        public int Id { get; set; }

        public string FileKey { get; set; }

        public string Ruta { get; set; }
    }
}
