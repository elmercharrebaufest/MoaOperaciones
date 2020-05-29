using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Login
{
    public class LoginWSMOAResponse
    {
        public string error { get; set; }
        public string texto { get; set; }
        public string nombre { get; set; }
        public string proveedor { get; set; }
        public string granosFlag { get; set; }
        public string tipoUsuario { get; set; }
        public List<string> permisos { get; set; }

        public LoginWSMOAResponse() {
            this.permisos = new List<string>() { };
        }
    }
}
