using FluentValidation;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenDeCarga;

namespace SustitucionMOAUtils.Validadores.OrdenDeCarga
{
	public class CrearOrdenEnSAPRequestValidator : AbstractValidator<CrearOrdenEnSAPRequest>
	{
		public CrearOrdenEnSAPRequestValidator()
		{
			RuleFor(dto => dto.IdOrdenDeCarga).GreaterThan(0).OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Id de Orden de Carga")));

			RuleFor(dto => dto.ClienteCodigo).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Codigo de Cliente")));
			RuleFor(dto => dto.ContratoSAP).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Codigo de Contrato SAP")));
			RuleFor(dto => dto.CorredorCodigo).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Codigo de Corredor")));

			RuleFor(dto => dto.Cantidad).GreaterThan(0).OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Cantidad")));

			RuleFor(dto => dto.MaterialCodigoSAP).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Codigo SAP de Material")));
			RuleFor(dto => dto.NumeroPedidoIngresado).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Numero de Pedido Ingresado")));
			//RuleFor(dto => dto.ValidarKg).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Kilos ingresados")));
			RuleFor(dto => dto.MailUsuarioSAP).NotNull().NotEmpty().OnAnyFailure(error => throw new ValidationCustomException(string.Format(ErrorMsg.ErrorValorNuloVacio, "Usuario SAP")));
		}
	}
}
