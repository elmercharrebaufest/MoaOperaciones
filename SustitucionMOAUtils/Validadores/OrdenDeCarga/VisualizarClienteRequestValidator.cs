using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOAUtils.Validadores.OrdenDeCarga
{
    public class VisualizarClienteRequestValidator : AbstractValidator<VisualizarClienteRequest>
    {
        public VisualizarClienteRequestValidator()
        {
            RuleFor(dto => dto.Corredor)
                .NotNull()
                .NotEmpty()
                .WithMessage(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Corredor")));
        }
    }
}
