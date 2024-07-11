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
using System.Globalization;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services

{
    public class OrderService : IOrderService
    {
        protected readonly IRepositorio repositorio;
        private readonly IConsultaService consultaService;
        //private readonly ILiquidacionService _liquidacionService;
        //private OrderParamsDto parametros;

        private string dateTimeFormat = "dd/MM/yyyy";

        public OrderService(IConsultaService consultaService, IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.consultaService = consultaService;
            //_liquidacionService = liquidacionService;
        }

        public ListaPaginada<DetalleOrdenDeCompraDto> ObtenerOrdenesCompraConDetalle(OrderParamsDto parametros, string userMail)
        {
            try
            {
                List<DetalleOrdenDeCompraDto> result = ServicioSAP_OrdenesCompraCabeceras(parametros, userMail);

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

            int paginaValida =  pagina > 0 ? pagina : 1;
            int elementosPorPaginaValidos = (elementosPorPagina.HasValue && elementosPorPagina.Value > 0) ? elementosPorPagina.Value : 5;

            int indiceInicial = (paginaValida - 1) * elementosPorPaginaValidos;

            return new ListaPaginada<DetalleOrdenDeCompraDto>(resultados.ToList(), paginaValida, elementosPorPaginaValidos, itemsTotales);

        }

        // MMSN-768
        // Buscar nombre de proveedor para agregarlo a la ES.
        public Proveedor BuscarProveedor(OrderParamsDto parametros)
        {
            List<OrdenCompraDto> ordenesCompra = new List<OrdenCompraDto>();

            ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros);

            Proveedor _proveedor = new Proveedor();

            if (!string.IsNullOrEmpty(ordenesCompra[0].ProveedorNombre))
            {
                string razonSocial = ordenesCompra[0].ProveedorNombre;

                _proveedor = repositorio.Listar<Proveedor>(p =>
                    p.RazonSocial == razonSocial
                ).FirstOrDefault();

                if (_proveedor == null && !string.IsNullOrEmpty(ordenesCompra[0].ProveedorNumero))
                {
                    string codigoProveedor = ordenesCompra[0].ProveedorNumero;

                    _proveedor = repositorio.Listar<Proveedor>(p =>
                        p.CodigoProveedor == codigoProveedor
                    ).FirstOrDefault();
                }
            } 

            if (_proveedor == null)
            {
                _proveedor = new Proveedor();
                _proveedor.RazonSocial = ordenesCompra[0].ProveedorNombre;
            }

            return _proveedor;
        }

        // Consultas a servicio SAP con distintos criterios de busqueda
        public List<DetalleOrdenDeCompraDto> ServicioSAP_OrdenesCompraCabeceras(OrderParamsDto parametros, string userMail)
        {
            List< DetalleOrdenDeCompraDto> result = new List<DetalleOrdenDeCompraDto>();
            List<OrdenCompraDto> ordenesCompra = new List<OrdenCompraDto>();
            var obtenerOrdenConsumer = new ObtenerOrdenDeCompraConsumerMOA(repositorio);

            if (parametros.vendedor == "-")
                return result;

            bool usuarioSolp = false;

            if (userMail != null)
            {
                Usuario usuario = repositorio.Obtener<Usuario>(x => x.Mail == userMail);

                usuarioSolp = usuario.Roles.Any(rol => rol.Nombre == "SOLP");
            }

            ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros, usuarioSolp);

            //Se filtran por las OC tomando las que empiezan con 412
            ordenesCompra = ordenesCompra.Where(x => x.Id.ToString().StartsWith("412")).ToList();

            List<TablaSap> centros = repositorio.Listar<TablaSap>(a => a.Tabla == "Centro");
            List<TablaSap> almacenes = repositorio.Listar<TablaSap>(a => a.Tabla == "Almacen");
            //List<TablaSap> centros = new List<TablaSap>();
            //List<TablaSap> almacenes = new List<TablaSap>();

            //Recorro las ordenes de compra y obtengo el detalle de cada una

            string today = DateTime.Now.ToString(dateTimeFormat);
            DateTime fechaHasta = DateTime.ParseExact(today, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
            DateTime fechaInicio = DateTime.ParseExact(today, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);

            //MMSN-574 - Fecha Hasta
            if (!String.IsNullOrEmpty(parametros.fechaHasta) || !String.IsNullOrEmpty(parametros.fechaInicio))
            {
                //Desde FE viene como yyyy-MM-dd -> formatear a como devuelve el servicio(dd-MM-yyyy).
                parametros.fechaHasta = Convert.ToDateTime(parametros.fechaHasta).ToString(dateTimeFormat);
                fechaHasta = DateTime.ParseExact(parametros.fechaHasta, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);

                parametros.fechaInicio = Convert.ToDateTime(parametros.fechaInicio).ToString(dateTimeFormat);
                fechaInicio = DateTime.ParseExact(parametros.fechaInicio, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
            }


            if (!string.IsNullOrEmpty(parametros.vendedor))
            {
                DateTime dateInit = (fechaHasta.Year - fechaInicio.Year) * 12 + fechaHasta.Month - fechaInicio.Month > 24
                    && !string.IsNullOrEmpty(parametros.vendedor)
                    ? fechaHasta.AddYears(-2)
                    : fechaInicio;

                DateTime dateEnd = fechaHasta;
                

                ordenesCompra = ordenesCompra
                    .Where(oc => Convert.ToDateTime(oc.Fecha) >= dateInit && Convert.ToDateTime(oc.Fecha) <= dateEnd)
                    .ToList();
            }

            //MMSN-574
            List<OrdenCompraDto> ocFiltradas = new List<OrdenCompraDto>();
            foreach(var oc in ordenesCompra)
            {
                DateTime fechaOC = DateTime.ParseExact(oc.Fecha, dateTimeFormat, System.Globalization.CultureInfo.InvariantCulture);
                if(DateTime.Compare(fechaOC, fechaHasta) != 1)
                {
                    ocFiltradas.Add(oc);
                }
            }

            foreach (var ordenCompra in ocFiltradas)
            {               
                string nroOC = ordenCompra.Id.ToString();

                // Obtengo detalle de una OC //
                DetalleOrdenDeCompraDto detalleOrdendeCompra = obtenerOrdenConsumer.ObtenerDetalleDeOrdenDeCompra(nroOC, centros, almacenes, usuarioSolp);

                detalleOrdendeCompra.NombreProveedor = ordenCompra.ProveedorNombre;
                detalleOrdendeCompra.MonedaDescripcion = ordenCompra.MonedaDescripcion;
                detalleOrdendeCompra.SubjToR = ordenCompra.SUBJ_TO_R;


                var numeroSolpList = detalleOrdendeCompra.Posiciones
                .Select(p => p.NumeroSolp)
                .Distinct()
                .ToList();

                List<SolicitantesSolpedDto> solicitantes = GetSolicitantes(numeroSolpList).GetAwaiter().GetResult(); ;

                var solicitanteDiccionario = solicitantes.ToDictionary(s => s.NumeroSolp);

                foreach (var posicion in detalleOrdendeCompra.Posiciones)
                {
                    if (solicitanteDiccionario.TryGetValue(posicion.NumeroSolp, out var solicitante))
                    {
                        posicion.Solicitante = solicitante.Solicitante.Aprobador;
                    }
                }


                //MMSN-602
                List<Aprobaciones> aprobaciones = repositorio.Listar<Aprobaciones>(x => x.NRO_OC == nroOC && x.Estado_certificacion == "Pendiente Aprobación");
                if(aprobaciones != null && aprobaciones.Count > 0)
                {
                    foreach (Aprobaciones ap in aprobaciones)
                    {
                        int nroLinea = int.Parse(ap.Nro_linea);
                        long nroPosicion = long.Parse(ap.NRO_POS);

                        //Mapear aprobacion a ES
                        EntradaServicioDto es = MapAprobacionesToESDTO(ap);

                        //Buscar posición correspondiente a ES Temporal
                        var position = detalleOrdendeCompra.Posiciones.First(x => x.NumeroPosicion == nroPosicion);

                        

                        if (position != null)
                        {
                            
                            List<SolicitantesSolpedDto> solicitante = GetSolicitantes(new List<string> { position.NumeroSolp }).GetAwaiter().GetResult();


                            position.Solicitante = solicitante[0].Solicitante.Aprobador;
                            //Encontrar item correspondiente a ES Temporal

                            var item = position.Items.First(x => x.NumeroLinea == nroLinea);

                            if (item != null)
                            {
                                if(item.EntradasServicio == null)
                                {
                                    item.EntradasServicio = new List<EntradaServicioDto>();
                                    item.EntradasServicio.Add(es);
                                    //Recalcular Porcentaje y C. Real
                                    item.CantidadReal = item.CantidadReal + es.Cantidad;

                                    double res = Convert.ToDouble((item.CantidadReal * 100) / item.Cantidad);
                                    item.Porcentaje = res.ToString("0.##", CultureInfo.InvariantCulture);

                                    if (item.Porcentaje.EndsWith(".00"))
                                    {
                                        var redondeo = Math.Round(res);
                                        item.Porcentaje = res.ToString(CultureInfo.InvariantCulture);
                                    }
                                }
                                else
                                {
                                    item.EntradasServicio.Add(es);
                                    //Recalcular Porcentaje y C. Real
                                    item.CantidadReal = item.CantidadReal + es.Cantidad;

                                    double res = Convert.ToDouble((item.CantidadReal * 100) / item.Cantidad);
                                    item.Porcentaje = res.ToString("0.##", CultureInfo.InvariantCulture);

                                    if (item.Porcentaje.EndsWith(".00"))
                                    {
                                        var redondeo = Math.Round(res);
                                        item.Porcentaje = res.ToString(CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                        }

                    }
                }

                result.Add(detalleOrdendeCompra);

            }

            return result;
        }


        public async Task<List<SolicitantesSolpedDto>> GetSolicitantes(List<string> nroSolpedList)
        {
            List<SolicitantesSolpedDto> result = new List<SolicitantesSolpedDto>();
            string solicitante = string.Empty;
            string suplente = string.Empty;

            foreach (var nroSolped in nroSolpedList)
            {
                var solp = repositorio.Obtener<Solp>(x => x.NroSolp == nroSolped);

                if (solp != null)
                {
                    var pliego = repositorio.Obtener<Pliego>(x => x.Id == solp.Pliego_Id);

                    solicitante = !string.IsNullOrEmpty(pliego?.Email) ? pliego?.Email : pliego?.SupervisorTrabajo;

                    if (string.IsNullOrEmpty(solicitante))
                    {
                        var solpPosicion = repositorio.Obtener<SolpPosicion>(x => x.Solp_Id == solp.Id);

                        if (solpPosicion != null)
                           solicitante = repositorio.Obtener<Usuario>(x => x.UsuarioSap == solpPosicion.Solicitante)?.Mail;
                    }

                    suplente = !string.IsNullOrEmpty(solicitante) ? repositorio.Obtener<Usuario>(x => x.Mail == solicitante)?.Suplente : string.Empty ;

                    if (string.IsNullOrEmpty(solicitante))
                    {
                        solicitante = "Aprobador no encontrado";
                    }   
                }
                else
                {
                    solicitante = "No se encontro la Solp";
                }

                SolicitantesSolpedDto solicitantesSolpedDto = new SolicitantesSolpedDto();
                solicitantesSolpedDto.NumeroSolp = nroSolped;
                solicitantesSolpedDto.Solicitante = new SolicitanteDto() { Aprobador = solicitante, Suplente = suplente };

                result.Add(solicitantesSolpedDto);
            }
            
            return result;
        }

        /// <summary>
        /// MMSN-602: Aprobaciones a ESDto para FE
        /// </summary>
        /// <param name="ap"></param>
        /// <returns></returns>
        private EntradaServicioDto MapAprobacionesToESDTO(Aprobaciones ap)
        {
            EntradaServicioDto es = new EntradaServicioDto();

            es.TemporalId = ap.NRO_ES_LOCAL;
            es.Cantidad = decimal.Parse(ap.Cantidad_a_certificar);
            es.itemNumero = ap.Planned_package;
            es.ESS_LINE_NO = ap.Planned_line;
            es.ESS_PCKG_NO = ap.Planned_package;
            es.Fecha = ap.Fecha_Carga_ES.ToString();
            DateTime dtC = (DateTime)ap.Fecha_Contabilizacion;
            es.FechaContabilizacion = dtC.ToString("yyyy-MM-dd");
            DateTime dt = (DateTime)ap.Fecha_Documento;
            es.FechaDocumentoString = dt.ToString(dateTimeFormat);
            es.ImporteARPUSD = "$ " + ap.Monto_a_certificar.ToString();
            es.SePuedeBorrar = true;
            es.TextoBreve = ap.Texto_breve_servicio;
            es.Referencia = ap.Referencia;
            es.Ingresante = ap.Ingresante_CDS;
            es.IdES = ap.ID;

            return es;
        }
    }
}
