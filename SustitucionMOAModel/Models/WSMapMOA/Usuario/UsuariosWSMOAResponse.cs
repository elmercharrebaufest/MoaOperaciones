using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Usuario
{
    public class UsuariosWSMOAResponse
    {
        public List<Usuario> usuarios { get; set; }

        public UsuariosWSMOAResponse() {
            this.usuarios = new List<Usuario>() { };
        }
    }
}
