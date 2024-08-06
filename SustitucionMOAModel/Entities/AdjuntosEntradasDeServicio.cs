using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class AdjuntosEntradasDeServicio
    {
        [Key]
        public int Id { get; set; }
        public string NroESTemporal { get; set; }
        public string NombreEnBlob { get; set; }
        public string NombreArchivo { get; set; }
        public string Extension { get; set; }
    }
}
