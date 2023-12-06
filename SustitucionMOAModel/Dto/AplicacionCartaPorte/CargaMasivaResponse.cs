using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class CargaMasivaResponse
    {
        public bool HayErroresValidacion { get; set; }

        public List<ErrorValidacionCargaMasivaCCPP> ErroresValidacion { get; set; }

        public List<AplicacionGuardadaCargaMasivaCCPP> AplicacionesGuardadas { get; set; }
    }

    public class AplicacionGuardadaCargaMasivaCCPP
    {
        public string ContratoNumero { get; set; }
        public string CartaDePorte { get; set; }
        public string Kilos { get; set; }
    }

    public class ErrorValidacionCargaMasivaCCPP : AplicacionGuardadaCargaMasivaCCPP
    {
        public int Fila { get; set; }
        public string Error { get; set; }
    }
}
