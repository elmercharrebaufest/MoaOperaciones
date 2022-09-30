using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOAUtils.Validadores.OrdenDeCarga
{
	public class VisualizarProductoRequestValidator : AbstractValidator<VisualizarProductoRequest>
	{
		public VisualizarProductoRequestValidator()
		{
			RuleFor(dto => dto.Contrato).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Contrato")));
		}
	}
}
