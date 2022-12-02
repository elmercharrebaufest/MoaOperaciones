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
using System.Data.Entity;
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
				//var descripcion = EstadoOrdenDeCargaFason.Generada;


				var clientes = usuario.Proveedores.Select(c => c.Id);
				var listadoDB = _repositorio.Listar<OrdenDeCargaFason>(x => (esInterno ? true : clientes.Contains(x.Cliente_Id)) && x.FechaCreacion >= fechaIncioDateTime
				&& x.FechaCreacion <= fechaFinDateTime);

				var listado = listadoDB.Select(x => new OrdenDeCargaFasonDto
				{
					Id = x.Id,
					Cliente = x.Cliente.CodigoProveedor,
					Estado = x.Estado,
					FechaCreacion = x.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
					FechaRetiro = x.FechaRetiro.ToString("dd/MM/yyyy HH:mm"),
					CantidadDeViajesRealizados = x.CantidadDeViajesRealizados,
					CantidadDeViajesEsperados = x.CantidadDeViajesEsperados,
					Producto = x.Producto.Nombre,
					PatenteChasis = x.PatenteChasis,
					Destino = x.Destino,
					ColorSemaforo = x.Estado.ObtenerSemaforo(),
					DescripcionEstado = esInterno ? x.Estado.ToFriendlyString() : x.Estado.ToUserFriendlyString()
				}).ToList();

				var response = new ListarOrdenDeCargaFasonResponse();

				response.Response = listado;

                Log.Debug(GetType().Name, "Listar", $" response: {request.ToJson()}");


				return response;

			}
			catch (Exception error)
			{
				Log.Error(error);
				throw new WSCustomException(ErrorMsg.ErrorWS, error);
			}

		}

		public DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenDeCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario)
        {
            try
            {

				var usuario = _repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario.MailUsuario);
				var esInterno = usuario.TienePermiso("VER ORDENES DE CARGA FASON ADMIN");

				var ordenDeCagarFason = _repositorio.Obtener<OrdenDeCargaFason>(IdOrdenDeCargaFason);
				Proveedor cliente = _repositorio.Obtener<Proveedor>(ordenDeCagarFason.Cliente_Id);
				var response = new DetalleOrdenDeCargaFasonResponse(ordenDeCagarFason, esInterno);
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
			Log.Info("TransporteExiste Fason");
			var result = _consumer.OrdenCargaControlEstadoRequest("", "", CUITTransporte);

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

        public List<OrdenDeCargaFason> VerificarVencimientoOrdenDeCargaFason()
        {

            var fechaLimite = DateTime.Now.Date;

			if (_repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesDeCargaFasonJob").Habilitado == false)
				return null;

			var ordenes = _repositorio.Listar<OrdenDeCargaFason>((orden) => DbFunctions.AddDays(orden.FechaRetiro, 5) < fechaLimite && (orden.Estado == EstadoOrdenDeCargaFason.Generada || orden.Estado == EstadoOrdenDeCargaFason.Pendiente));

            foreach (var orden in ordenes)
            {
				orden.Estado = EstadoOrdenDeCargaFason.Vencida;
            }
            _repositorio.GuardarCambios();

            return ordenes;
        }

    }
}
