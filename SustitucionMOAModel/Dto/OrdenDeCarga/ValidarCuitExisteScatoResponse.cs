
namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ValidarCuitExisteScatoResponse
    {
        public bool Existe { get; set; } = false;
        public string RazonSocial { get; set; } = null;
    }
}
