
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class CartaPorteParaAplicacionCartaPorte
    {
        public string NumeroCartaPorte { get; set; }
        public decimal KgPendientes { get; set; }
    }
    public class CartaPorteParaAplicacionCartaPorteResponse
    {
        public List<CartaPorteParaAplicacionCartaPorte> CartasPorte { get; set; }
    }
}
