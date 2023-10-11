
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class ContratoParaAplicacionCartaPorte
    {
        public string NumeroContrato { get; set; }
    }
    public class ContratoParaAplicacionCartaPorteResponse
    {
        public List<ContratoParaAplicacionCartaPorte> Contratos { get; set; }
    }
}
