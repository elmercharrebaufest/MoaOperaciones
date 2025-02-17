using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class ContratoParaAplicacionCartaPorte
    {
        [Required]
        public string NumeroContrato { get; set; }
        [Required]
        public string Material { get; set; }
        [Required]
        public string CodigoProveedor { get; set; }
        public bool TieneAnticipo { get; set; }
        public string Centro { get; set; }

        public ContratoParaAplicacionCartaPorte() { }

        public ContratoParaAplicacionCartaPorte(string numeroContrato, string material, string codigoProveedor)
        {
            NumeroContrato = numeroContrato;
            Material = material;
            CodigoProveedor = codigoProveedor;
        }
    }
}
