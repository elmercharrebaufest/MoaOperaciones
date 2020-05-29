using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Usuario.Permiso
{
    public class PermisosWSMOAResponse
    {
        public List<Permiso> permisos { get; set; }

        public PermisosWSMOAResponse()
        {
            this.permisos = new List<Permiso>() { };
        }
    }
}
