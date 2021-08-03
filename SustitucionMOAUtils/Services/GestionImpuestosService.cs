using iTextSharp.text;
using iTextSharp.text.pdf;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Email;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Interfaces.Helpers;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class GestionImpuestosService : IGestionImpuestosService
    {
        private readonly IRepositorio repositorio;
        private readonly ITimeProvider timeProvider;

        public GestionImpuestosService(IRepositorio repositorio, ITimeProvider timeProvider)
        {
            this.repositorio = repositorio;
            this.timeProvider = timeProvider;
        }

        public IList<IngresosBrutosCoeficienteUnificadoDto> ListarCabeceras()
        {
            return repositorio.Listar<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>(
            x => new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = x.Id,
                EstadoId = x.EstadoIngresosBrutosCoeficienteUnificado.Id,
                Anticipo = x.Anticipo,
                CUIT = x.CUIT,
                FechaCarga = x.FechaCarga,
                FechaUltimaModificacion = x.FechaUltimaModificacion,
                Sede = x.Sede
            });
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

            if(ingresosBrutosCoeficienteUnificadoDetalle == null)
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
        
        public string AutorizarCabecera(int idCabecera)
        {
            var cabecera = this.repositorio.Obtener<IngresosBrutosCoeficienteUnificado>(idCabecera);

            if (cabecera == null)
                throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "registros de cabecera de coeficientes unificados"));

            cabecera.EstadoIngresosBrutosCoeficienteUnificado_Id = (int)EnumEstadoIngresosBrutosCoeficienteUnificado.Autorizado;
            cabecera.FechaUltimaModificacion = timeProvider.Now();

            repositorio.GuardarCambios();

            return (SuccessMsg.IngresosBrutosCoeficienteUnificadoAutorizado);
        }
    }
}
