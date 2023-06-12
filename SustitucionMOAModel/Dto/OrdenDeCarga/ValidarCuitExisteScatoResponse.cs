
namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class ValidarCuitExisteScatoResponse
    {
        public bool Existe { get; set; } = false;
        public string RazonSocial { get; set; } = null;
        public ValidarCuitExisteScatoResponse(bool existe, string razonSocial)
        {
            Existe = existe;
            RazonSocial = razonSocial;
        }

        public static ValidarCuitExisteScatoResponse Nuevo(bool existe, string razonSocial)
        {
            return new ValidarCuitExisteScatoResponse(existe, razonSocial);
        }
    }
}
