using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.ViewModel.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Formulario;
using SustitucionMOAModel.Models.WSMapMOA.ContactoMail;
using SustitucionMOAModel.Models.WSMapMOA.Liquidacion;
using SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle;
using SustitucionMOARepositorio;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.ServiceModel.Channels;
using Comunicacion = SustitucionMOAModel.Entities.Comunicacion;
using SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection;
using SustitucionMOAWS.ObtenerEntradaDeServicioPorNumeroWebServiceMOA;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Consultas;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAWS.ScatoComandosWebService;

namespace SustitucionMOAUtils.Services

{
    public class OrderService : IOrderService
    {
        protected readonly IRepositorio repositorio;
        private readonly IConsultaService consultaService;
        //private readonly ILiquidacionService _liquidacionService;
        //private OrderParamsDto parametros;

        public OrderService(IConsultaService consultaService, IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.consultaService = consultaService;
            //_liquidacionService = liquidacionService;
        }

        public ListaPaginada<DetalleOrdenDeCompraDto> ObtenerOrdenesCompraConDetalle(OrderParamsDto parametros)
        {
            List<DetalleOrdenDeCompraDto> result = ServicioSAP_OrdenesCompraCabeceras(parametros);

            // Ordena si se proporciona la columna de orden y el tipo de orden
            if (!string.IsNullOrEmpty(parametros.ColumnaOrden))
                result = OrdenarOrdenesCompra(result, parametros.ColumnaOrden, parametros.OrdenAscendente);

            // Realiza la paginación
            var response = PaginarResultados(result, parametros.pagina, parametros.elementosPorPagina);

            return response;
        }

        /// <summary>
        /// Realiza ordenamiento del objeto OrdenCompraDto según la columna y el tipo de orden especificados
        /// </summary>
        public List<DetalleOrdenDeCompraDto> OrdenarOrdenesCompra(List<DetalleOrdenDeCompraDto> ordenes, string columnaOrden, bool ordenAscendente)
        {
            if (string.IsNullOrEmpty(columnaOrden))
                return ordenes; // Sin ordenación si no se especifica una columna

            // reflexión para obtener la propiedad de la columna
            var propiedadOrden = typeof(DetalleOrdenDeCompraDto).GetProperty(columnaOrden, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propiedadOrden == null)
                return ordenes; // Si la propiedad no se encuentra, no se realiza ordenación

            // Ordenar la lista según la propiedad especificada y el orden ascendente o descendente
            var orderedOrdenes = ordenAscendente
                ? ordenes.OrderBy(o => propiedadOrden.GetValue(o, null))
                : ordenes.OrderByDescending(o => propiedadOrden.GetValue(o, null));

            return orderedOrdenes.ToList();
        }

        /// <summary>
        /// Pagina los resultados de la lista de ordenes de compra
        /// </summary>
        public ListaPaginada<DetalleOrdenDeCompraDto> PaginarResultados(List<DetalleOrdenDeCompraDto> resultados, int pagina, int? elementosPorPagina)
        {
            // Establecer valores predeterminados si son nulos o inválidos
            int itemsTotales = resultados.Count();
            List<DetalleOrdenDeCompraDto> resultado;

            int paginaValida =  pagina > 0 ? pagina : 1;
            int elementosPorPaginaValidos = (elementosPorPagina.HasValue && elementosPorPagina.Value > 0) ? elementosPorPagina.Value : 5;

            int indiceInicial = (paginaValida - 1) * elementosPorPaginaValidos;
            //if (indiceInicial >= 0 && indiceInicial < resultados.ToList().Count)
            //{
                //resultado = resultado.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);
                resultado = resultados.Skip(indiceInicial).Take(elementosPorPaginaValidos).ToList();

                return new ListaPaginada<DetalleOrdenDeCompraDto>(resultado.ToList(), paginaValida, elementosPorPaginaValidos, itemsTotales);

                //return resultados.Skip(indiceInicial).Take(elementosPorPaginaValidos).ToList();
            //}
            //else
            //{
            //    return new ListaPaginada<DetalleOrdenDeCompraDto>(); // Si la página solicitada está fuera de rango, devuelve una lista vacía
            //}
        }


        // Consultas a servicios SAP con distintos de busqueda
        public List<DetalleOrdenDeCompraDto> ServicioSAP_OrdenesCompraCabeceras(OrderParamsDto parametros)
        {
            //Obtiene Cabeceras de Ordenes de Compra
            List<OrdenCompraDto> ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros);

            // Filtra por número de orden de compra, si se proporciona el parámetro
            if (parametros.OrdenCompraId != null)
                ordenesCompra = ordenesCompra.Where(orden => orden.Id.ToString() == parametros.OrdenCompraId).ToList();

            //Recorro las ordenes de compra y obtengo el detalle de cada una
            List< DetalleOrdenDeCompraDto> result = new List<DetalleOrdenDeCompraDto>();
            foreach (var ordenCompra in ordenesCompra)
            {
                string nroOC = ordenCompra.Id.ToString();
                
                //Solo pruebas de desarrollo, luego se debe eliminar.
                if (nroOC == "4123001500" || nroOC == "4123001336" || nroOC == "4123001899" || nroOC == "4123001916" || nroOC == "4123001874") {  }

                // Obtengo detalle de una OC
                DetalleOrdenDeCompraDto detalleOrdendeCompra = new ObtenerOrdenDeCompraConsumerMOA(repositorio).ObtenerDetalleDeOrdenDeCompra(nroOC);
                detalleOrdendeCompra.NombreProveedor = ordenCompra.ProveedorNombre;
                detalleOrdendeCompra.MonedaDescripcion = ordenCompra.MonedaDescripcion;

                result.Add(detalleOrdendeCompra);
            }

            return result;
        }
    }
}
