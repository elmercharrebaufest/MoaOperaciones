using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using System.ComponentModel.DataAnnotations;
using SustitucionMOAUtils.Logger;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.AplicacionCartaPortePendienteAplicarWebServiceMOA;
using AppCCPPRequests = SustitucionMOAWS.WSRequests.AplicacionCartaPorte;

namespace SustitucionMOAUtils.Services
{
    public class AplicacionCartaPorteService : IAplicacionCartaPorteService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IAplicacionCartaPorteConsumer consumer;
        public AplicacionCartaPorteService(IRepositorio repositorio, IAplicacionCartaPorteConsumer consumer)
        {
            this.repositorio = repositorio;
            this.consumer = consumer;
        }
        public List<AplicacionCartaPorteDto> Listar(string mailUsuario, string fechaInicio, string fechaFin)
        {
            var fechaInicioDateTime = DataFormatter.StringToDateTime(fechaInicio, "fechaInicio");
            var fechaFinDateTime = DataFormatter.StringToDateTime(fechaFin, "fechaFin");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            string cuit = usuario.CUITRegistro;
            bool isAdmin = usuario.TienePermiso(PermisoEnum.AbmAplicacionesCcpp);

            var rawList = repositorio.Listar<AplicacionCartaPorte>(apl =>
                (isAdmin || apl.Proveedor.CUIT == cuit) &&
                fechaInicioDateTime <= apl.FechaAlta && fechaFinDateTime >= DbFunctions.TruncateTime(apl.FechaAlta) &&
                apl.Estado != EstadoAplicacionCartaPorte.Eliminado
            ) ;

            var lista = rawList.Select(apl => new AplicacionCartaPorteDto(apl)).ToList();
            return lista;
        }
        public AplicacionCartaPorteDto Obtener(int aplicacionCCPPId, string mailUsuario)
        {
            return new AplicacionCartaPorteDto(new AplicacionCartaPorte());
        }
        public AplicacionCartaPorteFiltrosDto ObtenerFiltros(List<AplicacionCartaPorteDto> aplicaciones)
        {
            return new AplicacionCartaPorteFiltrosDto(aplicaciones);
        }
        public void EliminarAplicacion(int aplicacionId)
        {
            var aplicacion = repositorio.Obtener<AplicacionCartaPorte>(aplicacionId);
            if(aplicacion == null)
                throw new InfoCustomException("No se ha encontrado la aplicación.");
            if (aplicacion.Estado != EstadoAplicacionCartaPorte.Pendiente)
                throw new InfoCustomException("No se puede eliminar la aplicación.");
            aplicacion.Estado = EstadoAplicacionCartaPorte.Eliminado;
            repositorio.GuardarCambios();
        }
        public ComboAplicacionesContratosCcppResponse ObtenerCombosDeContratoCCPP(string mailUsuario, string codigoProveedor) {
            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);
            var proveedorAsignado = usuario.ObtenerProveedorAsignado();
            Log.Info($"el mail:{mailUsuario} el codigo proveedor: {codigoProveedor} codigo proveedor asignado: {proveedorAsignado.CodigoProveedor}");

            var codigoProveedorSeleccionado = ObtenerCodigoProveedorSeleccionado(usuario, proveedorAsignado, codigoProveedor);            
            var codigoCorredor = usuario.EsCorredor() ? proveedorAsignado.CodigoProveedor : null;
            
            var aplicacionesPendientes = consumer.ObtenerAplicacionesPendientes(
                new AppCCPPRequests.AppCartasPortePendienteRequest { Proveedor = codigoProveedorSeleccionado, Corredor = codigoCorredor}
            );

            var contratos = ObtenerContratosDisponibles(aplicacionesPendientes);

            var aplicacionesPendientesAplicar = ObtenerAplicacionesPendientes(codigoProveedor);
            var cartasPorte = ObtenerCartasPorteDisponibles(aplicacionesPendientes, aplicacionesPendientesAplicar);

