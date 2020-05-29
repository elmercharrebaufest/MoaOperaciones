using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil
{
    public class PerfilesWSMOAResponse
    {
        public List<Perfil> perfiles { get; set; }
        public List<TipoProveedor> tipos { get; set; }

        public PerfilesWSMOAResponse() {
            this.perfiles = new List<Perfil>() { };
            this.tipos = new List<TipoProveedor>() { };
        }

    }
}
