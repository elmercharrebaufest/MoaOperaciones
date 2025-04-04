using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using System.Collections.Generic;

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
        public EstadoAplicacionCartaPorte Estado { get; set; }
        public int Proveedor_Id { get; set; }
        public string CodigoCorredor { get; set; }
        public string CodigoCentro { get; set; }
        public string CodigoMaterial { get; set; }
    }

    public class ErrorValidacionCargaMasivaCCPP : AplicacionGuardadaCargaMasivaCCPP
    {
        public int Fila { get; set; }
        public string Error { get; set; }
    }
}
