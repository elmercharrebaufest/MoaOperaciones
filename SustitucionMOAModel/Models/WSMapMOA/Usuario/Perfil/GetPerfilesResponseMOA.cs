using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil
{
    public class GetPerfilesResponseMOA
    {
        public List<DropdownPerfilElement> perfiles { get; set; }
        public List<DropdownTipoElement> tipos { get; set; }

        public GetPerfilesResponseMOA()
        {
            this.perfiles = new List<DropdownPerfilElement>() { };
            this.tipos = new List<DropdownTipoElement>() { };
        }
    }
}
