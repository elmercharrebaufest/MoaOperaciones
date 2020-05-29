using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models
{
    public class Usuario
    {
        public string username { get; set; }
        public string nombre { get; set; }
        public List<string> permisos { get; set; }
    }
}
