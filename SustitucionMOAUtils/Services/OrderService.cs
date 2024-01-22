using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Linq;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using System.Data;
using Comunicacion = SustitucionMOAModel.Entities.Comunicacion;
using System.Reflection;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Consultas;
using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;

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
            try
            {
                List<DetalleOrdenDeCompraDto> result = ServicioSAP_OrdenesCompraCabeceras(parametros);

                if (!string.IsNullOrEmpty(parametros.ColumnaOrden))
                    result = OrdenarOrdenesCompra(result, parametros.ColumnaOrden, parametros.OrdenAscendente);

                return PaginarResultados(result, parametros.pagina, parametros.elementosPorPagina);
            }
            catch (Exception e) when (e is InfoCustomException || e is ValidationCustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new WSCustomException(ErrorMsg.ErrorWS, e);
            }
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
                //resultado = resultados.Skip(indiceInicial).Take(elementosPorPaginaValidos).ToList();

                return new ListaPaginada<DetalleOrdenDeCompraDto>(resultados.ToList(), paginaValida, elementosPorPaginaValidos, itemsTotales);

                //return resultados.Skip(indiceInicial).Take(elementosPorPaginaValidos).ToList();
            //}
            //else
            //{
            //    return new ListaPaginada<DetalleOrdenDeCompraDto>(); // Si la página solicitada está fuera de rango, devuelve una lista vacía
            //}
        }


        // Consultas a servicio SAP con distintos criterios de busqueda
        public List<DetalleOrdenDeCompraDto> ServicioSAP_OrdenesCompraCabeceras(OrderParamsDto parametros)
        {
            List<OrdenCompraDto> ordenesCompra = new List<OrdenCompraDto>();
            // Si la consulta no tiene un número de orden de compra, se obtienen todas las ordenes de compra en el rango de fechas
            if (parametros.OrdenCompraId == null)
            {
                ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros);
            }
            else
            {
                if (long.TryParse(parametros.OrdenCompraId, out long ordenCompraId))
                {
                    OrdenCompraDto nuevaOrden = new OrdenCompraDto
                    {
                        Id = ordenCompraId,
                    };

                    ordenesCompra.Add(nuevaOrden);
                }
            }

            //List<TablaSap> centros = repositorio.Listar<TablaSap>(a => a.Tabla == "Centro");
            //List<TablaSap> almacenes = repositorio.Listar<TablaSap>(a => a.Tabla == "Almacen");
            List<TablaSap> centros = new List<TablaSap>();
            List<TablaSap> almacenes = new List<TablaSap>();


            //Recorro las ordenes de compra y obtengo el detalle de cada una
            List< DetalleOrdenDeCompraDto> result = new List<DetalleOrdenDeCompraDto>();
            foreach (var ordenCompra in ordenesCompra)
            {
                string nroOC = ordenCompra.Id.ToString();
                

                // Obtengo detalle de una OC //
                DetalleOrdenDeCompraDto detalleOrdendeCompra = new ObtenerOrdenDeCompraConsumerMOA(repositorio).ObtenerDetalleDeOrdenDeCompra(nroOC, centros, almacenes);

                detalleOrdendeCompra.NombreProveedor = ordenCompra.ProveedorNombre; 
                detalleOrdendeCompra.MonedaDescripcion = ordenCompra.MonedaDescripcion; 

                result.Add(detalleOrdendeCompra);
            }

            return result;
        }
    }
}
