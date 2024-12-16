using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOAUtils.Validadores.OrdenDeCarga
{
    public class ValidarCorredorClienteContratoProductoRequestValidator : AbstractValidator<ValidarCorredorClienteContratoProductoRequest>
    {
        public ValidarCorredorClienteContratoProductoRequestValidator()
        {
            RuleFor(dto => dto.ClienteCuit)
                .NotNull()
                .NotEmpty()
                .WithMessage(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "CUIT Cliente")));
            RuleFor(dto => dto.Contrato).NotNull().NotEmpty().WithMessage(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Contrato")));
            RuleFor(dto => dto.ProductoId).NotNull().NotEmpty().WithMessage(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Id Producto")));
        }
    }
}
