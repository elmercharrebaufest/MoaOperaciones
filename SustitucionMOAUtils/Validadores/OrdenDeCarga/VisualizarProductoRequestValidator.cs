using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOAUtils.Validadores.OrdenDeCarga
{
    public class VisualizarProductoRequestValidator : AbstractValidator<VisualizarProductoRequest>
    {
        public VisualizarProductoRequestValidator()
        {
            RuleFor(dto => dto.Contrato)
                .NotNull()
                .NotEmpty()
                .WithMessage(string.Format(ErrorMsg.ErrorValorNuloVacio, "Contrato"));

        }
    }
}
