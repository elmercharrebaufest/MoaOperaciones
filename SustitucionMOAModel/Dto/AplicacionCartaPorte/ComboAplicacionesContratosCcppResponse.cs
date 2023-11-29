using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class ComboAplicacionesContratosCcppResponse
    {
        public List<CartaPorteParaAplicacionCartaPorte> CartasPorte { get; set; }
        public List<ContratoParaAplicacionCartaPorte> Contratos { get; set; }
    }
}
