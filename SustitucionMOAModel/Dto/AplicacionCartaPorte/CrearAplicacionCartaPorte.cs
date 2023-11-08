
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class CrearAplicacionCartaPorte
    {
        [Required]
        public ContratoParaAplicacionCartaPorte ContratoSeleccionado { get; set; }
        [Required]
        public CartaPorteParaAplicacionCartaPorte CartaPorteSeleccionada { get; set; }
        [Required]
        public decimal Kilogramos { get; set; }

        public bool ValidarKilogramos()
        {
            return Kilogramos <= CartaPorteSeleccionada.KgPendientes;
        }
        public bool ValidarContrato(List<ContratoParaAplicacionCartaPorte> contratosValidos)
        {
            return contratosValidos.Any(contrato => contrato.NumeroContrato == ContratoSeleccionado.NumeroContrato);
        }
        public bool ValidarCartaPorteSeleccionada(List<CartaPorteParaAplicacionCartaPorte> cartasPorteValidas)
        {
            return CartaPorteSeleccionada.Material == ContratoSeleccionado.Material && cartasPorteValidas.Any(cartaPorte => cartaPorte.NumeroCartaPorte == CartaPorteSeleccionada.NumeroCartaPorte);
        }
    }
}
