using SustitucionMOAAssets;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class OrdenResiduosService : IOrdenResiduosService
    {
        protected readonly IRepositorioOrdenResiduos repositorio;
        protected readonly IFeriadoService feriadoService;
        protected readonly IEmailResiduosService emailResiduosService;

        public OrdenResiduosService(IRepositorioOrdenResiduos repositorio, IFeriadoService feriadoService, IEmailResiduosService emailResiduosService)
        {
            this.repositorio = repositorio;
            this.feriadoService = feriadoService;
            this.emailResiduosService = emailResiduosService;
        }

        public MaterialDto[] ObtenerMateriales()
        {
            return repositorio.ObtenerMateriales();
        }

        public ListarOrdenesResiduosResponse ObtenerListadoOrdenes(string fechaInicioStr, string fechaFinStr)
        {
            var fechaInicio = DataFormatter.StringToDateTime(fechaInicioStr, "");
            var fechaFin = DataFormatter.StringToDateTime(fechaFinStr, "");

            var listado = repositorio.ObtenerListadoOrdenes(fechaInicio, fechaFin);

            if (listado == null || listado.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "órdenes de carga de residuos e insumos"));
            }

            return new ListarOrdenesResiduosResponse
            {
                ListaOrdenes = listado
            };
        }

        public void VerificarVencimientoOrdenesResiduos()
        {
            var fechaActual = DateTime.Now;
            var dayOfWeek = fechaActual.DayOfWeek;
            if ((dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday))
            {
                return;
            }
            var feriados = feriadoService.ObtenerFeriados();
            if (feriados.Any(feriado=>feriado.Date == fechaActual.Date))
            {
                return;
            }

            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VencimientoOrdenesResiduosJob");
            if (!habilitacion.Habilitado)
            {
                return;
            }

            var ordenes = 
                repositorio.Listar<OrdenResiduos>(o => o.EstadoId == (int)EstadoOrdenResiduosEnum.OrdenGenerada)
                .Where(orden => orden.FechaVencimiento(feriados) < fechaActual);
            
            foreach (var orden in ordenes)
            {
                orden.EstadoId = (int)EstadoOrdenResiduosEnum.OrdenVencida;
            }
            repositorio.GuardarCambios();

            emailResiduosService.EnviarMailOrdenesVencidas(ordenes);
        }
    }
}