            return new ComboAplicacionesContratosCcppResponse { CartasPorte= cartasPorte, Contratos= contratos};
        }
        public void GuardarAplicacion(CrearAplicacionCartaPorte aplicacionACrear, string mailUsuario)
        {
            ValidarSchema(aplicacionACrear, "AplicacionCartaPorte", "GuardarAplicacion");
            if (!aplicacionACrear.ValidarKilogramos())
                throw new InfoCustomException("Revisar valor de KG.");

            var aplicacionesPendientes = consumer.ObtenerAplicacionesPendientes(new AppCCPPRequests.AppCartasPortePendienteRequest {
                Proveedor = aplicacionACrear.ContratoSeleccionado.CodigoProveedor
            });
            var contratosValidos = ObtenerContratosDisponibles(aplicacionesPendientes);

            if (!aplicacionACrear.ValidarContrato(contratosValidos))
                throw new InfoCustomException("Revisar contrato seleccionado.");

            var aplicacionesPendientesAplicar = ObtenerAplicacionesPendientes(aplicacionACrear.ContratoSeleccionado.CodigoProveedor);
            var cartasPorteValidas = ObtenerCartasPorteDisponibles(aplicacionesPendientes, aplicacionesPendientesAplicar);

            if (!aplicacionACrear.ValidarCartaPorteSeleccionada(cartasPorteValidas))
                throw new InfoCustomException("Revisar carta porte seleccionada.");

            var usuario = repositorio.Obtener<Usuario>(u => u.Mail == mailUsuario);

            var proveedor = repositorio.Obtener<Proveedor>(p=>p.CodigoProveedor == aplicacionACrear.ContratoSeleccionado.CodigoProveedor && p.EstadoAprobacion == EstadoAprobacion.Aprobado);

            var aplicacion = new AplicacionCartaPorte(aplicacionACrear, usuario, proveedor);
            repositorio.Agregar(aplicacion);

            repositorio.GuardarCambios();
        }

        private void ValidarSchema<T>(T schema, string controller, string metodo)
        {
            var validationContext = new ValidationContext(schema);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(schema, validationContext, validationResults))
            {
                Log.Debug(controller, metodo, string.Join("; ", validationResults.Select(valRes => valRes.ErrorMessage)));
                throw new InfoCustomException("Error validando formulario.");
            }
        }
        private List<CartaPorteParaAplicacionCartaPorte> ObtenerCartasPorteDisponibles(
              ZMPES7070[] aplicacionesPendientes,
              IEnumerable<AplicacionCartaPorte> aplicacionesPendientesCargadas)
        {
            return aplicacionesPendientes
                .Where(app => !string.IsNullOrEmpty(app.CCPP))
                .Select(app => {
                    var kgPendientesCargados = aplicacionesPendientesCargadas
                       .Where(appPendiente => appPendiente.CartaPorte == app.CCPP)
                       .Sum(appPendiente => appPendiente.Kilogramos);
                    var kgPendientes = kgPendientesCargados > app.CANTIDAD ? 0 : app.CANTIDAD - kgPendientesCargados;

                    return new CartaPorteParaAplicacionCartaPorte(
                        numeroCartaPorte: app.CCPP,
                        kgPendientes: kgPendientes,
                        material: app.MATERIAL);
                }).ToList();
        }

        private List<ContratoParaAplicacionCartaPorte> ObtenerContratosDisponibles(ZMPES7070[] aplicacionesPendientes)
        {
            return aplicacionesPendientes
                .Where(app => !string.IsNullOrEmpty(app.CONTRATO))
                .Select(app => new ContratoParaAplicacionCartaPorte(
                    numeroContrato: app.CONTRATO,
                    material: app.MATERIAL,
                    codigoProveedor: app.PROVEEDOR
                    )).ToList();
        }
        private string ObtenerCodigoProveedorSeleccionado(Usuario usuario, Proveedor proveedorAsignado, string codigoSeleccionado)
        {
            var puedeSeleccionarProveedor = usuario.TienePermiso(PermisoEnum.SeleccionarVendedor);
            if (usuario.EsCorredor() && !puedeSeleccionarProveedor)
            {
                return null;
            }
            if (puedeSeleccionarProveedor && !string.IsNullOrEmpty(codigoSeleccionado))
            {
                return codigoSeleccionado;
            }
            return proveedorAsignado.CodigoProveedor;
        }
        private List<AplicacionCartaPorte> ObtenerAplicacionesPendientes(string codigoProveedorSeleccionado)
        {
            return repositorio.Listar<AplicacionCartaPorte>(app => app.Estado == EstadoAplicacionCartaPorte.Pendiente && app.Proveedor.CodigoProveedor == codigoProveedorSeleccionado);
        }
    }
}
