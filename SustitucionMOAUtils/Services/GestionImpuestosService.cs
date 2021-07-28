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

        public GestionImpuestosService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public IList<IngresosBrutosCoeficienteUnificadoDto> ListarCabeceras()
        {
            return repositorio.Listar<IngresosBrutosCoeficienteUnificado, IngresosBrutosCoeficienteUnificadoDto>(
            x => new IngresosBrutosCoeficienteUnificadoDto
            {
                Id = x.Id,
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
                Jurisdiccion = x.Jurisdiccion,
                NumeroJurisdiccion = x.NumeroJurisdiccion,
                FechaCese = x.FechaCese,
                FechaInicio = x.FechaInicio,
                CoeficienteIngresos = x.CoeficienteIngresos,
                CoeficienteGastos = x.CoeficienteGastos,
                CoeficienteUnificado = x.CoeficienteUnificado,
                FechaUltimaModificacion = x.FechaUltimaModificacion,
                Editar = false
            },
            x => x.IngresosBrutosCoeficienteUnificado_Id == idCabecera);
        }

        public string EditarDetalles(IngresosBrutosCoeficienteUnificadoDetalle coeficientes)
        {
            var coeficienteEditar = repositorio.Obtener<IngresosBrutosCoeficienteUnificadoDetalle>(x => x.Id == coeficientes.Id);

            if(coeficienteEditar == null) throw new InfoCustomException(String.Format(InfoMsg.SinRegistros, "coeficiente unificado"));

            coeficienteEditar.FechaUltimaModificacion = DateTime.Now;
            coeficienteEditar.CoeficienteGastos = coeficientes.CoeficienteGastos;
            coeficienteEditar.CoeficienteIngresos = coeficientes.CoeficienteIngresos;
            coeficienteEditar.CoeficienteUnificado = coeficientes.CoeficienteUnificado;
            coeficienteEditar.FechaCese = coeficientes.FechaCese;
            coeficienteEditar.FechaInicio = coeficientes.FechaInicio;
            coeficienteEditar.NumeroJurisdiccion = coeficientes.NumeroJurisdiccion;
            coeficienteEditar.IngresosBrutosCoeficienteUnificado = coeficientes.IngresosBrutosCoeficienteUnificado;

            repositorio.GuardarCambios();

            return (SuccessMsg.CommodityActualizacionOK);
        }
    }
}
