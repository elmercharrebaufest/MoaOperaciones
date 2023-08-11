using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
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
    public class OrdenDeCargaFasonService : OrdenDeCargaServiceBase, IOrdenDeCargaFasonService
    {
        private readonly IRepositorio repositorio;

        private readonly IEnumerable<string> codigosRetiroEnPatagonia = new string[] { "98855", "99098" };

        public OrdenDeCargaFasonService(IRepositorio repositorio,
            IOrdenCargaConsumerMOA ordenCargaConsumer,
            IScatoConsumer scatoConsumer,
            IEmailFasonService emailFasonService,
            IScatoRepositorioClient scatoRepositorioClient
            ) : base(ordenCargaConsumer, scatoConsumer, scatoRepositorioClient, emailFasonService)
        {
            this.repositorio = repositorio;
        }

        public ListarOrdenDeCargaFasonResponse Listar(ListarOrdenDeCargaFasonRequest request)
        {
            Log.Info($"Listar(request: {request.ToJson()})");

            try
            {
                var fechaIncioDateTime = DataFormatter.StringToDateTime(request.FechaDesde, "");
                var fechaFinDateTime = DataFormatter.StringToDateTime(request.FechaHasta, "");

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == request.MailUsuario);
                var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);
                fechaFinDateTime = fechaFinDateTime.AddDays(1);


                var clientes = usuario.Proveedores.Select(c => c.CodigoProveedor);
                var tipoUsuarioId = usuario.TipoUsuario.Id;
                var listadoDB = repositorio.Listar<OrdenDeCargaFason>(x =>
                (esInterno || clientes.Contains(x.Cliente.CodigoProveedor))
                && x.FechaCreacion >= fechaIncioDateTime && x.FechaCreacion <= fechaFinDateTime
                && (esInterno || (tipoUsuarioId == 5 ? x.CorredorId == null : x.CorredorId != null))
                );

                if (listadoDB == null || listadoDB.Count == 0)
                {
                    throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga fason"));
                }

                var listado = listadoDB.OrderByDescending(x => x.FechaCreacion)
                    .Select(x => new OrdenDeCargaFasonDto(x, esInterno))
                    .ToList();

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

        public DetalleOrdenDeCargaFasonResponse ObtenerDetalle(int IdOrdenCargaFason, DetalleOrdenDeCargaFasonRequest mailUsuario)
        {
            try
            {

                var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario.MailUsuario);
                var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);

                var orden = repositorio.Obtener<OrdenDeCargaFason>(IdOrdenCargaFason);

                var response = new OrdenDeCargaFasonDto(orden, esInterno);

                return new DetalleOrdenDeCargaFasonResponse { Response = response };
            }

            catch (Exception error)
            {
                Log.Error(error);
                throw new WSCustomException(ErrorMsg.ErrorWS, error);
            }
        }

        public string VerificarTransporte(int ordenId)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId);
            ActualizarOrdenDeCarga(orden);

            repositorio.GuardarCambios();
            return "Orden actualizada";
        }

        public List<OrdenDeCargaFason> VerificarVencimientoOrdenDeCargaFason()
        {

            var fechaLimite = DateTime.Now.Date;

            if (repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesDeCargaFasonJob" && a.Habilitado) == null)
                return new List<OrdenDeCargaFason>();

            var ordenes = repositorio.Listar<OrdenDeCargaFason>((orden) => DbFunctions.AddDays(orden.FechaRetiro, 5) < fechaLimite && (orden.Estado == EstadoOrdenDeCargaFason.Generada || orden.Estado == EstadoOrdenDeCargaFason.Pendiente));

            foreach (var orden in ordenes)
            {
                orden.Estado = EstadoOrdenDeCargaFason.Vencida;
            }
            repositorio.GuardarCambios();

            return ordenes;
        }
        public List<ProveedorDto> GetCorredores()
        {
            var corredoresBD = repositorio
                        .Listar<Usuario>(u =>
                            u.TipoUsuario.Id == (int)TipoUsuarioEnum.Corredor &&
                            u.Habilitado &&
                            u.Roles.Any(r =>
                                r.Codigo == "FASON"));

            var corredores = corredoresBD.Select(c => new ProveedorDto(c.ObtenerProveedor()));

            return corredores.ToList();
        }

        public List<ProveedorDto> GetClientesDeCorredor(string codigoCorredor)
        {
            if (!string.IsNullOrEmpty(codigoCorredor))
            {
                var corredor = repositorio.Obtener<Proveedor>(p => p.CodigoProveedor == codigoCorredor);

                var corredores = repositorio
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
                var corredores = repositorio
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

        public Resultado Crear(CrearOrdenDeCargaFasonRequest request, string mailUsuario)
        {
            try
            {
                ValidarRequest(request, mailUsuario);
                var existeTransporteEIntermediario = TransporteExiste(request.CUITTransporte, request.CUITIntermediarioFlete);

                for (int i = 0; i < request.CantidadDeViajes; i++)
                {
                    var ordenEntity = new OrdenDeCargaFason(request);
                    ActualizarOrdenDeCarga(ordenEntity, existeTransporteEIntermediario);
                    repositorio.Agregar(ordenEntity);

                    repositorio.GuardarCambios();
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

        public Resultado Editar(EditarOrdenDeCargaFasonRequest request, string mailUsuario)
        {
            try
            {
                var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
                ValidarRequest(request, usuario);
                var orden = repositorio.Obtener<OrdenDeCargaFason>(request.Id);

                orden.Cantidad = request.Cantidad;
                orden.Cliente_Id = request.Cliente;
                orden.CorredorId = request.CorredorId;
                orden.CUILChofer = request.CUILChofer;
                orden.CUITTransporte = request.CUITTransporte;
                orden.LocalidadId = request.Destino.LocalidadId;
                orden.LocalidadDescripcion = request.Destino.LocalidadDescripcion;
                orden.FechaCreacion = DateTime.Now;
                orden.FechaRetiro = request.FechaRetiro;
                orden.NombreChofer = request.NombreChofer;
                orden.Observacion = request.Observacion;
                orden.PatenteAcoplado = request.PatenteAcoplado;
                orden.PatenteChasis = request.PatenteChasis;
                orden.Producto_Id = request.Producto_Id;
                orden.RazonSocialTransporte = request.RazonSocialTransporte;
                orden.KmARecorrer = request.Destino.KmARecorrer;
                orden.FleteMOA = request.FleteMOA;
                orden.CUITIntermediarioFlete = request.CUITIntermediarioFlete;
                orden.RazonSocialIntermediarioFlete = request.RazonSocialIntermediarioFlete;
                orden.Reventa = request.Reventa;
                orden.PlantaCodigo = request.PlantaCodigo;
                orden.DomicilioTipo = request.DomicilioTipo;
                orden.DomicilioOrden = request.DomicilioOrden;
                orden.DomicilioDescr = request.DomicilioDescr;
                orden.RazonSocialDestino = request.RazonSocialDestino;
                orden.Escalable = request.Escalable;
                ActualizarOrdenDeCarga(orden);

                if (!usuario.TieneRol(RolEnum.FasonAdmin))
                    orden.Estado = EstadoOrdenDeCargaFason.EdicionSolicitada;

                repositorio.GuardarCambios();

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
            Proveedor proveedor = repositorio.Obtener<Proveedor>(a => a.Id == clienteId);
            if (proveedor.CUIT.Length != 11)
            {
                throw new ValidationCustomException("El cuit no tiene el formato correcto.");
            }
            return scatoConsumer.BuscarDestinos(proveedor.CUIT);
        }
        public void VerificarTransporteJob()
        {
            if (repositorio.Obtener<HabilitacionJob>(hj => hj.Nombre == "VerificarTransporteOrdenesDeCargaFasonJob" && hj.Habilitado) == null)
                return;

            var ordenes = repositorio.Listar<OrdenDeCargaFason>(orden => !orden.TransporteExiste && orden.Estado == EstadoOrdenDeCargaFason.Pendiente);
            foreach (var orden in ordenes)
            {
                ActualizarOrdenDeCarga(orden);
            }
            repositorio.GuardarCambios();
        }
        public OrdenDeCargaFasonDto ActualizarSolicitudEdicion(EstadoSolicitudEdicionFason estadoSolicitud)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(estadoSolicitud.OrdenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado != EstadoOrdenDeCargaFason.EdicionSolicitada)
                throw new InfoCustomException("Esta orden no está en estado de edición solicitada.");

            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == estadoSolicitud.MailUsuario);
            if (!usuario.TieneRol(RolEnum.FasonAdmin))
                throw new InfoCustomException("Usuario sin permisos para realizar esta acción.");

            if (estadoSolicitud.Aprobado)
                ActualizarOrdenDeCarga(orden);
            else
                orden.Estado = EstadoOrdenDeCargaFason.EdicionRechazada;

            repositorio.GuardarCambios();

            return OrdenDeCargaFasonDto(orden, usuario);
        }
        public OrdenDeCargaFasonDto SolicitarAnulacion(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado == EstadoOrdenDeCargaFason.AnulacionSolicitada)
                throw new InfoCustomException("La anulación de esta orden ya fue solicitada.");
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            if (usuario.TieneRol(RolEnum.FasonAdmin))
                throw new InfoCustomException("Usuario sin permisos para realizar esta acción.");

            orden.Estado = EstadoOrdenDeCargaFason.AnulacionSolicitada;

            repositorio.GuardarCambios();

            return OrdenDeCargaFasonDto(orden, usuario);
        }
        public OrdenDeCargaFasonDto ActualizarSolicitudAnulacion(EstadoSolicitudAnulacionFason estadoSolicitud)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(estadoSolicitud.OrdenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado != EstadoOrdenDeCargaFason.AnulacionSolicitada)
                throw new InfoCustomException("Esta orden no está en estado de anulación solicitada.");
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == estadoSolicitud.MailUsuario);
            if (!usuario.TieneRol(RolEnum.FasonAdmin))
                throw new InfoCustomException("Usuario sin permisos para realizar esta acción.");


            if (estadoSolicitud.Aprobado)
                orden.Estado = EstadoOrdenDeCargaFason.Anulada;
            else
                ActualizarOrdenDeCarga(orden);

            repositorio.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }
        public OrdenDeCargaFasonDto AnularOrden(int ordenId, string mailUsuario)
        {
            var orden = repositorio.Obtener<OrdenDeCargaFason>(ordenId);
            if (orden == null)
                throw new InfoCustomException("Orden no encontrada");
            if (orden.Estado == EstadoOrdenDeCargaFason.Entregada)
                throw new InfoCustomException("Esta orden no puede ser anulada, ya fue entregada.");
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            if (!usuario.TieneRol(RolEnum.FasonAdmin))
                throw new InfoCustomException("Usuario sin permisos para realizar esta acción.");

            orden.Estado = EstadoOrdenDeCargaFason.Anulada;

            repositorio.GuardarCambios();
            return OrdenDeCargaFasonDto(orden, usuario);
        }
        private void ActualizarOrdenDeCarga(OrdenDeCargaFason orden)
        {
            var existeTransporteEIntermediario = TransporteExiste(orden);
            ActualizarOrdenDeCarga(orden, existeTransporteEIntermediario);
        }
        private void ActualizarOrdenDeCarga(OrdenDeCargaFason orden, bool existeTransporteEIntermediario)
        {
            orden.TransporteExiste = existeTransporteEIntermediario;
            orden.Estado = ObtenerEstadoOrden(orden);
        }
        private EstadoOrdenDeCargaFason ObtenerEstadoOrden(OrdenDeCargaFason orden)
        {
            if (orden.TransporteExiste)
            {
                var producto = orden.Producto ?? repositorio.Obtener<Material>(orden.Producto_Id);
                if (codigosRetiroEnPatagonia.Contains(producto.CodigoSap))
                    return EstadoOrdenDeCargaFason.PendienteCompensacion;
                else
                    return EstadoOrdenDeCargaFason.Generada;
            }
            return EstadoOrdenDeCargaFason.Pendiente;
        }
        /// <summary>
        /// Modifica los datos de la request según validaciones respecto al usuario que realizala petición
        /// </summary>
        /// <param name="request"></param>
        /// <param name="mailUsuario"></param>
        private void ValidarRequest(OrdenDeCargaFasonRequest request, string mailUsuario)
        {
            var usuario = repositorio.Obtener<Usuario>(us => us.Mail == mailUsuario);
            ValidarRequest(request, usuario);
        }
        private void ValidarRequest(OrdenDeCargaFasonRequest request, Usuario usuario)
        {
            Log.Info($"FASON - Validar Request {request.ToJson()}  usuario: {usuario.Mail}");
            var fleteMOA = usuario.TieneRol(RolEnum.FleteMOA);
            var reventa = usuario.TieneRol(RolEnum.Revendedor);
            Log.Info($"FASON - Validar Request: RolFleteMOA={fleteMOA};  RolReventa={reventa}");
            request.FleteMOA = fleteMOA && request.FleteMOA;
            request.Reventa = reventa && request.Reventa;
            if (!request.ProductoSeleccionado.ValidaSisaRuca)
            {
                request.CUITIntermediarioFlete = null;
                request.RazonSocialIntermediarioFlete = null;
                request.DomicilioDescr = null;
                request.DomicilioOrden = null;
                request.DomicilioTipo = null;
                request.PlantaCodigo = null;
            }
        }
        private OrdenDeCargaFasonDto OrdenDeCargaFasonDto(OrdenDeCargaFason orden, Usuario usuario)
        {
            var esInterno = usuario.TienePermiso(PermisoEnum.VerOrdenesDeCargaFasonAdmin);
            return new OrdenDeCargaFasonDto(orden, esInterno);
        }
        private bool TransporteExiste(OrdenDeCargaFason orden)
        {
            return TransporteExiste(orden.CUITTransporte, orden.CUITIntermediarioFlete);
        }
        private bool TransporteExiste(string transporte, string intermediarioFlete)
        {
            var existeTransporte = TransporteExiste(transporte);
            //Si no tiene Cuit intermediario flete lo tomamos como que existe
            var existeIntermediarioFlete = string.IsNullOrEmpty(intermediarioFlete) || TransporteExiste(intermediarioFlete);

            return existeTransporte && existeIntermediarioFlete;
        }
        private bool TransporteExiste(string CUITTransporte)
        {
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest " + $"Cuit {CUITTransporte ?? ""}");
            var estadoTransportista = ordenCargaConsumer.GetOrdenCargaControlEstadoTransportista(CUITTransporte);
            Log.Info("TransporteExiste OrdenCargaControlEstadoRequest Result " + estadoTransportista);

            return estadoTransportista == ControlEstadoResEnum.TransportistaOK;
        }
    }
}
