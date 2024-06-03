using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOARepositorio;
using System.Data;
using System.Data.Entity;
using System.Reflection;
using SustitucionMOAModel.Dto.OrdenesCompra;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using SustitucionMOAUtils.Services;
using static SustitucionMOAWS.WSConsumers.CrearEntradaDeServicioConsumerMOA;
using System;

namespace SustitucionMOAUtils.Services

{
    public class EntradaServicioService : IEntradaServicioService
    {
        protected readonly IRepositorio repositorio;
        private readonly IConsultaService consultaService;
        //private readonly ILiquidacionService _liquidacionService;
        //private OrderParamsDto parametros;

        public EntradaServicioService(IConsultaService consultaService, IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.consultaService = consultaService;
            //_liquidacionService = liquidacionService;
        }

        public async Task<List<EntradaServicioCabeceraDto>> ObtenerEntradasServicioCompleta(EntradaServicioParamsDto parametros)
        {
            List<EntradaServicioCabeceraDto> Documentos = await ServicioSAP_EntradasServicioCabecera(parametros);

            //Ordena si se proporciona la columna de orden y el tipo de orden
            if (!string.IsNullOrEmpty(parametros.ColumnaOrden))
                Documentos = OrdenarEntradasServicio(Documentos, parametros.ColumnaOrden, parametros.OrdenAscendente);

            Documentos = PaginarResultados(Documentos, parametros.pagina, parametros.elementosPorPagina); //Realiza la paginación

            return Documentos;
        }

        /// <summary>
        /// Realiza ordenamiento del objeto OrdenCompraDto según la columna y el tipo de orden especificados
        /// </summary>
        public List<EntradaServicioCabeceraDto> OrdenarEntradasServicio(List<EntradaServicioCabeceraDto> ordenes, string columnaOrden, bool ordenAscendente)
        {
            if (string.IsNullOrEmpty(columnaOrden))
                return ordenes; // Sin ordenamiento. Si no se especifica una columna

            // reflexión para obtener la propiedad de la columna
            var propiedadOrden = typeof(EntradaServicioCabeceraDto).GetProperty(columnaOrden, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propiedadOrden == null)
                return ordenes; // Si la propiedad no se encuentra, no se realiza ordenación

            // Ordenar la lista según la propiedad especificada y el orden ascendente o descendente
            var orderedDocument = ordenAscendente
                ? ordenes.OrderBy(o => propiedadOrden.GetValue(o, null))
                : ordenes.OrderByDescending(o => propiedadOrden.GetValue(o, null));

            //return orderedOrdenes.ToList();
            return orderedDocument.ToList();
        }

        /// <summary>
        /// Pagina los resultados de la lista de ordenes de compra
        /// </summary>
        public List<EntradaServicioCabeceraDto> PaginarResultados(List<EntradaServicioCabeceraDto> resultados, int? pagina, int? elementosPorPagina)
        {
            // Establecer valores predeterminados si son nulos o inválidos
            int paginaValida = (pagina.HasValue && pagina.Value > 0) ? pagina.Value : 1;
            int elementosPorPaginaValidos = (elementosPorPagina.HasValue && elementosPorPagina.Value > 0) ? elementosPorPagina.Value : 5;

            int indiceInicial = (paginaValida - 1) * elementosPorPaginaValidos;
            if (indiceInicial >= 0 && indiceInicial < resultados.Count)
            {
                return resultados.Skip(indiceInicial).Take(elementosPorPaginaValidos).ToList();
            }
            else
            {
                return new List<EntradaServicioCabeceraDto>(); // Si la página solicitada está fuera de rango, devuelve una lista vacía
            }
        }


        /// <summary>
        /// Consultas SAP cabecera de documento.
        /// No lista las entradas de servicio que esté en estado "Borrada"
        /// </summary>
        /// <param name="parametros"></param>
        /// <returns></returns>
        public async Task<List<EntradaServicioCabeceraDto>> ServicioSAP_EntradasServicioCabecera(EntradaServicioParamsDto parametros)
        {
            //Obtiene Cabeceras de Entradas de Servicio
            List<EntradaServicioCabeceraDto> EntradasServicioCabecera = await new ObtenerCabecerasEntradaServicioConsumerMOA().ObtenerEntradasServicioCabeceraAsync(parametros.FechaInicio);

            List<EntradaServicioCabeceraDto> EntradasServicio = new List<EntradaServicioCabeceraDto>();

            //Se filtran por las OC tomando las que empiezan con 412
            EntradasServicioCabecera = EntradasServicioCabecera.Where(x => x.OrdenCompra.StartsWith("412")).ToList();

            // Filtra por número de documento, si se proporciona el parámetro
            if (parametros.DocumentoNumero != null)
                EntradasServicioCabecera = EntradasServicioCabecera.Where(orden => orden.EntradaServicio.ToString() == parametros.DocumentoNumero).ToList();

            //Recorre los documentos y obteniene el detalle de cada uno
            List< EntradaServicioDto> result = new List<EntradaServicioDto>();
            foreach (var documento in EntradasServicioCabecera)
            {
                string nroDoc = documento.EntradaServicio.ToString();

                // Se obtiene detalle de una ES
                List<EntradaServicioDetalleDto> entradasServicioDetalleSAP = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicioDetalle(nroDoc);

                documento.entradaServicioDetalle = entradasServicioDetalleSAP;

                EntradasServicio.Add(documento);

            }
            return EntradasServicio;
        }


        /// <summary>
        /// Borrar Entrada de Servicio indicando su número de documento
        /// Actualmente hay varias incognicas con respecto a las condiciones que debe cumplir una ES para poder ser borrada
        /// </summary>
        /// <returns></returns>
        public string BorrarEntradaServicio(EntradaServicioParamsDto parametros)
        {
            string fechaContabilizacion = parametros.FechaContabilizacion;
            DateTime FechaContabilizacionToDateTime = Convert.ToDateTime(fechaContabilizacion).ToUniversalTime();
            int currentMonth = DateTime.UtcNow.Month;
            int currentYear = DateTime.UtcNow.Year;
            int lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            int lastYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            if (FechaContabilizacionToDateTime.Year == lastYear && FechaContabilizacionToDateTime.Month == lastMonth)
                fechaContabilizacion = DateTime.UtcNow.ToString("yyyy-MM-dd");

            string result = new BorrarEntradaServicioConsumerMOA().BorrarEntradaServicio(parametros.DocumentoNumero, fechaContabilizacion);

            return result;
        }


        public string CrearEntradaServicioAnt(EntradaServicioCreateParamsDto parametros)
        {
            string result = new CrearEntradaDeServicioConsumerMOA().CrearEntradaServicio(parametros);

            return result;
        }


        public async Task<EntradaServicioCreateRespuestaDto> CrearEntradaServicio(EntradaServicioCreateParamsDto parametros)
        {
            // 3 - Si alguna de las validaciones es correcta, alta automatica.
            EntradaServicioCreateRespuestaDto result = await new CrearEntradaDeServicioConsumerMOA().CrearEntradaServicioAsync(parametros);
            return result;
        }

    }
}

