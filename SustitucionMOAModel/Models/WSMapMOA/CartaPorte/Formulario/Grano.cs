using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario
{
    public class Grano
    {
        public string grano { get; set; }

        public string cosecha { get; set; }

        public string localOrig { get; set; }

        public string provOrig { get; set; }

        public string tipo { get; set; }

        public string contrato { get; set; }

        public Boolean pesadaDest { get; set; }

        public string kilosEst { get; set; }

        public Boolean decCalidad { get; set; }

        public Boolean conforme { get; set; }

        public Boolean condicional { get; set; }

        public string bruto { get; set; }

        public string tara { get; set; }

        public string neto { get; set; }

        public string observa { get; set; }

        public string procedencia { get; set; }

        public string establecim { get; set; }
    }
}
