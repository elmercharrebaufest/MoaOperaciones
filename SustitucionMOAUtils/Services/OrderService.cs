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

        public List<OrdenCompraDto> ObtenerOrdenesCompraPorProveedor(OrderParamsDto parametros)
        {
            //Obtiene OC, todo el arbol completo.
            List<OrdenCompraDto> result = ServicioSAP_OrdenesCompraCompleto(parametros);

            // Filtra si se proporciona nroOC
            if (!string.IsNullOrEmpty(parametros.OrdenCompraId))
                result = result.Where(orden => orden.OrdenCompraId.ToString() == parametros.OrdenCompraId).ToList();

            // Ordena si se proporciona la columna de orden y el tipo de orden
            if (!string.IsNullOrEmpty(parametros.ColumnaOrden))
                result = OrdenarOrdenesCompra(result, parametros.ColumnaOrden, parametros.OrdenAscendente);

            // Realiza la paginación
            result = PaginarResultados(result, parametros.pagina, parametros.elementosPorPagina);

            return result;
        }

        /// <summary>
        /// Realiza ordenamiento del objeto OrdenCompraDto según la columna y el tipo de orden especificados
        /// </summary>
        public List<OrdenCompraDto> OrdenarOrdenesCompra(List<OrdenCompraDto> ordenes, string columnaOrden, bool ordenAscendente)
        {
            if (string.IsNullOrEmpty(columnaOrden))
                return ordenes; // Sin ordenación si no se especifica una columna

            // reflexión para obtener la propiedad de la columna
            var propiedadOrden = typeof(OrdenCompraDto).GetProperty(columnaOrden, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

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
        public List<OrdenCompraDto> PaginarResultados(List<OrdenCompraDto> resultados, int? pagina, int? elementosPorPagina)
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
                // Si la página solicitada está fuera de rango, devuelve una lista vacía
                return new List<OrdenCompraDto>();
            }
        }


        // Consultas a servicios SAP para distintos datos de ordenes de compra
        public List<OrdenCompraDto> ServicioSAP_OrdenesCompraCompleto(OrderParamsDto parametros)
        {
            //Obtiene OC con Cabeceras y Posiciones
            List<OrdenCompraDto> ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros); //Cabecera y posiciones

            //Obtiene 1 Entrada de Servicio, prueba para obtener todas las necesarias.
            OrdenCompraEntradaServicioDto entradaServicio = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicio("1001457023");

            //Obtiene Oc con ItemsLineas
            List<AdjudicacionDto> OrdenesCompraConItemLinea = new List<AdjudicacionDto>(); //
            List< OrdenCompraItemDto > ItemsLinea = new List<OrdenCompraItemDto>(); // ItemsLinea con referencia a posiciones
            foreach (var orden in ordenesCompra)
            {
                string nroOC = orden.OrdenCompraId.ToString();
                //Solo pruebas de desarrollo, luego se debe eliminar.
                if (nroOC == "4123001500")
                {
                    var aux = "solo pruebas";
                }
                AdjudicacionDtoCopia detalleOrdendeCompra = new ObtenerOrdenDeCompraConsumerMOA(repositorio).ObtenerOrdenDeCompraConItems(nroOC);

                List<OrdenCompraPosicionDto> PosicionesAux = new List<OrdenCompraPosicionDto>();
                
                foreach (var posicionSap in detalleOrdendeCompra.AdjudicacionPosiciones)
                {
                    List<OrdenCompraItemDto> ItemsAux = new List<OrdenCompraItemDto>();

                    if (posicionSap != null && posicionSap.SubposicionesCompras != null && posicionSap.SubposicionesCompras.Any())
                    {
                        foreach (var item in posicionSap.SubposicionesCompras)
                        {
                            OrdenCompraItemDto PosicionItem = new OrdenCompraItemDto();

                            PosicionItem.ItemId = item.Indice;
                            PosicionItem.PosicionId = posicionSap.Indice;
                            PosicionItem.Descripcion = item.Tarea;
                            PosicionItem.ServicioNumero = item.CodigoSolp;
                            PosicionItem.Cantidad = item.Cantidad;
                            PosicionItem.PrecioBruto = item.PrecioBruto;
                            //PosicionItem.PCKG_NO = item.PCKG_NO;

                            ItemsAux.Add(PosicionItem);
                        }
                    }
                    OrdenCompraPosicionDto aux = new OrdenCompraPosicionDto();

                    aux.Items = ItemsAux;
                    aux.Descripcion = posicionSap.Tarea;
                    //Mapear campos de posiciones

                    PosicionesAux.Add(aux);
                    //orden.Posiciones = PosicionesAux;
                }

                orden.Posiciones = PosicionesAux;
            }
            return ordenesCompra;
        }


        //Estre se podría eliminar.
        public void ObtenerOrdenesCompraItemsLineas(List<OrdenCompraDto> ordenesCompras)
        {
        }
    }
}
