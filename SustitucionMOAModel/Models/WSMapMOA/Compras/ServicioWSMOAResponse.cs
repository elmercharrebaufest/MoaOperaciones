using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class ServicioWSMOAResponse
    {
        public string error { get; set; }
        public List<Servicio> Servicios { get; set; }

    }
}
