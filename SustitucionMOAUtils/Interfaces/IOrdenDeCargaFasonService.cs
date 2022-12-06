using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
	public interface IOrdenDeCargaFasonService
	{
		ListarOrdenDeCargaFasonResponse Listar(ListarOrdenDeCargaFasonRequest request);
		DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario);
		string VerificarTransporte(int ordenId);
        List<OrdenDeCargaFason> VerificarVencimientoOrdenDeCargaFason();
    }
}
