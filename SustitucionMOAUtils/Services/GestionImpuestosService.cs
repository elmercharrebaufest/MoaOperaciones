using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace SustitucionMOAUtils.Services
{
    public class GestionImpuestosService : IGestionImpuestosService
    {
        private readonly IRepositorio repositorio;
        private readonly ITimeProvider timeProvider;
        private readonly IAzureService azureService;

        private readonly string rutaArchivosCM05 = ConfigurationManager.AppSettings["RutaArchivosCM05"];

        public GestionImpuestosService(IRepositorio repositorio, ITimeProvider timeProvider, IAzureService azureService)
        {
            this.repositorio = repositorio;
            this.timeProvider = timeProvider;
            this.azureService = azureService;
        }

        public IList<IngresosBrutosCoeficienteUnificadoDto> ListarCabeceras()
        {
            return repositorio.Listar<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>(
            x => new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = x.Id,
                EstadoId = x.EstadoIngresosBrutosCoeficienteUnificado.Id,
                Estado = new EstadoIngresosBrutosCoeficienteUnificadoDto()
                {
                    Id = x.EstadoIngresosBrutosCoeficienteUnificado.Id,
                    Descripcion = x.EstadoIngresosBrutosCoeficienteUnificado.Descripcion
                },
                Secuencia = x.SecuenciaIngresosBrutosCoeficienteUnificado_Id == null ?
                    null :
                    new SecuenciaIngresosBrutosCoeficienteUnificadoDto()
                    {
                        Id = x.SecuenciaIngresosBrutosCoeficienteUnificado.Id,
                        Descripcion = x.SecuenciaIngresosBrutosCoeficienteUnificado.Descripcion
                    },
                Anticipo = x.Anticipo,
                CUIT = x.CUIT,
                FechaCarga = x.FechaCarga,
                FechaUltimaModificacion = x.FechaUltimaModificacion,
                Sede = x.Sede,
                MalCargada = x.MalCargada,
                SecuenciaId = x.SecuenciaIngresosBrutosCoeficienteUnificado_Id,
                ConsultaId = x.Consulta_Id,
                RazonSocial = x.RazonSocial,
            }, null, 0, "Id", SustitucionMOAModel.Consultas.DirOrden.Desc);
        }

        public IList<IngresosBrutosCoeficienteUnificadoDetalleDto> ListarDetalles(int idCabecera)
        {
            return repositorio.Listar<IngresosBrutosCoeficienteUnificadoDetalle, IngresosBrutosCoeficienteUnificadoDetalleDto>(
            x => new IngresosBrutosCoeficienteUnificadoDetalleDto
            {
                Id = x.Id,
                IdCabecera = x.IngresosBrutosCoeficienteUnificado_Id,
                Jurisdiccion = x.Jurisdiccion,
                NumeroJurisdiccion = x.NumeroJurisdiccion,
                FechaCese = x.FechaCese,
                FechaInicio = x.FechaInicio,
                CoeficienteIngresos = x.CoeficienteIngresos,
                CoeficienteGastos = x.CoeficienteGastos,
                CoeficienteUnificado = x.CoeficienteUnificado,
                FechaUltimaModificacion = x.FechaUltimaModificacion
            },
            x => x.IngresosBrutosCoeficienteUnificado_Id == idCabecera);
        }

        public EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto EditarIngresosBrutosCoeficienteUnificadoDetalle(IngresosBrutosCoeficienteUnificadoDetalleDto ingresosBrutosCoeficienteUnificadoDetalleDto)
        {
            var ingresosBrutosCoeficienteUnificadoDetalle = repositorio.Obtener<IngresosBrutosCoeficienteUnificadoDetalle>(ingresosBrutosCoeficienteUnificadoDetalleDto.Id);

            if (ingresosBrutosCoeficienteUnificadoDetalle == null)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "registros de detalles de coeficientes unificados"));

            ingresosBrutosCoeficienteUnificadoDetalle.CoeficienteGastos = ingresosBrutosCoeficienteUnificadoDetalleDto.CoeficienteGastos;
            ingresosBrutosCoeficienteUnificadoDetalle.CoeficienteIngresos = ingresosBrutosCoeficienteUnificadoDetalleDto.CoeficienteIngresos;
            ingresosBrutosCoeficienteUnificadoDetalle.CoeficienteUnificado = ingresosBrutosCoeficienteUnificadoDetalleDto.CoeficienteUnificado;
            ingresosBrutosCoeficienteUnificadoDetalle.FechaCese = ingresosBrutosCoeficienteUnificadoDetalleDto.FechaCese;
            ingresosBrutosCoeficienteUnificadoDetalle.FechaInicio = ingresosBrutosCoeficienteUnificadoDetalleDto.FechaInicio;
            ingresosBrutosCoeficienteUnificadoDetalle.NumeroJurisdiccion = ingresosBrutosCoeficienteUnificadoDetalleDto.NumeroJurisdiccion;
            ingresosBrutosCoeficienteUnificadoDetalle.Jurisdiccion = ingresosBrutosCoeficienteUnificadoDetalleDto.Jurisdiccion;
            ingresosBrutosCoeficienteUnificadoDetalle.FechaUltimaModificacion = timeProvider.Now();

            repositorio.GuardarCambios();

            return new EditarIngresosBrutosCoeficienteUnificadoDetalleResponseDto
            {
                Mensaje = SuccessMsg.IngresosBrutosCoeficienteUnificadoDetalleActualizadoOK,
                FechaUltimaModificacion = ingresosBrutosCoeficienteUnificadoDetalle.FechaUltimaModificacion
            };
        }

        public EditarIngresosBrutosCoeficienteUnificadoResponseDto EditarIngresosBrutosCoeficienteUnificado(IngresosBrutosCoeficienteUnificadoDto ingresosBrutosCoeficienteUnificadoDto)
        {
            var ingresosBrutosCoeficienteUnificado = repositorio.Obtener<IngresosBrutosCoeficienteUnificado>(ingresosBrutosCoeficienteUnificadoDto.Id);

            if (ingresosBrutosCoeficienteUnificado == null)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "registros de coeficientes unificados"));

            var estado = repositorio.Obtener<EstadoIngresosBrutosCoeficienteUnificado>(ingresosBrutosCoeficienteUnificadoDto.EstadoId);

            if (estado == null)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "registros de estados."));

            ingresosBrutosCoeficienteUnificado.CUIT = ingresosBrutosCoeficienteUnificadoDto.CUIT;
            ingresosBrutosCoeficienteUnificado.Anticipo = ingresosBrutosCoeficienteUnificadoDto.Anticipo;
            ingresosBrutosCoeficienteUnificado.Sede = ingresosBrutosCoeficienteUnificadoDto.Sede;
            ingresosBrutosCoeficienteUnificado.MalCargada = ingresosBrutosCoeficienteUnificadoDto.MalCargada;
            ingresosBrutosCoeficienteUnificado.SecuenciaIngresosBrutosCoeficienteUnificado_Id = ingresosBrutosCoeficienteUnificadoDto.SecuenciaId;
            ingresosBrutosCoeficienteUnificado.RazonSocial = ingresosBrutosCoeficienteUnificadoDto.RazonSocial;
            ingresosBrutosCoeficienteUnificado.EstadoIngresosBrutosCoeficienteUnificado = estado;
            ingresosBrutosCoeficienteUnificado.EstadoIngresosBrutosCoeficienteUnificado_Id = ingresosBrutosCoeficienteUnificadoDto.EstadoId;
            ingresosBrutosCoeficienteUnificado.FechaUltimaModificacion = timeProvider.Now();

            repositorio.GuardarCambios();

            return new EditarIngresosBrutosCoeficienteUnificadoResponseDto
            {
                Mensaje = SuccessMsg.IngresosBrutosCoeficienteUnificadoCabeceraActualizadoOK,
                FechaUltimaModificacion = ingresosBrutosCoeficienteUnificado.FechaUltimaModificacion,
                estadoCabecera = new EstadoIngresosBrutosCoeficienteUnificadoDto(estado)
            };
        }

        public string AutorizarCabecera(int idCabecera, string mailUsuario)
        {
            var cabecera = this.repositorio.Obtener<IngresosBrutosCoeficienteUnificado>(idCabecera);

            if (cabecera == null)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "registros de cabecera de coeficientes unificados"));

            cabecera.EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado;
            cabecera.FechaUltimaModificacion = timeProvider.Now();

            Usuario usuario = repositorio.Obtener<Usuario>(usr => usr.Mail == mailUsuario);

            repositorio.GuardarCambios();

            return (SuccessMsg.IngresosBrutosCoeficienteUnificadoAutorizado);
        }

        public string ObtenerRutaArchivoFormularioCM05(int idCabecera)
        {
            IngresosBrutosCoeficienteUnificado cabecera = repositorio.Obtener<IngresosBrutosCoeficienteUnificado>(idCabecera);

            return cabecera.Archivo.Ruta;
        }

        public IList<MovimientoIngresosBrutosCoeficienteUnificadoDto> ListarMovimientos(int idCabecera)
        {
            var result = repositorio.Listar<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>(
                x => new MovimientoIngresosBrutosCoeficienteUnificadoDto
                {
                    Id = x.Id,
                    IngresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificadoDto { Id = x.IngresosBrutosCoeficienteUnificado_Id },
                    Observaciones = x.Observaciones,
                    Fecha = x.Fecha,
                    TipoId = x.TipoMovimientoIngresosBrutosCoeficienteUnificado_Id,
                    OrigenId = x.OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id,
                    EstadoAnteriorId = x.EstadoAnterior_Id,
                    EstadoPosteriorId = x.EstadoPosterior_Id,
                },
                x => x.IngresosBrutosCoeficienteUnificado_Id == idCabecera);

            return result;
        }

        public List<EstadoIngresosBrutosCoeficienteUnificadoDto> ListarEstados()
        {
            var estados = repositorio.Listar<EstadoIngresosBrutosCoeficienteUnificado>()
                .Select(e => new EstadoIngresosBrutosCoeficienteUnificadoDto(e)).OrderBy(x => x.Descripcion).ToList();

            if (estados.Count < 1 || estados == null)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "registros de estados"));

            return estados;
        }

        public List<SecuenciaIngresosBrutosCoeficienteUnificadoDto> ListarSecuenciaIngresosBrutosCoeficientesUnificador()
        {
            var secuencias = repositorio.Listar<SecuenciaIngresosBrutosCoeficienteUnificado>()
                .Select(e => new SecuenciaIngresosBrutosCoeficienteUnificadoDto(e)).OrderBy(x => x.Descripcion).ToList();

            if (secuencias.Count < 1 || secuencias == null)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "registros de secuencias"));

            return secuencias;
        }

        public IList<MovimientoIngresosBrutosCoeficienteUnificadoDto> InsertarMovimientoIngresosBrutosCoeficienteUnificado(MovimientoIngresosBrutosCoeficienteUnificadoCustomDto movimientoIngresosBrutosCoeficienteUnificadoCustomDto)
        {
            var ingresosBrutosCoeficienteUnificado = repositorio.Obtener<IngresosBrutosCoeficienteUnificado>(movimientoIngresosBrutosCoeficienteUnificadoCustomDto.IdIngreso);
            MovimientoIngresosBrutosCoeficienteUnificado movimientoIngresosBrutosCoeficienteUnificado = new MovimientoIngresosBrutosCoeficienteUnificado
            {
                IngresosBrutosCoeficienteUnificado_Id = movimientoIngresosBrutosCoeficienteUnificadoCustomDto.IdIngreso,
                Observaciones = $"{movimientoIngresosBrutosCoeficienteUnificadoCustomDto.Accion} por: {movimientoIngresosBrutosCoeficienteUnificadoCustomDto.Persona}",
                Fecha = timeProvider.Now(),
                TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = movimientoIngresosBrutosCoeficienteUnificadoCustomDto.Tipo,
                OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = movimientoIngresosBrutosCoeficienteUnificadoCustomDto.Origen,
                EstadoAnterior_Id = ingresosBrutosCoeficienteUnificado.EstadoIngresosBrutosCoeficienteUnificado_Id,
                EstadoPosterior_Id = movimientoIngresosBrutosCoeficienteUnificadoCustomDto.EstadoNuevo,
            };
            repositorio.Agregar(movimientoIngresosBrutosCoeficienteUnificado);
            repositorio.GuardarCambios();

            var result = repositorio.Listar<MovimientoIngresosBrutosCoeficienteUnificado, MovimientoIngresosBrutosCoeficienteUnificadoDto>(
                x => new MovimientoIngresosBrutosCoeficienteUnificadoDto
                {
                    Id = x.Id,
                    IngresosBrutosCoeficienteUnificado = new IngresosBrutosCoeficienteUnificadoDto { Id = x.IngresosBrutosCoeficienteUnificado_Id },
                    Observaciones = x.Observaciones,
                    Fecha = x.Fecha,
                    TipoId = x.TipoMovimientoIngresosBrutosCoeficienteUnificado_Id,
                    OrigenId = x.OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id,
                    EstadoAnteriorId = x.EstadoAnterior_Id,
                    EstadoPosteriorId = x.EstadoPosterior_Id,
                },
                x => x.IngresosBrutosCoeficienteUnificado_Id == movimientoIngresosBrutosCoeficienteUnificadoCustomDto.IdIngreso);

            return result;
        }
 
        public void ActualizarCM05(int consultaId, Usuario usuario)
        {
            var ingresoBrutoCU = this.repositorio.Obtener<IngresosBrutosCoeficienteUnificado>(i => i.Consulta_Id == consultaId);
            if (ingresoBrutoCU == null)
            {
                throw new InfoCustomException($"No se encontró la gestion de CM05 para la consulta nro: {consultaId}.");
            }
            ingresoBrutoCU.EstadoIngresosBrutosCoeficienteUnificado_Id = 3;

            var movimientoIngresosBrutosCoeficienteUnificado = new MovimientoIngresosBrutosCoeficienteUnificado
            {
                IngresosBrutosCoeficienteUnificado_Id = ingresoBrutoCU.Id,
                Observaciones = $"Cambió de estado por usuario: {usuario.Mail}",
                Fecha = timeProvider.Now(),
                TipoMovimientoIngresosBrutosCoeficienteUnificado_Id = 6,
                OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id = 1,
                EstadoAnterior_Id = ingresoBrutoCU.EstadoIngresosBrutosCoeficienteUnificado_Id,
                EstadoPosterior_Id = 3,
            };
            repositorio.Agregar(movimientoIngresosBrutosCoeficienteUnificado);
            repositorio.GuardarCambios();
        }

    }
}