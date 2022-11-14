using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
	public class OrdenDeCargaFasonService : IOrdenDeCargaFasonService
	{
		private readonly IRepositorio _repositorio;
		protected readonly IOrdenCargaConsumerMOA _consumer;

		public OrdenDeCargaFasonService(IRepositorio repositorio, IOrdenCargaConsumerMOA consumer)
		{
			_repositorio = repositorio;
			_consumer = consumer;
		}

		public ListarOrdenDeCargaFasonResponse Listar(ListarOrdenDeCargaFasonRequest request)
		{
			Log.Info($"Listar(request: {request.ToJson()})");
			try
			{
				var ordenDeCargaFason = _repositorio.Listar<OrdenDeCargaFason>().ToList();
				var response = new ListarOrdenDeCargaFasonResponse(ordenDeCargaFason);
				Log.Debug(GetType().Name, "Listar", $" response: {request.ToJson()}");
				return response;
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				throw new WSCustomException(ErrorMsg.ErrorWS, ex);
			}
		}

		public DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenDeCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario)
        {
            try
            {
				
				var ordenDeCagarFason = _repositorio.Obtener<OrdenDeCargaFason>(IdOrdenDeCargaFason);
				Proveedor cliente = _repositorio.Obtener<Proveedor>(ordenDeCagarFason.Cliente_Id);
				var response = new DetalleOrdenDeCargaFasonResponse(ordenDeCagarFason);
				return response;
            }

			catch(Exception error)
            {
				Log.Error(error);
				throw new WSCustomException(ErrorMsg.ErrorWS, error);
			}
        }
		private bool TransporteExiste(string CUITTransporte)
		{
			Log.Info("TransporteExiste OrdenCargaControlEstadoRequest " + $"orden.CUITTransporte {CUITTransporte ?? ""}");
			var result = _consumer.OrdenCargaControlEstadoRequest("", "", CUITTransporte);
			Log.Info("TransporteExiste OrdenCargaControlEstadoRequest Result " + result);

			return result == "CE-07";
		}
		
		public string VerificarTransporte(int ordenId)
		{
			var orden = _repositorio.Obtener<OrdenDeCargaFason>(ordenId);
			orden.TransporteExiste = TransporteExiste(orden.CUITTransporte);
			if (orden.TransporteExiste)
			{
				return SuccessMsg.OrdenDeCargaActualizada;
			}
			else
			{
				return "El transporte no existe";
			}
		}

	}
}
