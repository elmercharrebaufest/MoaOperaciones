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
using System.Linq.Expressions;

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
				DateTime fechaIncioDateTime, fechaFinDateTime;
				try
				{
					fechaIncioDateTime = DateTime.Parse(request.FechaDesde);
				}
				catch
				{
					try
					{
						request.FechaDesde = new string(request.FechaDesde.Where(c => c != '\u200E').ToArray());
						fechaIncioDateTime = DateTime.Parse(request.FechaDesde);
					}
					catch (Exception e)
					{
						throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "inicio"), e);
					}
				}

				try
				{
					fechaFinDateTime = DateTime.Parse(request.FechaHasta);
				}
				catch
				{
					try
					{
						request.FechaHasta = new string(request.FechaHasta.Where(c => c != '\u200E').ToArray());
						fechaFinDateTime = DateTime.Parse(request.FechaHasta);
					}
					catch (Exception e)
					{
						throw new ValidationCustomException(String.Format(ErrorMsg.ErrorFechaInvalida, "fin"), e);
					}

				}


				var usuario = _repositorio.Obtener<Usuario>(u => u.Mail == request.MailUsuario);
				var esInterno = usuario.TienePermiso("VER ORDENES DE CARGA FASON ADMIN");
				fechaFinDateTime = fechaFinDateTime.AddDays(1);
				var descripcion = EstadoOrdenDeCargaFason.SinEstado;


				var clientes = usuario.Proveedores.Select(c => c.Id);
				var listadoDB = _repositorio.Listar<OrdenDeCargaFason>(x => (esInterno ? true : clientes.Contains(x.Cliente_Id)) && x.FechaCreacion <= fechaFinDateTime
				&& x.FechaCreacion >= fechaIncioDateTime);

				var listado = listadoDB.Select(x => new OrdenDeCargaFasonDto
				{
					Id = x.Id,
					FechaCreacion = x.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
					FechaRetiro = x.FechaRetiro.ToString("dd/MM/yyyy HH:mm"),
					Producto = x.Producto.Nombre,
					ColorSemaforo = x.Estado.ObtenerSemaforo(),
					EstadoDescripcion = esInterno ? descripcion.ToUserFriendlyString() : x.Estado.ToUserFriendlyString(),
					DescripcionEstadoListado = esInterno ? x.Estado.ToUserFriendlyString() : x.Estado.ToUserFriendlyString(),
					PatenteChasis = x.PatenteChasis,
					CantidadDeViajesRealizados = x.CantidadDeViajesRealizados,
					CantidadDeViajesEsperados = x.CantidadDeViajesEsperados,
					Destino = x.Destino
				}).ToList();

				var response = new ListarOrdenDeCargaFasonResponse();

				response.Response = listado;

                Log.Debug(GetType().Name, "Listar", $" response: {request.ToJson()}");


				return response;

			}
			catch (Exception)
            {

                throw;
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
