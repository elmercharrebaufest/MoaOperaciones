using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.OrdenResiduos;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using SustitucionMOAModel.Enums;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class ExternalApiOrdenesResiduosService : IExternalApiOrdenesResiduosService
    {
        private readonly IRepositorio repositorio;

        public ExternalApiOrdenesResiduosService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<OrdenResiduosApiDto> ObtenerOrdenes(string patenteChasis = null)
        {
            var listado = repositorio
                .Listar<OrdenResiduos>(or =>
                    (string.IsNullOrEmpty(patenteChasis) || or.PatenteChasis == patenteChasis) &&
                    or.EstadoId == (int)EstadoOrdenResiduosEnum.OrdenGenerada)
                .Select(or =>
                    new OrdenResiduosApiDto(or));

            return listado.ToList();
        }

        public void ActualizarOrden(ActualizarOrdenResiduosExternalDto datos)
        {
            var orden = repositorio.Obtener<OrdenResiduos>(datos.Id);

            if (orden == null)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinDatos, "actualizar", "orden de residuos."));
            }

            switch (datos.TipoActualizacion)
            {
                case FlujoActualizacionOrdenResiduos.Ingreso:
                    orden.FechaIngreso = datos.Fecha;
                    orden.EstadoId = (int)EstadoOrdenResiduosEnum.Ingresada;
                    break;
                case FlujoActualizacionOrdenResiduos.Salida:
                    orden.FechaEgreso = datos.Fecha;
                    orden.EstadoId = (int)EstadoOrdenResiduosEnum.Retirada;
                    break;
                case FlujoActualizacionOrdenResiduos.Rechazo:
                    orden.MotivoRechazo = datos.MotivoRechazo;
                    orden.EstadoId = (int)EstadoOrdenResiduosEnum.Rechazada;
                    break;
            }

            repositorio.GuardarCambios();
        }
        public void InformarViaje(IngresosEgresosResiduos ingresosEgresos)
        {
            var orden = repositorio.Obtener<OrdenResiduos>(ingresosEgresos.IdOperaciones);
            if (orden == null)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinDatos, "informar", "orden de residuos."));
            }

            orden.FechaIngreso = ingresosEgresos.FechaEntrada;
            orden.FechaEgreso = ingresosEgresos.FechaSalida;
            orden.PesadaTara = ingresosEgresos.PesadaTara;
            orden.PesadaNeto = ingresosEgresos.PesadaNeto;
            orden.PesadaBruto = ingresosEgresos.PesadaBruto;
            orden.NroCertificacion = ingresosEgresos.NroCertificacion;
            orden.Balanza = ingresosEgresos.Balanza;
            orden.IdScato = ingresosEgresos.OrdenCargaInterna;
            orden.UniMedCant = ingresosEgresos.UniMedCant;
            orden.EstadoId = (int)EstadoOrdenResiduosEnum.OrdenEntregada;
            repositorio.GuardarCambios();
        }
    }
}
