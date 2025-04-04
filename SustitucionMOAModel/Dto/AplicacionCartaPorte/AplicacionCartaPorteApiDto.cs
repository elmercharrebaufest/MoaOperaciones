using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class AplicacionCartaPorteApiDto
    {
        public int Id { get; set; }

        public string Contrato { get; set; }

        public string CartaPorte { get; set; }

        public EstadoAplicacionCartaPorte Estado { get; set; }

        public int Kilogramos { get; set; }
        public string CodigoCentro { get; set; }
        public string CodigoCorredor { get; set; }
        public string CodigoMaterial { get; set; }

        public AplicacionCartaPorteApiDto(Entities.AplicacionCartaPorte entidad)
        {
            Id = entidad.Id;
            Contrato = entidad.Contrato;
            CartaPorte = entidad.CartaPorte;
            Estado = entidad.Estado;
            Kilogramos = entidad.Kilogramos;
            CodigoCentro = entidad.CodigoCentro;
            CodigoCorredor = entidad.CodigoCorredor;
            CodigoMaterial = entidad.CodigoMaterial;
        }
    }
}
