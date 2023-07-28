using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenDeCargaFason;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Enums.MoaWS.OrdenCargaWS;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class OrdenDeCargaFasonService : IOrdenDeCargaFasonService
    {
        private readonly IRepositorio _repositorio;
        protected readonly IOrdenCargaConsumerMOA _consumer;
        private readonly IScatoConsumer _scatoConsumer;
        private readonly IEnumerable<string> _codigosRetiroEnPatagonia = new string[] { "98855", "99098" };

        public OrdenDeCargaFasonService(IRepositorio repositorio, IOrdenCargaConsumerMOA consumer, IScatoConsumer _scatoConsumer)
        {
            _repositorio = repositorio;
            _consumer = consumer;
            this._scatoConsumer = _scatoConsumer;
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


                var clientes = usuario.Proveedores.Select(c => c.CodigoProveedor);
                var tipoUsuarioId = usuario.TipoUsuario.Id;
                var listadoDB = _repositorio.Listar<OrdenDeCargaFason>(x =>
                (esInterno ? true : clientes.Contains(x.Cliente.CodigoProveedor))
                && x.FechaCreacion >= fechaIncioDateTime && x.FechaCreacion <= fechaFinDateTime
                && (esInterno ? true : tipoUsuarioId == 5 ? x.CorredorId == null : x.CorredorId != null)
                );

                if (listadoDB == null || listadoDB.Count == 0)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga fason"));
                }

                var listado = listadoDB.OrderByDescending(x => x.FechaCreacion).Select(x => new OrdenDeCargaFasonDto
                {
                    Id = x.Id,
                    Cliente = x.Cliente.CodigoProveedor,
                    RazonSocialCliente = x.Cliente.RazonSocial,
                    Estado = x.Estado,
                    FechaCreacion = x.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                    FechaRetiro = x.FechaRetiro.ToString("dd/MM/yyyy"),
                    Cantidad = x.Cantidad,
                    Material = x.Producto.Nombre,
                    PatenteAcoplado = x.PatenteAcoplado,
                    PatenteChasis = x.PatenteChasis,
                    ColorSemaforo = x.Estado.ObtenerSemaforo(),
                    DescripcionEstado = esInterno ? x.Estado.ToFriendlyString() : x.Estado.ToUserFriendlyString(),
                    TransporteExiste = x.TransporteExiste,
                    NombreChofer = x.NombreChofer,
                    CUILChofer = x.CUILChofer,
                    RazonSocialTransporte = x.RazonSocialTransporte,
                    CUITTransporte = x.CUITTransporte,
                    Observacion = x.Observacion,
                    LocalidadId = x.LocalidadId,
                    LocalidadDescripcion = x.LocalidadDescripcion,
                    Corredor = x.Corredor?.CodigoProveedor,
                    RazonSocialCorredor = x.Corredor?.RazonSocial

                }).ToList();

                var response = new ListarOrdenDeCargaFasonResponse();

                response.Response = listado;

                return response;

            }
            catch (InfoCustomException icex)
            {
                throw icex;
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

                var x = _repositorio.Obtener<OrdenDeCargaFason>(IdOrdenDeCargaFason);

                var response = new OrdenDeCargaFasonDto
                {
                    Cantidad = x.Cantidad,
                    Cliente = x.Cliente.CodigoProveedor,
                    ColorSemaforo = x.Estado.ObtenerSemaforo(),
                    Corredor = x.Corredor?.CodigoProveedor,
                    CUILChofer = x.CUILChofer,
                    CUITCliente = "",
                    CUITTransporte = x.CUITTransporte,
                    DescripcionEstado = esInterno ? x.Estado.ToFriendlyString() : x.Estado.ToUserFriendlyString(),
                    DescripcionEstadoListado = "",
                    Estado = x.Estado,
                    FechaCreacion = x.FechaCreacion.ToString("dd/MM/yyyy HH:mm"),
                    FechaRetiro = x.FechaRetiro.ToString("dd/MM/yyyy"),
                    Id = x.Id,
                    LocalidadDescripcion = x.LocalidadDescripcion,
                    LocalidadId = x.LocalidadId,
                    Material = x.Producto.Nombre,
                    NombreChofer = x.NombreChofer,
                    Producto_Id = new SustitucionMOAModel.Models.DataAgro.MaterialDto
                    {
                        MaterialId = x.Producto.Id,
                        Descripcion = x.Producto.Nombre,
                        CodigoSap = x.Producto.CodigoSap
                    },
                    PatenteAcoplado = x.PatenteAcoplado,
                    PatenteChasis = x.PatenteChasis,
                    RazonSocialCliente = x.Cliente.RazonSocial,
                    RazonSocialCorredor = x.Corredor?.RazonSocial,
                    RazonSocialTransporte = x.RazonSocialTransporte,
                    TransporteExiste = x.TransporteExiste,
                    Observacion = x.Observacion
                };

                return new DetalleOrdenDeCargaFasonResponse { Response = response };
            }

            catch (Exception error)
            {
                Log.Error(error);
                throw new WSCustomException(ErrorMsg.ErrorWS, error);
            }
        }
        private bool TransporteExiste(string CUITTransporte)
        {
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest " + $"orden.CUITTransporte {CUITTransporte ?? ""}");
            var estadoTransportista = _consumer.GetOrdenCargaControlEstadoTransportista(CUITTransporte);
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest Result " + estadoTransportista);

            return estadoTransportista == ControlEstadoResEnum.TransportistaOK;
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
                orden.Estado = EstadoOrdenDeCargaFason.Pendiente;
                _repositorio.GuardarCambios();
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
        public List<ProveedorDto> GetCorredores()
        {
            var corredoresBD = _repositorio
                        .Listar<Usuario>(u =>
                            u.TipoUsuario.Id == (int)TipoUsuarioEnum.Corredor &&
                            u.Habilitado &&
                            u.Roles.Any(r =>
                                r.Codigo == "FASON"));  //TODO: Constantes

            var corredores = corredoresBD.Select(c => new ProveedorDto(c.ObtenerProveedor()));

            return corredores.ToList();
        }

        public List<ProveedorDto> GetClientesDeCorredor(string codigoCorredor)
        {
            if (!string.IsNullOrEmpty(codigoCorredor))
            {
                var corredor = _repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoCorredor);

                var corredores = _repositorio
                            .Listar<Usuario>(u =>
                                u.TipoUsuario.Id == (int)TipoUsuarioEnum.Corredor &&
                                u.CUITRegistro == corredor.CUIT &&
                                u.Habilitado);

                var proveedores = new List<ProveedorDto>();
                foreach (var corr in corredores)
                {
                    proveedores.AddRange(
                    corr.Proveedores.Where(x => x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente)
                        .Select(x => new ProveedorDto(x)).ToList());
                }
                return proveedores;
            }
            else
            {
                // igual pero tipo cliente
                var corredores = _repositorio
                            .Listar<Usuario>(u =>
                                u.TipoUsuario.Id == (int)TipoUsuarioEnum.Cliente &&
                                u.Habilitado &&
                                u.Roles.Any(r => r.Codigo == "FASON"));

                var proveedores = new List<ProveedorDto>();
                foreach (var corr in corredores)
                {
                    proveedores.AddRange(
                    corr.Proveedores.Where(x => x.TipoProveedor.Id == (int)TipoUsuarioEnum.Cliente)
                        .Select(x => new ProveedorDto(x)).ToList());
                }
                return proveedores;
            }
        }

        public Resultado Crear(CrearOrdenDeCargaFasonRequest request)
        {
            try
            {
                var existeTransporte = TransporteExiste(request.CUITTransporte);

                for (int i = 0; i < request.CantidadDeViajes; i++)
                {
                    var ordenEntity = new OrdenDeCargaFason(request);
                    ActualizarOrdenDeCarga(ordenEntity, existeTransporte);
                    _repositorio.Agregar(ordenEntity);
                    
                    _repositorio.GuardarCambios();
                }

                var resultado = new Resultado { Mensaje = SuccessMsg.OrdenDeCargaAgregada };
                return resultado;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw new WSCustomException(ErrorMsg.ErrorWS, ex);
            }
        }

        public Resultado Editar(EditarOrdenDeCargaFasonRequest request)
        {
            try
            {
                var orden = _repositorio.Obtener<OrdenDeCargaFason>(request.Id);
                var existeTransporte = TransporteExiste(request.CUITTransporte);

                orden.Cantidad = request.Cantidad;
                orden.Cliente_Id = request.Cliente;
                orden.CorredorId = request.CorredorId;
                orden.CUILChofer = request.CUILChofer;
                orden.CUITTransporte = request.CUITTransporte;
                orden.LocalidadId = request.Destino.LocalidadId;
                orden.LocalidadDescripcion = request.Destino.LocalidadDescripcion;
                //orden.Estado = estadoOrden;
                orden.FechaCreacion = DateTime.Now;
                orden.FechaRetiro = request.FechaRetiro;
                //Id =
                orden.NombreChofer = request.NombreChofer;
                orden.Observacion = request.Observacion;
                orden.PatenteAcoplado = request.PatenteAcoplado;
                orden.PatenteChasis = request.PatenteChasis;
                //Producto =
                orden.Producto_Id = request.Producto_Id.MaterialId;
                orden.RazonSocialTransporte = request.RazonSocialTransporte;
                orden.TransporteExiste = existeTransporte;
                orden.KmARecorrer = request.Destino.KmARecorrer;
                orden.Estado = existeTransporte ? EstadoOrdenDeCargaFason.Generada : EstadoOrdenDeCargaFason.Pendiente;

                _repositorio.GuardarCambios();

                return new Resultado { IdEntidad = request.Id, Mensaje = SuccessMsg.OrdenDeCargaActualizada };
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new Resultado { error = ex.Message };
            }
        }

        public object ObtenerDestinos(int clienteId)
        {
            Proveedor proveedor = _repositorio.Obtener<Proveedor>(a => a.Id == clienteId);
            if (proveedor.CUIT.Length != 11)
            {
                throw new ValidationCustomException("El cuit no tiene el formato correcto.");
            }
            return _scatoConsumer.BuscarDestinos(proveedor.CUIT);
        }
        private void ActualizarOrdenDeCarga(OrdenDeCargaFason orden, bool existeTransporte=false)
        {
            orden.TransporteExiste = existeTransporte;
            orden.Estado = ObtenerEstadoOrden(orden);
        }
        private EstadoOrdenDeCargaFason ObtenerEstadoOrden(OrdenDeCargaFason orden)
        {
            if (orden.TransporteExiste)
            {
                var producto = orden.Producto ?? _repositorio.Obtener<Material>(orden.Producto_Id);
                if (_codigosRetiroEnPatagonia.Contains(producto.CodigoSap))
                    return EstadoOrdenDeCargaFason.PendienteCompensacion;
                else
                    return EstadoOrdenDeCargaFason.Generada;
            }
            return EstadoOrdenDeCargaFason.Pendiente;
        }
    }
}
