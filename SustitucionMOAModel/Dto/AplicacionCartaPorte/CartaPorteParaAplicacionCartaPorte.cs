
using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class CartaPorteParaAplicacionCartaPorte
    {
        [Required]
        public string NumeroCartaPorte { get; set; }
        [Required]
        public decimal KgPendientes { get; set; }
        [Required]
        public string Material { get; set; }
        public CartaPorteParaAplicacionCartaPorte(string numeroCartaPorte, decimal kgPendientes, string material)
        {
            NumeroCartaPorte = numeroCartaPorte;
            KgPendientes = kgPendientes;
            Material = material;
        }   
    }
}
