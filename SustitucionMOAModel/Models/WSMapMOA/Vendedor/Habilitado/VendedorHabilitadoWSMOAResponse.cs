using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor.Habilitado
{
    public class VendedorHabilitadoWSMOAResponse
    {
        public string status { get; set; }
        public List<Cabecera> cabeceras { get; set; }
        public List<Convenio> convenios { get; set; }
        public List<Exencion> exenciones { get; set; }

        public VendedorHabilitadoWSMOAResponse()
        {
            this.cabeceras = new List<Cabecera>() { };
            this.convenios = new List<Convenio>() { };
            this.exenciones = new List<Exencion>() { };
        }
    }
}
