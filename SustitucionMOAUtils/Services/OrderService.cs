using SustitucionMOAAssets;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Util.EntitiesExtensions;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services

{
    public class OrderService : IOrderService
    {
        protected readonly IRepositorioEntradaServicio repositorioEntradaServicio;

        private const string dateTimeFormat = "dd/MM/yyyy";

        public OrderService(IRepositorioEntradaServicio repositorioEntradaServicio)
        {
            this.repositorioEntradaServicio = repositorioEntradaServicio;
        }

        public ListaPaginada<DetalleOrdenDeCompraDto> ObtenerOrdenesCompraConDetalle(OrderParamsDto parametros, string userMail)
        {
            try
            {
                ListaPaginada<DetalleOrdenDeCompraDto> result = ServicioSAP_OrdenesCompraCabeceras(parametros, userMail);

                return result;
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
        private List<DetalleOrdenDeCompraDto> OrdenarOrdenesCompra(List<DetalleOrdenDeCompraDto> ordenes, string columnaOrden, bool ordenAscendente)
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
        private ListaPaginada<DetalleOrdenDeCompraDto> PaginarResultados(List<DetalleOrdenDeCompraDto> resultados, OrderParamsDto parametros)
        {
            // Establecer valores predeterminados si son nulos o inválidos
            int itemsTotales = resultados.Count();

            int paginaValida = parametros.pagina > 0 ? parametros.pagina : 1;
            int elementosPorPaginaValidos = (parametros.elementosPorPagina > 0) ? parametros.elementosPorPagina : 10;

            int indiceInicial = (paginaValida - 1) * elementosPorPaginaValidos;
            var items = resultados.Skip(indiceInicial).Take(elementosPorPaginaValidos).ToList();
            return new ListaPaginada<DetalleOrdenDeCompraDto>(items, paginaValida, elementosPorPaginaValidos, itemsTotales);

        }

        // MMSN-768
        // Buscar nombre de proveedor para agregarlo a la ES.
        public virtual Proveedor BuscarProveedor(OrderParamsDto parametros)
        {
            List<OrdenCompraDto> ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros);

            Proveedor _proveedor = new Proveedor();

            if (ordenesCompra.Count > 0 && !string.IsNullOrEmpty(ordenesCompra[0].ProveedorNombre))
            {
                string razonSocial = ordenesCompra[0].ProveedorNombre.Trim();

                _proveedor = repositorioEntradaServicio.Listar<Proveedor>(p =>
                   p.RazonSocial.Trim() == razonSocial
                ).FirstOrDefault();

                if (_proveedor == null && !string.IsNullOrEmpty(ordenesCompra[0].ProveedorNumero))
                {
                    string codigoProveedor = ordenesCompra[0].ProveedorNumero.TrimEnd();

                    _proveedor = repositorioEntradaServicio.Listar<Proveedor>(p =>
                        p.CodigoProveedor.Trim() == codigoProveedor
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
        private ListaPaginada<DetalleOrdenDeCompraDto> ServicioSAP_OrdenesCompraCabeceras(OrderParamsDto parametros, string userMail)
        {
            ListaPaginada<DetalleOrdenDeCompraDto> result = new ListaPaginada<DetalleOrdenDeCompraDto>(new List<DetalleOrdenDeCompraDto>(), parametros.pagina, parametros.elementosPorPagina, 0);
            ObtenerOrdenDeCompraConsumerMOA obtenerOrdenConsumer = new ObtenerOrdenDeCompraConsumerMOA(repositorioEntradaServicio);

            if (parametros.vendedor == "-")
                return result;

            var esUsuarioSolp = userMail != null && repositorioEntradaServicio.Obtener<Usuario>(x => x.Mail == userMail).Roles.Any(rol => rol.Nombre == "SOLP");

            List<OrdenCompraDto> ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros, esUsuarioSolp);

            //Se filtran por las OC tomando las que empiezan con 412
            ordenesCompra = ordenesCompra.Where(x => x.Id.ToString().StartsWith("412")).ToList();
            string today = DateTime.Now.ToString(dateTimeFormat);
            DateTime fechaHasta = DateTime.ParseExact(today, dateTimeFormat, CultureInfo.InvariantCulture);
            DateTime fechaInicio = DateTime.ParseExact(today, dateTimeFormat, CultureInfo.InvariantCulture);

            if (!String.IsNullOrEmpty(parametros.fechaHasta) || !String.IsNullOrEmpty(parametros.fechaInicio))
            {
                //Desde FE viene como yyyy-MM-dd -> formatear a como devuelve el servicio(dd-MM-yyyy).
                parametros.fechaHasta = Convert.ToDateTime(parametros.fechaHasta).ToString(dateTimeFormat);
                fechaHasta = DateTime.ParseExact(parametros.fechaHasta, dateTimeFormat, CultureInfo.InvariantCulture);

                parametros.fechaInicio = Convert.ToDateTime(parametros.fechaInicio).ToString(dateTimeFormat);
                fechaInicio = DateTime.ParseExact(parametros.fechaInicio, dateTimeFormat, CultureInfo.InvariantCulture);
            }

            List<OrdenCompraDto> ocFiltradas = new List<OrdenCompraDto>();
            foreach (var oc in ordenesCompra)
            {
                DateTime fechaOC = DateTime.ParseExact(oc.Fecha, dateTimeFormat, CultureInfo.InvariantCulture);
                if (fechaOC <= fechaHasta)
                {
                    ocFiltradas.Add(oc);
                }
            }

            if (!string.IsNullOrEmpty(parametros.vendedor))
            {
                DateTime dateInit = (fechaHasta.Year - fechaInicio.Year) * 12 + fechaHasta.Month - fechaInicio.Month > 24
                    && !string.IsNullOrEmpty(parametros.vendedor)
                    ? fechaHasta.AddYears(-2)
                    : fechaInicio;

                DateTime dateEnd = fechaHasta;


                ocFiltradas = ocFiltradas
                    .Where(oc => Convert.ToDateTime(oc.Fecha) >= dateInit && Convert.ToDateTime(oc.Fecha) <= dateEnd)
                    .ToList();
            }

            List<DetalleOrdenDeCompraDto> ordenesCompraDto = ocFiltradas.Select(x => new DetalleOrdenDeCompraDto
            {
                NumeroOrdenDeCompra = x.Id.ToString(),
                FechaCreacion = x.Fecha,
                NombreProveedor = x.ProveedorNombre,
                MonedaDescripcion = x.MonedaDescripcion,
                SubjToR = x.SUBJ_TO_R,
                Proveedor = x.ProveedorNumero,
            }).ToList();


            if (!string.IsNullOrEmpty(parametros.ColumnaOrden))
                ordenesCompraDto = OrdenarOrdenesCompra(ordenesCompraDto, parametros.ColumnaOrden, parametros.OrdenAscendente);


            ListaPaginada<DetalleOrdenDeCompraDto> ordenesCompraDtoPaginada = PaginarResultados(ordenesCompraDto, parametros);


            List<TablaSap> centros = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Centro");
            List<TablaSap> almacenes = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Almacen");
            result = new ListaPaginada<DetalleOrdenDeCompraDto>(new List<DetalleOrdenDeCompraDto>(), ordenesCompraDtoPaginada.Pagina, ordenesCompraDtoPaginada.ItemsPorPagina, ordenesCompraDtoPaginada.ItemsTotales);
            //Recorro las ordenes de compra y obtengo el detalle de cada una
            foreach (DetalleOrdenDeCompraDto ordenCompra in ordenesCompraDtoPaginada.Items)
            {
                var detalleOrdenDeCompraDto = obtenerOrdenConsumer.ObtenerDetalleDeOrdenDeCompra(ordenCompra.NumeroOrdenDeCompra, centros, almacenes, esUsuarioSolp);

                detalleOrdenDeCompraDto.NombreProveedor = ordenCompra.NombreProveedor;
                detalleOrdenDeCompraDto.MonedaDescripcion = ordenCompra.MonedaDescripcion;
                detalleOrdenDeCompraDto.SubjToR = ordenCompra.SubjToR;


                var numeroSolpList = detalleOrdenDeCompraDto.Posiciones
                .Select(p => p.NumeroSolp)
                .Distinct()
                .ToList();

                List<SolicitantesSolpedDto> solicitantes = GetSolicitantes(numeroSolpList).GetAwaiter().GetResult(); ;

                var solicitanteDiccionario = solicitantes.ToDictionary(s => s.NumeroSolp);

                foreach (var posicion in detalleOrdenDeCompraDto.Posiciones)
                {
                    if (solicitanteDiccionario.TryGetValue(posicion.NumeroSolp, out var solicitante))
                    {
                        posicion.Solicitante = solicitante.Solicitante.Aprobador;
                    }
                }

                Adjudicacion adjudicacion = repositorioEntradaServicio.ObtenerUltimaAdjudicacionOC(ordenCompra.NumeroOrdenDeCompra);
                detalleOrdenDeCompraDto.AdmiteCertificacionesParciales = adjudicacion?.AdmiteCertificacionesParciales ?? true;

                List<Aprobaciones> aprobaciones = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_OC == ordenCompra.NumeroOrdenDeCompra && (x.Estado_certificacion == "Pendiente Aprobación" || x.Estado_certificacion == "Aprobada"));
                foreach (Aprobaciones aprobacion in aprobaciones)
                {
                    int nroLinea = int.Parse(aprobacion.Nro_linea);
                    long nroPosicion = long.Parse(aprobacion.NRO_POS);

                    var entradaServicioDto = MapAprobacionesToESDTO(aprobacion, ordenCompra.MonedaDescripcion);

                    //Buscar posición correspondiente a ES Temporal
                    var position = detalleOrdenDeCompraDto.Posiciones.First(x => x.NumeroPosicion == nroPosicion);

                    if (aprobacion.EstaAprobada())
                    {
                        var itemPosicion = position.Items.First(x => x.NumeroLinea == nroLinea);

                        var entradaServicioItem = itemPosicion.EntradasServicio.FirstOrDefault(x => x.Id == aprobacion.NRO_ES_SAP);

                        if (entradaServicioItem != null)
                        {
                            entradaServicioItem.TextoBreve =
                                string.IsNullOrEmpty(aprobacion.Texto_breve_servicio) || aprobacion.Texto_breve_servicio == "Este campo es ignorado por el servicio SAP, pero debe enviarsele algo"
                                // Por algún motivo se decidió enviar a sap ese texto cuando falta la descripción (en realidad, antes se enviaba siempre...).
                                // Por lo que ahora estamos atrapados consultando por ese texto para evitar mostrarlo... ¬¬
                                ? ""
                                : aprobacion.Texto_breve_servicio.Trim();
                        }
                    }

                    if (aprobacion.EstaPendienteAprobacion() && position != null)
                    {
                        List<SolicitantesSolpedDto> solicitante = GetSolicitantes(new List<string> { position.NumeroSolp }).GetAwaiter().GetResult();

                        position.Solicitante = solicitante[0].Solicitante.Aprobador;
                        //Encontrar item correspondiente a ES Temporal

                        var item = position.Items.First(x => x.NumeroLinea == nroLinea);

                        if (item != null)
                        {
                            item.EntradasServicio = item.EntradasServicio ?? new List<EntradaServicioDto>();
                            item.EntradasServicio.Add(entradaServicioDto);

                            //Recalcular Porcentaje y C. Real
                            item.CantidadReal += entradaServicioDto.Cantidad;

                            double porcentaje = Convert.ToDouble((item.CantidadReal * 100) / item.Cantidad);
                            item.Porcentaje = porcentaje.ToString("0.##", CultureInfo.InvariantCulture);

                            if (item.Porcentaje.EndsWith(".00"))
                            {
                                var redondeo = Math.Round(porcentaje);
                                item.Porcentaje = porcentaje.ToString(CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }

                result.Items.Add(detalleOrdenDeCompraDto);

            }
            if (result.Items.Count > 0)
            {
                result.Items.FirstOrDefault().ItemsTotales = ordenesCompraDtoPaginada.ItemsTotales;
                result.Items.FirstOrDefault().Pagina = ordenesCompraDtoPaginada.Pagina;
                result.Items.FirstOrDefault().ItemPorPagina = ordenesCompraDtoPaginada.ItemsPorPagina;

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
                var solp = repositorioEntradaServicio.Obtener<Solp>(x => x.NroSolp == nroSolped);

                if (solp != null)
                {
                    var pliego = repositorioEntradaServicio.Obtener<Pliego>(x => x.Id == solp.Pliego_Id);

                    solicitante = !string.IsNullOrEmpty(pliego?.Email) ? pliego?.Email : pliego?.SupervisorTrabajo;

                    if (string.IsNullOrEmpty(solicitante))
                    {
                        var solpPosicion = repositorioEntradaServicio.Obtener<SolpPosicion>(x => x.Solp_Id == solp.Id);

                        if (solpPosicion != null)
                            solicitante = repositorioEntradaServicio.Obtener<Usuario>(x => x.UsuarioSap == solpPosicion.Solicitante)?.Mail;
                    }

                    suplente = !string.IsNullOrEmpty(solicitante) ? repositorioEntradaServicio.Obtener<Usuario>(x => x.Mail == solicitante)?.Suplente : string.Empty;

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
        private EntradaServicioDto MapAprobacionesToESDTO(Aprobaciones ap, string moneda)
        {
            EntradaServicioDto es = new EntradaServicioDto();

            es.TemporalId = ap.NRO_ES_LOCAL;
            es.Cantidad = decimal.Parse(ap.Cantidad_a_certificar, CultureInfo.InvariantCulture);
            es.itemNumero = ap.Planned_package;
            es.ESS_LINE_NO = ap.Planned_line;
            es.ESS_PCKG_NO = ap.Planned_package;
            es.Fecha = ap.Fecha_Carga_ES.ToString();
            DateTime dtC = (DateTime)ap.Fecha_Contabilizacion;
            es.FechaContabilizacion = dtC.ToString("yyyy-MM-dd");
            DateTime dt = (DateTime)ap.Fecha_Documento;
            es.FechaDocumentoString = dt.ToString(dateTimeFormat);
            es.ImporteARPUSD = moneda + " " + ap.Monto_a_certificar.ToString();
            es.SePuedeBorrar = true;
            es.TextoBreve = string.IsNullOrEmpty(ap.Texto_breve_servicio) || ap.Texto_breve_servicio == "Este campo es ignorado por el servicio SAP, pero debe enviarsele algo"
                // Por algún motivo se decidió enviar a sap ese texto cuando falta la descripción (en realidad, antes se enviaba siempre...).
                // Por lo que ahora estamos atrapados consultando por ese texto para evitar mostrarlo... ¬¬
                ? ""
                : ap.Texto_breve_servicio.Trim();
            es.Referencia = ap.Referencia;
            es.Ingresante = ap.Ingresante_CDS;
            es.IdES = ap.ID;

            return es;
        }
    }
}
