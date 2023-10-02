using System.Collections.Generic;

namespace SustitucionMOAModel.Models
{
    public class Usuario
    {
        public string username { get; set; }
        public string nombre { get; set; }
        public List<string> permisos { get; set; }
    }
}
