// Ignore Spelling: reasignaciones

using Quartz.Util;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Services.Email.Dto;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SustitucionMOAWS.WSConsumers.ModificarOrdenDeCompraConsumerMOA;

namespace SustitucionMOAUtils.Services

{
    public class EntradaServicioService : IEntradaServicioService
    {
        protected readonly IRepositorioEntradaServicio repositorioEntradaServicio;
        private readonly OrderService orderService;
        private readonly IComprasService comprasService;
        private readonly IEmailCertificationService emailCertificationService;
        private readonly IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA;
        private readonly IReporteESService _reporteESService;
        private readonly string EmailEnvioErrores = ConfigurationManager.AppSettings["EmailEnvioErrores"];

        public EntradaServicioService(
            IRepositorioEntradaServicio repositorioEntradaServicio,
            OrderService orderService,
            IComprasService comprasService,
            IEmailCertificationService emailCertificationService,
            IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA,
            IReporteESService reporteESService)
        {
            this.obtenerOrdenDeCompraConsumerMOA = obtenerOrdenDeCompraConsumerMOA;
            this.repositorioEntradaServicio = repositorioEntradaServicio;
            this.orderService = orderService;
            this.comprasService = comprasService;
            this.emailCertificationService = emailCertificationService;
            this._reporteESService = reporteESService;
        }

        public async Task<List<EntradaServicioCabeceraDto>> ObtenerEntradasServicioCompleta(EntradaServicioParamsDto parametros, UsuarioDto usuario)
        {
            List<EntradaServicioCabeceraDto> Documentos = await ServicioSAP_EntradasServicioCabecera(parametros, usuario);
            Documentos = OrdenarEntradasServicio(Documentos);

            return Documentos;
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
        public List<EntradaServicioCabeceraDto> ServicioAprobaciones_EntradasServicioCabecera(EntradaServicioParamsDto parametros, UsuarioDto usuario)
        {
            var entradasServicio = new List<EntradaServicioCabeceraDto>();
            var ordenParams = new OrderParamsDto();
            var correoUsuario = usuario.Mail.ToLower();
            List<Aprobaciones> aprobacionesTemporales;

            var debeVerTodo = parametros.VerTodo && usuario.Permisos.Contains("VER TODOS LOS ESTADOS DE ES");
            var certExt = usuario.Permisos.Contains("VER SOLAPA CERTIFICACION DE SERVICIOS EXTERNA") && !usuario.Permisos.Contains("VER SOLAPA CERTIFICACION DE SERVICIOS");

            var fechaDesde = DateTime.Now;
            var fechaHasta = DateTime.Now;

            var debeFiltrarPorFecha =
                string.IsNullOrEmpty(parametros.OrdenCompra) &&
                !string.IsNullOrEmpty(parametros.FechaInicio) &&
                !string.IsNullOrEmpty(parametros.FechaFin) &&
                DateTime.TryParseExact(parametros.FechaInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaDesde) &&
                DateTime.TryParseExact(parametros.FechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaHasta);

            if (debeVerTodo)
            {
                aprobacionesTemporales = repositorioEntradaServicio.Listar<Aprobaciones>(x =>
                    x.NRO_ES_SAP == null &&
                    (string.IsNullOrEmpty(parametros.OrdenCompra) || x.NRO_OC == parametros.OrdenCompra) &&
                    (!debeFiltrarPorFecha || !x.Fecha_Carga_ES.HasValue || (x.Fecha_Carga_ES >= fechaDesde && x.Fecha_Carga_ES <= fechaHasta)));
            }
            else
            {
                aprobacionesTemporales = repositorioEntradaServicio.Listar<Aprobaciones>(x =>
                    x.NRO_ES_SAP == null &&
                    (x.Ingresante_CDS.ToLower() == correoUsuario ||
                        x.Fiscal_SOLPED.ToLower() == correoUsuario ||
                        x.Aprobador_CDS.ToLower() == correoUsuario ||
                        (certExt && x.Proveedor == parametros.Vendedor)) &&
                    (string.IsNullOrEmpty(parametros.OrdenCompra) || x.NRO_OC == parametros.OrdenCompra) &&
                    (!debeFiltrarPorFecha || !x.Fecha_Carga_ES.HasValue || (x.Fecha_Carga_ES >= fechaDesde && x.Fecha_Carga_ES <= fechaHasta)));
            }

            try
            {
                Dictionary<string, EntradaServicioCabeceraDto> diccionarioES = aprobacionesTemporales
                .GroupBy(temporal => temporal.NRO_ES_LOCAL)
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo =>
                    {
                        ordenParams.OrdenCompraId = grupo.First().NRO_OC;
                        var entradaServicioTemp = MapEntradaServicioCabecera(grupo.First(), ordenParams);
                        entradaServicioTemp.entradaServicioDetalle = grupo
                            .Select(MapEntradaServicioDetalle)
                            .ToList();
                        return entradaServicioTemp;
                    });

                foreach (var kvp in diccionarioES)
                {
                    entradasServicio.Add(kvp.Value);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            entradasServicio = OrdenarEntradasServicio(entradasServicio);

            return entradasServicio;
        }

        /// <summary>
        /// Metodo para obtener el correo del responsable de la solp
        /// </summary>
        /// <param name="nroSolp"></param>
        /// <returns></returns>
        public string ObtenerCorreoSolp(string nroSolp)
        {
            string email = "";

            try
            {
                Solp solp = repositorioEntradaServicio.Obtener<Solp>(s => s.NroSolp == nroSolp);

                if (solp != null)
                {
                    if (!string.IsNullOrEmpty(solp.Pliego.Email) && solp.Pliego.Email.Contains("@"))
                    {
                        email = solp.Pliego.Email;
                    }
                    else if (!string.IsNullOrEmpty(solp.Pliego.SupervisorTrabajo) && solp.Pliego.SupervisorTrabajo.Contains("@"))
                    {
                        email = solp.Pliego.SupervisorTrabajo;
                    }
                    else if (solp.Posiciones.Count > 0)
                    {
                        foreach (var pos in solp.Posiciones)
                        {
                            if (!pos.Solicitante.IsNullOrWhiteSpace())
                            {
                                string solicitante = pos.Solicitante.Replace(" ", "");
                                var usuario = repositorioEntradaServicio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.UsuarioSap.ToUpper() == solicitante.ToUpper());
                                if (usuario != null)
                                {
                                    if (!string.IsNullOrEmpty(usuario.Mail))
                                    {
                                        email = usuario.Mail;
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                throw (e);
            }

            return email;
        }

        /// <summary>
        /// Mapeo para cargar la cabecera de la ES
        /// </summary>
        /// <param name="temporal"></param>
        /// <param name="ordenParams"></param>
        /// <returns>entradaServicioTemp</returns>
        public EntradaServicioCabeceraDto MapEntradaServicioCabecera(Aprobaciones temporal, OrderParamsDto ordenParams)
        {
            DateTime fechaCreacionFormateada = (DateTime)temporal.Fecha_Carga_ES;
            DateTime fechaContabilizacionFormateada = (DateTime)temporal.Fecha_Contabilizacion;
            DateTime fechaDocumentoFormateada = (DateTime)temporal.Fecha_Documento;

            EntradaServicioCabeceraDto entradaServicioTemp = new EntradaServicioCabeceraDto
            {
                ID = temporal.ID,
                OrdenCompra = temporal.NRO_OC,
                Descripcion = string.IsNullOrEmpty(temporal.Texto_breve_servicio) ? "" : temporal.Texto_breve_servicio.Trim(),
                MontoTotal = temporal.Monto_total.ToString(),
                FechaCreacion = fechaCreacionFormateada.ToString("dd/MM/yyyy"),
                FechaCreacionDateTime = temporal.Fecha_Carga_ES,
                EntradaServicio = temporal.Estado_certificacion == "Aprobada" ? temporal.NRO_ES_SAP.ToString() : temporal.NRO_ES_LOCAL,
                Estado = temporal.Estado_certificacion,
                MotivoRechazo = temporal.Motivo_rechazo,
                NumeroCertificacion = temporal.NRO_ES_LOCAL,
                Ingresante = temporal.Ingresante_CDS,
                Aprobador = temporal.Aprobador_CDS,
                Suplente = temporal.Suplente,
                Fiscal = temporal.Fiscal_SOLPED,
                FechaContabilizacion = fechaContabilizacionFormateada.ToString("dd/MM/yyyy"),
                FechaDocumento = fechaDocumentoFormateada.ToString("dd/MM/yyyy"),
                NroPosicion = temporal.NRO_POS,
                AnuladaPor = temporal.Anulado_por
            };

            if (temporal.Estado_certificacion == "Aprobada")
            {
                DateTime fechaAprobacionFormateada = (DateTime)temporal.Fecha_aprobacion;
                entradaServicioTemp.FechaAprobacion = fechaAprobacionFormateada.ToString("dd/MM/yyyy");
            }

            if (temporal.Estado_certificacion == "Rechazado")
            {
                DateTime fechaRechazoFormateada = (DateTime)temporal.Fecha_rechazo;
                entradaServicioTemp.FechaRechazo = fechaRechazoFormateada.ToString("dd/MM/yyyy");
            }

            Proveedor prov = orderService.BuscarProveedor(ordenParams);
            entradaServicioTemp.Proveedor = prov.RazonSocial ?? "-";
            entradaServicioTemp.CUIT = prov.CUIT ?? "-";

            entradaServicioTemp.Moneda = BuscarMoneda(ordenParams);

            return entradaServicioTemp;
        }

        /// <summary>
        /// Mapeo para cargar el detalle de la ES
        /// </summary>
        /// <param name="temporal"></param>
        /// <returns>detalleEntradaServicioTemp</returns>
        public EntradaServicioDetalleDto MapEntradaServicioDetalle(Aprobaciones temporal)
        {
            EntradaServicioDetalleDto detalleEntradaServicioTemp = new EntradaServicioDetalleDto
            {
                ID = temporal.ID,
                Cantidad = temporal.Cantidad.ToString(),
                NumeroLinea = int.Parse(temporal.Nro_linea).ToString(),
                NroPosicion = int.Parse(temporal.NRO_POS).ToString(),
                UM = temporal.UM,
                Descripcion = string.IsNullOrEmpty(temporal.Descripcion_ES) ? "" : temporal.Descripcion_ES.Trim(),
                TextoBreveServicio = temporal.Texto_breve_servicio.Trim(),
                Monto = Convert.ToDecimal(temporal.Monto),
                CantidadCertificar = temporal.Cantidad_a_certificar,
                PorcentajeCertificar = temporal.Porcentaje_a_certificar,
                MontoCertificar = temporal.Monto_a_certificar,
                NroRemito = temporal.Referencia,
                CodigoServicio = temporal.Nro_servicio,
                FechaPrestacion = temporal.Fecha_Documento?.ToString("dd/MM/yyyy"),
                CantidadAnterior = Convert.ToDouble(temporal.Cantidad_Anterior),
                PrecioUnitario = Convert.ToDecimal(temporal.Monto),
            };

            return detalleEntradaServicioTemp;
        }

        public string BuscarMoneda(OrderParamsDto parametros)
        {

            List<OrdenCompraDto> ordenesCompra = new ObtenerOrdenesDeCompraConsumerMOA().Request(parametros);

            if (ordenesCompra.Count > 0 && !string.IsNullOrEmpty(ordenesCompra[0].MonedaDescripcion))
            {
                return ordenesCompra[0].MonedaDescripcion;
            }

            return "";
        }

        /// <summary>
        /// Borrar Entrada de Servicio indicando su número de documento
        /// Actualmente hay varias incognicas con respecto a las condiciones que debe cumplir una ES para poder ser borrada
        /// </summary>
        /// <returns></returns>
        public string BorrarEntradaServicio(EntradaServicioParamsDto parametros, UsuarioDto usuario)
        {
            string result = "";
            if (parametros.DocumentoNumero.Contains("\""))
            {
                parametros.DocumentoNumero = parametros.DocumentoNumero.Replace("\\", "").Replace("\"", "");
            }
            if (parametros.DocumentoNumero.Contains("T_"))
            {
                //Temporal - hard delete
                List<Aprobaciones> apToDelete = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == parametros.DocumentoNumero);
                foreach (Aprobaciones ap in apToDelete)
                {
                    ap.Estado_certificacion = "Anulada";
                    ap.Anulado_por = usuario.Mail;
                }
                repositorioEntradaServicio.GuardarCambios();
                result = "Se ha eliminado la entrada de servicio " + parametros.DocumentoNumero;
            }
            else
            {
                string fechaContabilizacion = parametros.FechaContabilizacion;
                DateTime FechaContabilizacionToDateTime = Convert.ToDateTime(fechaContabilizacion).ToUniversalTime();
                int currentMonth = DateTime.UtcNow.Month;
                int currentYear = DateTime.UtcNow.Year;
                int lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
                int lastYear = currentMonth == 1 ? currentYear - 1 : currentYear;

                if (FechaContabilizacionToDateTime.Year == lastYear && FechaContabilizacionToDateTime.Month == lastMonth)
                    fechaContabilizacion = DateTime.UtcNow.ToString("yyyy-MM-dd");

                result = new BorrarEntradaServicioConsumerMOA().BorrarEntradaServicio(parametros.DocumentoNumero, fechaContabilizacion);
            }


            return result;
        }

        public List<EntradaServicioCreateRespuestaDto> CrearEntradaServicio(CreateEntradaServicioDto crearESRequestDto, string mailUsuario)
        {
            var obtenerOrdenConsumer = new ObtenerOrdenDeCompraConsumerMOA(repositorioEntradaServicio);
            var centrosSap = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Centro");
            var almacenesSap = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Almacen");
            var solicitudesMailAprobacionES = new List<MailAprobacionESRequest>();
            var respuestasCreacion = new List<EntradaServicioCreateRespuestaDto>();

            foreach (var posicionES in crearESRequestDto.Posiciones)
            {
                var solpNro = posicionES.EntrySheetHeader.SolPedNumber;
                var proveedor = posicionES.EntrySheetHeader.Proveedor;

                var validacionIngresanteResp = ValidarIngresante(mailUsuario, solpNro);
                EntradaServicioCreateRespuestaDto resultadoCreacionES;
                if (validacionIngresanteResp.Message == "Auto")
                {
                    resultadoCreacionES = CrearEntradaServicio(posicionES, mailUsuario, crearESRequestDto.report, crearESRequestDto.IdAdjuntos, solpNro, proveedor);
                }
                else
                {
                    if (validacionIngresanteResp.Message == "Temporal")
                    {
                        resultadoCreacionES = CrearEntradaServicioTemporal(posicionES, mailUsuario, crearESRequestDto.report, crearESRequestDto.IdAdjuntos, solicitudesMailAprobacionES,
                            obtenerOrdenConsumer, centrosSap, almacenesSap, solpNro, proveedor);
                    }
                    else
                    {
                        resultadoCreacionES = validacionIngresanteResp;
                    }
                }

                respuestasCreacion.Add(resultadoCreacionES);
            }

            solicitudesMailAprobacionES.ForEach(s => emailCertificationService.EnviarMailAprobacion(s));

            return respuestasCreacion;
        }

        /// <summary>
        /// MMSN-1151: A llamar desde el servicio de LogicaDerivacion, para notificar las reasignaciones a un usuario
        /// </summary>
        /// <param name="ListaAp"></param>
        public async Task NotificarReasignaciones(IEnumerable<string> ListaAp)
        {
            //Todos los registros con mismo NRO_ES_LOCAL
            foreach (string esLocal in ListaAp)
            {
                //Todos los registros con mismo NRO_ES_LOCAL
                List<Aprobaciones> completeAp = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == esLocal);
                //Buscar Proveedor
                OrderParamsDto orderParams = new OrderParamsDto
                {
                    OrdenCompraId = completeAp[0].NRO_OC
                };
                Proveedor prov = orderService.BuscarProveedor(orderParams);

                //MMSN-1030: Fix
                string aprobador = completeAp[0].Aprobador_CDS;
                var user = repositorioEntradaServicio.Listar<Usuario>(x => x.Mail == aprobador).FirstOrDefault();
                int userId = 0;
                if (user != null)
                {
                    userId = user.Id;
                }

                await NotifyCreation(completeAp, prov, userId, aprobador);
            }
        }

        /// <summary>
        /// Aprobación de las ES en estado Pendiente Aprobación
        /// Se carga la EntrySheetHeader y el EntrySheetServices para su correcta aprobación
        /// </summary>
        /// <param name="nro_es_local"></param>
        /// <returns></returns>
        public async Task<EntradaServicioCreateRespuestaDto> AprobarEntradaDeServicio(string nro_es_local, string Moneda)
        {
            Logger.Log.Info($"AprobarEntradaDeServicio: '{nro_es_local}', moneda '{Moneda}'");
            List<Aprobaciones> EntradasDeServicioTemp = repositorioEntradaServicio.Listar<SustitucionMOAModel.Entities.Aprobaciones>(x => x.NRO_ES_LOCAL == nro_es_local);
            EntradaServicioCreateRespuestaDto result = new EntradaServicioCreateRespuestaDto();
            string status = CheckESStatus(EntradasDeServicioTemp);
            if (status.Contains("Modificado"))
            {
                result.Type = "Desync";
                string estado = status.Split('-')[1];
                result.Message = $"La entrada de servicio {nro_es_local} no se encuentra en estado Pendiente de Aprobación. Su estado actual es: {estado}";
                return result;
            }
            EntradaServicioCreateParamsDto EntradaServicioSapParams = new EntradaServicioCreateParamsDto();
            EmailDetailCertificateDto emailDetailCertificateDto = new EmailDetailCertificateDto();
            List<ServiceDetailDto> serviceDetailDtoList = new List<ServiceDetailDto>();

            if (EntradasDeServicioTemp.Count > 0)
            {
                //MMSN - 693: Desfasaje de fecha de contabilización entre ingreso de certificación y aprobación
                DateTime? originalFC = EntradasDeServicioTemp[0].Fecha_Contabilizacion;
                DateTime today = DateTime.Today;
                bool modDate = false;
                DateTime newDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                if (originalFC.HasValue &&
                   originalFC.Value.Year == today.AddMonths(-1).Year &&
                   originalFC.Value.Month == today.AddMonths(-1).Month)
                {
                    modDate = true;
                }

                try
                {
                    EntradaServicioSapParams.EntrySheetHeader = new EntrySheetHeaderSection
                    {
                        Descripcion = EntradasDeServicioTemp[0].Descripcion_ES,
                        FechaDocumento = EntradasDeServicioTemp[0].Fecha_Documento?.ToString("yyyy-MM-dd"),
                        FechaContabilizacion = modDate == false ? EntradasDeServicioTemp[0].Fecha_Contabilizacion?.ToString("yyyy-MM-dd") : newDate.ToString("yyyy-MM-dd"),
                        OrdenCompraNumero = EntradasDeServicioTemp[0].NRO_OC,
                        OrdenCompraPosicionNumero = EntradasDeServicioTemp[0].NRO_POS,
                        DocumentoReferenciaNumero = EntradasDeServicioTemp[0].Referencia
                    };

                    string codigoProveedor = EntradasDeServicioTemp[0].Proveedor;

                    var proveedor = repositorioEntradaServicio.Obtener<Proveedor>(x => x.CodigoProveedor == codigoProveedor);

                    emailDetailCertificateDto.Descripcion = EntradasDeServicioTemp[0].Texto_breve_servicio;
                    emailDetailCertificateDto.FechaCertificacion = EntradasDeServicioTemp[0].Fecha_Contabilizacion?.ToString("yyyy-MM-dd");
                    emailDetailCertificateDto.Destinatario = EntradasDeServicioTemp[0].Ingresante_CDS;
                    emailDetailCertificateDto.Proveedor = proveedor.RazonSocial;
                    emailDetailCertificateDto.MontoTotal = Moneda == "ARP" ? "$ " + EntradasDeServicioTemp[0].Monto_total.ToString() : Moneda + " " + EntradasDeServicioTemp[0].Monto_total.ToString();
                    emailDetailCertificateDto.NroOC = EntradasDeServicioTemp[0].NRO_OC;
                    emailDetailCertificateDto.NumeroPosicion = EntradasDeServicioTemp[0].NRO_POS;
                    emailDetailCertificateDto.Aprobador = EntradasDeServicioTemp[0].Aprobador_CDS;


                    EntradaServicioSapParams.EntrySheetServices = new EntrySheetServiceSection
                    {
                        Items = new List<EntrySheetServiceItemSection>()
                    };

                    foreach (var ES in EntradasDeServicioTemp)
                    {
                        ServiceDetailDto serviceDetailDto = new ServiceDetailDto();

                        if (ES.Estado_certificacion == "Pendiente Aprobación")
                        {
                            EntrySheetServiceItemSection item = new EntrySheetServiceItemSection
                            {
                                ExternalLineNumber = ES.Nro_linea,
                                Service = ES.Nro_servicio,
                                Quantity = ES.Cantidad_a_certificar,
                                ShortText = ES.Texto_breve_servicio,
                                PlannedPackage = ES.Planned_package,
                                PlannedLine = ES.Planned_line
                            };
                            EntradaServicioSapParams.EntrySheetServices.Items.Add(item);
                        }

                        serviceDetailDto.UM = ES.UM;
                        serviceDetailDto.Descripcion = ES.Descripcion_ES;
                        serviceDetailDto.Porcentaje = ES.Porcentaje_a_certificar;
                        serviceDetailDto.Cantidad = ES.Cantidad.ToString();
                        serviceDetailDto.Monto = Moneda == "ARP" ? "$ " + ES.Monto_a_certificar.ToString() : Moneda + " " + ES.Monto_a_certificar.ToString();


                        serviceDetailDtoList.Add(serviceDetailDto);
                    }

                    emailDetailCertificateDto.DetalleServicio = serviceDetailDtoList;


                    if (EntradaServicioSapParams.EntrySheetServices.Items.Count > 0)
                    {
                        result = await new CrearEntradaDeServicioConsumerMOA().CrearEntradaServicioAsync(EntradaServicioSapParams);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error($"Error al aprobar la certificacion temporal {nro_es_local}", e);
                    throw;
                }

                if (result.Type == "I" && result.Id == "SE")
                {
                    int ESNumber = GetESNumber(result.Message);
                    emailDetailCertificateDto.NumeroCertificacion = ESNumber.ToString();
                    try
                    {
                        foreach (var ES in EntradasDeServicioTemp)
                        {
                            if (ES.Estado_certificacion == "Pendiente Aprobación")
                            {

                                ES.NRO_ES_SAP = ESNumber;
                                ES.Estado_certificacion = "Aprobada";
                                ES.Fecha_aprobacion = DateTime.Today;
                                result.NroESSap = ESNumber.ToString();
                                repositorioEntradaServicio.GuardarCambios();

                            }
                        }

                        _ = NotifyApproval(emailDetailCertificateDto, nro_es_local);
                    }
                    catch (Exception e)
                    {
                        Logger.Log.Error(e);
                    }
                }
            }
            Logger.Log.Info($"AprobarEntradaDeServicio: '{nro_es_local}' aprobada.");

            return result;
        }

        /// <summary>
        /// Rechazo de ES temporal, Se notifica al usuario por correo
        /// </summary>
        /// <param name="rechazo"></param>
        /// <returns></returns>
        public EntradaServicioRejectRespuestaDto RechazarEntradaDeServicio(EmailDetailCertificateDto rechazo)
        {
            List<Aprobaciones> EntradasDeServicioTemp = repositorioEntradaServicio.Listar<SustitucionMOAModel.Entities.Aprobaciones>(x => x.NRO_ES_LOCAL == rechazo.NumeroCertificacion);
            EntradaServicioRejectRespuestaDto ret = new EntradaServicioRejectRespuestaDto();
            ret.status = "";
            string check = CheckESStatus(EntradasDeServicioTemp);
            if (check.Contains("Modificado"))
            {
                string estado = check.Split('-')[1];
                ret.status = $"La entrada de servicio {EntradasDeServicioTemp[0].NRO_ES_LOCAL} no se encuentra en estado Pendiente de Aprobación. Su estado actual es: {estado}";
                ret.result = EntradasDeServicioTemp;
                return ret;
            }
            else
            {
                ret.status = "OK";
            }

            foreach (var ES in EntradasDeServicioTemp)
            {
                if (ES.Estado_certificacion == "Pendiente Aprobación")
                {
                    try
                    {
                        ES.Estado_certificacion = "Rechazado";
                        ES.Motivo_rechazo = rechazo.MotivoRechazo;
                        ES.Fecha_rechazo = DateTime.Today;
                        repositorioEntradaServicio.GuardarCambios();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    try
                    {
                        rechazo.GeneradoPor = ES.Aprobador_CDS;
                        rechazo.NroOC = ES.NRO_OC;
                        rechazo.MontoTotal = rechazo.Moneda == "ARP" ? "$ " + ES.Monto_total.ToString() : rechazo.Moneda + " " + ES.Monto_total.ToString();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            //MMSN-1158
            try
            {
                Task.Run(() => NotifyRejection(rechazo)).Wait();
            }
            catch (Exception e)
            {
                throw e;
            }

            DateTime fechaRechazoFormateada = (DateTime)EntradasDeServicioTemp[0].Fecha_rechazo;
            ret.Fecha_rechazo_string = fechaRechazoFormateada.ToString("dd/MM/yyyy");
            ret.result = EntradasDeServicioTemp;

            return ret;
        }

        public async Task<bool> NotifyRejection(EmailDetailCertificateDto emailDetailCertificateDto)
        {
            bool response = false;
            var aprobaciones = repositorioEntradaServicio.Obtener<Aprobaciones>(a => a.NRO_ES_LOCAL == emailDetailCertificateDto.NumeroCertificacion);

            if (emailDetailCertificateDto != null && !string.IsNullOrEmpty(emailDetailCertificateDto.Destinatario))
            {
                await emailCertificationService.SendNotifyRejectionEmail(emailDetailCertificateDto);

                if (aprobaciones.Notificaciones_enviadas == false)
                {
                    aprobaciones.Notificaciones_enviadas = true;

                    repositorioEntradaServicio.GuardarCambios();
                }

                response = true;
            }

            return response;
        }

        /// <summary>
        /// Método para obtener resultado con dos decimales como máximo.
        /// </summary>
        /// <param name="monto"></param>
        /// <param name="cantidad"></param>
        /// <returns></returns>
        public static double DividirConDosDecimales(double monto, int cantidad)
        {
            double resultado = monto / cantidad;

            // Redondear el resultado a dos decimales
            resultado = Math.Round(resultado, 2);

            return resultado;
        }

        /// <summary>
        /// Método para formatear la fecha
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <returns></returns>
        public static string FormatearFecha(string fechaOriginal)
        {
            // Convertir la cadena de fecha a un objeto DateTime con el formato adecuado
            DateTime fecha = DateTime.ParseExact(fechaOriginal, "d/M/yyyy HH:mm:ss", null);

            // Formatear la fecha como una cadena con el formato deseado "yyyy/M/d"
            string fechaFormateada = fecha.ToString("yyyy-MM-dd");

            return fechaFormateada;
        }

        public List<Aprobaciones> GetESTemporaria(string nroESLocal)
        {
            List<Aprobaciones> toReturn = new List<Aprobaciones>();
            try
            {
                toReturn = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == nroESLocal).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }


            return toReturn;
        }

        public string GetCurrencyType(string NroOC)
        {
            var obtenerOrdenConsumer = new ObtenerOrdenDeCompraConsumerMOA(repositorioEntradaServicio);

            List<TablaSap> centros = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Centro");
            List<TablaSap> almacenes = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Almacen");

            DetalleOrdenDeCompraDto detalleOrdendeCompra = obtenerOrdenConsumer.ObtenerDetalleDeOrdenDeCompra(NroOC, centros, almacenes, false);

            return detalleOrdendeCompra.Posiciones[0].MonedaDescripcion;
        }

        /// <summary>
        /// Reasigna el suplente del usuario fiscal a la CES como Aprobador
        /// Reasigna al fiscal de nuevo como Aprobador
        /// Solo el fiscal puede reasignar.
        /// </summary>
        /// <param name="nro_es_local"></param>
        /// <param name="suplente"></param>
        /// <returns></returns>
        public EntradaServicioReasignacionRespuestaDto ReasignarSuplente(string nro_es_local, string suplente)
        {
            EntradaServicioReasignacionRespuestaDto resp = new EntradaServicioReasignacionRespuestaDto();
            List<Aprobaciones> esTemporalPendienteAprobacionList = repositorioEntradaServicio.Listar<Aprobaciones>(t => t.NRO_ES_LOCAL == nro_es_local);

            try
            {
                SustitucionMOAModel.Entities.Usuario user = repositorioEntradaServicio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.Mail == suplente);


                foreach (Aprobaciones esTemporalPendienteAprobacion in esTemporalPendienteAprobacionList)
                {

                    if (esTemporalPendienteAprobacion.NRO_ES_SAP.HasValue || esTemporalPendienteAprobacion.Estado_certificacion != "Pendiente Aprobación")
                    {
                        throw new ValidationCustomException("Entrada de servicio ya tratada.");
                    }

                    if (esTemporalPendienteAprobacion != null)
                    {
                        if (suplente == esTemporalPendienteAprobacion.Fiscal_SOLPED)
                        {
                            esTemporalPendienteAprobacion.Suplente = esTemporalPendienteAprobacion.Aprobador_CDS;
                            esTemporalPendienteAprobacion.Aprobador_CDS = esTemporalPendienteAprobacion.Fiscal_SOLPED;
                        }
                        else
                        {
                            esTemporalPendienteAprobacion.Suplente = esTemporalPendienteAprobacion.Fiscal_SOLPED;
                            esTemporalPendienteAprobacion.Aprobador_CDS = suplente;

                        }

                    }
                    else
                    {
                        throw new ValidationCustomException("Nro de entrada servicio no encontrado.");
                    }
                }

                OrderParamsDto orderParams = new OrderParamsDto();
                orderParams.OrdenCompraId = esTemporalPendienteAprobacionList[0].NRO_OC;
                Proveedor prov = new Proveedor();
                prov = orderService.BuscarProveedor(orderParams);

                string nroEsLocal = esTemporalPendienteAprobacionList[0].NRO_ES_LOCAL;

                List<Aprobaciones> aprobaciones = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == nroEsLocal);
                List<ReporteDto> reporte = new List<ReporteDto>();
                _ = NotifyCreation(aprobaciones, prov, user.Id, esTemporalPendienteAprobacionList[0].Aprobador_CDS);

                repositorioEntradaServicio.GuardarCambios();

            }
            catch (Exception e)
            {
                throw e;
            }

            resp.newApprover = esTemporalPendienteAprobacionList[0].Aprobador_CDS;
            resp.newSubstitute = esTemporalPendienteAprobacionList[0].Suplente;
            resp.status = "Se reasigno el suplente de la Entrada de servicio éxitosamente.";

            return resp;
        }

        public string ActualizarInformacionIngresante(IngresanteInfoEditableDto info)
        {
            try
            {
                Aprobaciones ESTemporal = repositorioEntradaServicio.Obtener<Aprobaciones>(t => t.ID == info.ID);
                if (ESTemporal != null)
                {
                    if (info.ColumnaEditar == "FechaDocumento")
                    {
                        DateTime fecha;
                        if (DateTime.TryParseExact(info.NuevoValor, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None, out fecha))
                        {
                            ESTemporal.Fecha_Documento = fecha;
                        }
                    }
                    else if (info.ColumnaEditar == "DescripcionES")
                    {
                        ESTemporal.Descripcion_ES = info.NuevoValor;
                    }
                    else if (info.ColumnaEditar == "Remito")
                    {
                        ESTemporal.Referencia = info.NuevoValor;
                    }
                    repositorioEntradaServicio.GuardarCambios();
                }
                else
                {
                    return "Nro de entrada servicio no encontrado.";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Actualización exitosa";
        }

        public void GenerarCertificacionAutomaticaPorLiberacionOC(string nroOC)
        {
            var detalleOC = ObtenerDetalleOrdenDeCompra(nroOC) ?? throw new ArgumentNullException("No se pudo obtener el detalle de la OC " + nroOC);
            var listadoNroSolps = detalleOC.Posiciones.Select(a => a.NumeroSolp).ToList();
            var solps = repositorioEntradaServicio.ObtenerSolpsAutocertificablesDeOC(listadoNroSolps);
            if (!solps.Any())
            {
                Logger.Log.Debug("No se encontraron solps Autocertificables para la oc:" + nroOC);
                return;
            }

            var obtenerOrdenConsumer = new ObtenerOrdenDeCompraConsumerMOA(repositorioEntradaServicio);
            var centrosSap = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Centro");
            var almacenesSap = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Almacen");
            var solicitudesMailAprobacionES = new List<MailAprobacionESRequest>();


            foreach (var posicionOC in detalleOC.Posiciones)
            {
                var solpNro = posicionOC.NumeroSolp;
                var solpACertificar = solps.FirstOrDefault(s => s.NroSolp == solpNro);

                if (posicionOC.Bloqueada || posicionOC.EsConEntregaFinal || solpACertificar == null || !posicionOC.Items.Any())
                {
                    continue;
                }

                try
                {
                    var crearESParamsDto = ObtenerDatosPosicionCertificar(posicionOC);

                    if (crearESParamsDto.EntrySheetServices.Items.Any())
                    {
                        CrearEntradaServicioCertificacionAutomatica(crearESParamsDto, solpACertificar, solicitudesMailAprobacionES, obtenerOrdenConsumer, centrosSap, almacenesSap, detalleOC.Proveedor);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex);
                    emailCertificationService.EnviarMailCertificacionAutomatica(nroOC, solpNro,
                        "No se pudo generar la certificación automática, deberá hacerlo manualmente. Error: " + ex.Message,
                        new string[] { solpACertificar.UsuarioCreacion.Mail, EmailEnvioErrores });
                }
            }

            solicitudesMailAprobacionES.ForEach(s => emailCertificationService.EnviarMailAprobacion(s));
        }

        /// <summary>
        /// Realiza ordenamiento del objeto OrdenCompraDto según la columna y el tipo de orden especificados
        /// </summary>
        private List<EntradaServicioCabeceraDto> OrdenarEntradasServicio(List<EntradaServicioCabeceraDto> ordenes)
        {
            return ordenes.OrderByDescending(es => es.FechaCreacionDateTime).ThenByDescending(es => es.EntradaServicio).ToList();
        }

        /// <summary>
        /// Consultas SAP cabecera de documento.
        /// No lista las entradas de servicio que esté en estado "Borrada"
        /// </summary>
        private async Task<List<EntradaServicioCabeceraDto>> ServicioSAP_EntradasServicioCabecera(EntradaServicioParamsDto parametros, UsuarioDto usuario)
        {
            var entradasServicioResponse = new List<EntradaServicioCabeceraDto>();
            var entradasServicioCabeceraSap = await new ObtenerCabecerasEntradaServicioConsumerMOA().ObtenerEntradasServicioCabeceraAsync(parametros.FechaInicio, parametros.OrdenCompra);

            var correoUsuario = usuario.Mail.ToLower();

            var fechaHasta = DateTime.Now;

            var debeFiltrarPorFecha =
                string.IsNullOrEmpty(parametros.OrdenCompra) &&
                !string.IsNullOrEmpty(parametros.FechaFin) &&
                DateTime.TryParseExact(parametros.FechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaHasta);

            entradasServicioCabeceraSap = entradasServicioCabeceraSap
                .Where(x =>
                    x.OrdenCompra.StartsWith("412") && //Se filtran por las OC tomando las que empiezan con 412
                    (!debeFiltrarPorFecha || !x.FechaCreacionDateTime.HasValue || x.FechaCreacionDateTime <= fechaHasta)
                ).ToList();

            // Filtra por número de documento, si se proporciona el parámetro
            if (parametros.DocumentoNumero != null)
                entradasServicioCabeceraSap = entradasServicioCabeceraSap.Where(orden => orden.EntradaServicio.ToString() == parametros.DocumentoNumero).ToList();

            var ordenParams = new OrderParamsDto();

            // ES APROBADAS
            foreach (var documento in entradasServicioCabeceraSap)
            {
                var correoSolp = "-";
                var nroDoc = documento.EntradaServicio.ToString();
                var nro_es_sap = int.Parse(nroDoc);

                // Se obtiene detalle de una ES
                documento.entradaServicioDetalle = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicioDetalle(nroDoc);

                if (documento.entradaServicioDetalle != null && documento.entradaServicioDetalle.Count > 0)
                {
                    if (ordenParams.OrdenCompraId != documento.entradaServicioDetalle[0].OrdenCompra || string.IsNullOrEmpty(ordenParams.OrdenCompraId))
                    {
                        var ocSap = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraRFC(documento.entradaServicioDetalle[0].OrdenCompra);
                        ModificarPedidoSAP POSCHEDULE = new ModificarPedidoSAP
                        {
                            NRO_SOLP = ocSap.POSCHEDULE.FirstOrDefault().PREQ_NO
                        };

                        string nroSolp = POSCHEDULE.NRO_SOLP;

                        correoSolp = ObtenerCorreoSolp(nroSolp);
                    }

                    ordenParams.OrdenCompraId = documento.entradaServicioDetalle[0].OrdenCompra;

                    var proveedor = orderService.BuscarProveedor(ordenParams);
                    documento.Proveedor = proveedor.RazonSocial ?? "-";
                    documento.CUIT = proveedor.CUIT ?? "-";
                }

                // Se obtiene detalle de la APROBACIÓN de la Entrada de Servicio
                List<Aprobaciones> ESTemporales = new List<Aprobaciones>();

                bool verTodo = parametros.VerTodo && usuario.Permisos.Contains("VER TODOS LOS ESTADOS DE ES");
                bool certExt = usuario.Permisos.Contains("VER SOLAPA CERTIFICACION DE SERVICIOS EXTERNA") && !usuario.Permisos.Contains("VER SOLAPA CERTIFICACION DE SERVICIOS");

                if (verTodo)
                {
                    ESTemporales = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_ES_SAP == nro_es_sap);
                }
                else
                {
                    ESTemporales = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_ES_SAP == nro_es_sap &&
                        (x.Ingresante_CDS.ToLower() == correoUsuario ||
                        x.Fiscal_SOLPED.ToLower() == correoUsuario ||
                        x.Aprobador_CDS.ToLower() == correoUsuario ||
                        (certExt && x.Proveedor == parametros.Vendedor)));
                }

                if (ESTemporales != null && ESTemporales.Count > 0)
                {
                    List<Aprobaciones> detalleAprobacionesTemporales = ESTemporales.Where(t => t.NRO_ES_SAP == int.Parse(documento.EntradaServicio)).ToList();

                    var detalleEntradadeServicio = MergeDetalle(documento, detalleAprobacionesTemporales, correoSolp);

                    documento.entradaServicioDetalle = detalleEntradadeServicio.entradaServicioDetalle;
                }
                else
                {
                    documento.Fiscal = correoSolp;
                    DateTime fecha = DateTime.Parse(documento.FechaCreacion); // FechaCreacion es la fecha de la alta en sap no es la fecha_carga_es de aprobaciones.
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    documento.FechaAprobacion = fechaFormateada;
                    documento.FechaCreacion = fechaFormateada;
                }

                documento.Estado = "Aprobada";

                entradasServicioResponse.Add(documento);
            }

            return entradasServicioResponse.Where(x => x.Ingresante.Contains("@")).ToList();
        }

        /// <summary>
        /// MMSN-601: Validación de información ingresante c/ SolPed
        /// </summary>
        private EntradaServicioCreateRespuestaDto ValidarIngresante(string userMail, string nroSolped)
        {
            //MMSN-601 agregar lógica entrada servicio automatica- temporal, nro solped en parametros.Header.Solp
            //1 - Obtener información asociada a SolPed
            SolpESDto detalleSolPed = new SolpESDto();
            try
            {
                if (!string.IsNullOrEmpty(nroSolped))
                {
                    detalleSolPed = comprasService.TraerSolpPorNumero(nroSolped);
                }
                else
                {
                    return new EntradaServicioCreateRespuestaDto { Type = "S", Message = "No se encontró la SOLP" };
                }
            }
            catch (Exception e)
            {
                return new EntradaServicioCreateRespuestaDto { Type = "S", Message = e.Message };
            }

            //2 - Comparar datos SolPed para certificar automaticamente o WKF de aprobaciones
            bool auto = false;
            bool difSolicitante = false;

            var usuarioIngresante = repositorioEntradaServicio.Obtener<Usuario>(x => x.Mail == userMail);
            var usuarioReasignacion = repositorioEntradaServicio.Obtener<UsuarioReasignacion>(x => x.Usuario_Id == usuarioIngresante.Id);

            //2a - Comparar Fiscal/Email con usuario FE

            if (usuarioIngresante.Externo != null && usuarioIngresante.Externo == true)
            {
                auto = false;
                difSolicitante = false;
            }
            else if ((userMail == detalleSolPed.Email && usuarioReasignacion != null &&
                DateTime.Now <= usuarioReasignacion.FechaHasta && DateTime.Now >= usuarioReasignacion.FechaDesde)
                || (userMail == detalleSolPed.Email))
            {
                auto = true;
            }
            else if (detalleSolPed.SupervisorTrabajo != null && detalleSolPed.SupervisorTrabajo.Count > 0
                && userMail == detalleSolPed.SupervisorTrabajo[0])
            {
                //2b - Si el supervisor del trabajo es el mismo que el usuario ingresante
                auto = true;
            }
            else if (string.IsNullOrEmpty(detalleSolPed.Email) && detalleSolPed.SupervisorTrabajo != null && detalleSolPed.SupervisorTrabajo.Count > 0
                && string.IsNullOrEmpty(detalleSolPed.SupervisorTrabajo[0]) && detalleSolPed.Posiciones != null
                && detalleSolPed.Posiciones.Count > 0)
            {
                //2c - Si el solicitante de la SolPed es el mismo que el usuario ingresante
                foreach (var pos in detalleSolPed.Posiciones)
                {
                    //TODO: En este punto se deberá validar si es un usuario que coincida con el campo “Usuario SAP” en el ABM de usuarios. Si coincide, sería el fiscal/aprobador
                    if (!pos.Solicitante.IsNullOrWhiteSpace())
                    {
                        if (userMail == pos.Solicitante)
                        {
                            auto = true;
                        }
                        else
                        {
                            //Si no coincide el email con el campo solicitante, buscar el valor de campo solicitante (EN MAYUSCULAS Y SIN ESPACIOS) (todo junto sin espacios).
                            //Si existe, traer los datos del usuario, y comparar usuario.email con usermail, si son iguales, aprobación automatica.
                            string solicitante = pos.Solicitante.Replace(" ", "");
                            var usuario = repositorioEntradaServicio.Obtener<Usuario>(x => x.UsuarioSap.Trim().ToUpper() == solicitante.Trim().ToUpper());

                            if (usuario != null && usuario.Mail == userMail)
                            {
                                //Aca es donde se aporueba automaticamente
                                auto = true;
                                difSolicitante = false;
                            }
                            else if (usuario != null && usuario.Mail != userMail)
                            {
                                auto = false;
                                difSolicitante = false;
                            }
                            else if (usuario == null)
                            {
                                difSolicitante = true;
                            }
                        }
                    }
                }
            }
            //Aca - Si los 3 datos estan vacios o no vienen -> “No se identifica un aprobador en su orden de compra. Por favor, comunicarse con su contratante”. 
            if (auto == false)
            {
                bool empty = EmptySolPedValues(detalleSolPed);
                if (empty || difSolicitante)
                {
                    EntradaServicioCreateRespuestaDto emptySolPed = new EntradaServicioCreateRespuestaDto();
                    emptySolPed.Type = "S";
                    emptySolPed.Message = "No se identifica un aprobador en su orden de compra. Por favor, comunicarse con su contratante";
                    return emptySolPed;
                }
            }

            EntradaServicioCreateRespuestaDto result = new EntradaServicioCreateRespuestaDto();
            if (auto == true)
            {
                //Provisional - Pendiente desarrollo ticket 602 - ES Temporal
                result.Type = "S";
                result.Message = "Auto";
            }
            else
            {
                result.Type = "S";
                result.Message = "Temporal";
            }

            return result;
        }

        private EntradaServicioCreateRespuestaDto CrearEntradaServicio(EntradaServicioCreateParamsDto posicion,
            string userMail, List<ReporteDto> reporte, List<string> idAdjuntos, string solpedNumber, string proveedor = null)
        {
            SustitucionMOAWS.Logger.Log.Info("EntradaServicioService.CrearEntradaServicio");

            // 3 - Si alguna de las validaciones es correcta, alta automatica.
            EntradaServicioCreateRespuestaDto result = new CrearEntradaDeServicioConsumerMOA().CrearEntradaServicio(posicion);

            ////MMSN-602 - Cargar en tabla aprobaciones si se creo la ES.
            if (result.Type == "I" && result.Id == "SE")
            {
                int ESNumber = GetESNumber(result.Message);
                Aprobaciones ap = GuardarDatosES(posicion, userMail, ESNumber, true, reporte, solpedNumber, proveedor);

                ActualizarAdjuntosConES(idAdjuntos, ap.NRO_ES_LOCAL);
            }
            return result;
        }

        /// <summary>
        /// mergea los datos del detalle que vienen de sap con los datos que se guardaron en la temporal
        /// </summary>
        /// <param name="entradaServicioSAP">detalle que devuelve sap</param>
        /// <param name="detalleAprobacionesTemporales">detalle que se guardo en la temporal</param>
        /// <param name="emailFiscal"></param>
        private EntradaServicioCabeceraDto MergeDetalle(EntradaServicioCabeceraDto entradaServicioSAP, List<Aprobaciones> detalleAprobacionesTemporales, string emailFiscal)
        {
            foreach (var detalle in entradaServicioSAP.entradaServicioDetalle)
            {
                var detalleAprobacion = detalleAprobacionesTemporales.FirstOrDefault(x => x.Planned_line == detalle.NumeroLinea && x.Planned_package == detalle.PLN_PCKG);

                if (detalleAprobacion != null)
                {
                    detalle.NumeroLinea = int.Parse(detalleAprobacion.Nro_linea).ToString();
                    detalle.TextoBreveServicio = string.IsNullOrEmpty(detalleAprobacion.Texto_breve_servicio) || detalleAprobacion.Texto_breve_servicio == "Este campo es ignorado por el servicio SAP, pero debe enviarsele algo" ? "" : detalleAprobacion.Texto_breve_servicio.Trim();
                    detalle.CantidadCertificar = detalleAprobacion.Cantidad_a_certificar;
                    detalle.PorcentajeCertificar = detalleAprobacion.Porcentaje_a_certificar;
                    detalle.MontoCertificar = detalleAprobacion.Monto_a_certificar;
                    detalle.NroRemito = detalleAprobacion.Referencia;
                    detalle.CodigoServicio = string.IsNullOrEmpty(detalle.CodigoServicio) ? "0" : detalle.CodigoServicio;
                    detalle.NroPosicion = int.Parse(detalleAprobacion.NRO_POS).ToString();
                    detalle.Cantidad = Convert.ToDecimal(detalleAprobacion.Cantidad, CultureInfo.InvariantCulture).ToString();
                    detalle.CantidadAnterior = Convert.ToDouble(detalleAprobacion.Cantidad_Anterior);

                    entradaServicioSAP.MotivoRechazo = detalleAprobacion.Motivo_rechazo;
                    entradaServicioSAP.NumeroCertificacion = detalleAprobacion.NRO_ES_LOCAL;
                    entradaServicioSAP.Ingresante = detalleAprobacion.Ingresante_CDS;
                    entradaServicioSAP.Aprobador = detalleAprobacion.Aprobador_CDS;
                    entradaServicioSAP.Suplente = detalleAprobacion.Suplente;
                    entradaServicioSAP.Fiscal = detalleAprobacion.Fiscal_SOLPED;
                    entradaServicioSAP.Descripcion = string.IsNullOrEmpty(detalleAprobacion.Texto_breve_servicio) ? "" : detalleAprobacion.Texto_breve_servicio.Trim();
                    DateTime fechaAprobacionFormateada = (DateTime)detalleAprobacion.Fecha_aprobacion;
                    entradaServicioSAP.FechaAprobacion = fechaAprobacionFormateada.ToString("dd/MM/yyyy");
                    DateTime FechaCreacion = (DateTime)detalleAprobacion.Fecha_Carga_ES;
                    entradaServicioSAP.FechaCreacion = FechaCreacion.ToString("dd/MM/yyyy");
                    entradaServicioSAP.AnuladaPor = detalleAprobacion.Anulado_por;
                }
                else
                {
                    entradaServicioSAP.Fiscal = emailFiscal;
                    DateTime fecha = DateTime.Parse(entradaServicioSAP.FechaCreacion); // FechaCreacion es la fecha de la alta en sap no es la fecha_carga_es de aprobaciones.
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    entradaServicioSAP.FechaAprobacion = fechaFormateada;
                    entradaServicioSAP.FechaCreacion = fechaFormateada;
                }
            }
            return entradaServicioSAP;
        }

        private void ActualizarAdjuntosConES(List<string> idAdjuntos, string nroESTemporal)
        {
            if (idAdjuntos != null && idAdjuntos.Count > 0)
            {
                var adjuntos = repositorioEntradaServicio.Listar<AdjuntosEntradasDeServicio>(x => idAdjuntos.Contains(x.NombreEnBlob))
                                                          .GroupBy(a => a.NombreEnBlob)
                                                          .Select(g => g.First())
                                                          .ToList();
                foreach (var adjunto in adjuntos)
                {
                    if (string.IsNullOrEmpty(adjunto.NroESTemporal))
                    {
                        adjunto.NroESTemporal = nroESTemporal;
                    }
                    else
                    {
                        var adjuntoNuevo = new AdjuntosEntradasDeServicio()
                        {
                            NombreArchivo = adjunto.NombreArchivo,
                            Extension = adjunto.Extension,
                            NombreEnBlob = adjunto.NombreEnBlob,
                            NroESTemporal = nroESTemporal
                        };

                        repositorioEntradaServicio.Agregar(adjuntoNuevo);
                    }
                }
                repositorioEntradaServicio.GuardarCambios();
            }
        }

        private EntradaServicioCreateRespuestaDto CrearEntradaServicioTemporal(EntradaServicioCreateParamsDto posiciones, string userMail, List<ReporteDto> reporte,
            List<string> idAdjuntos, List<MailAprobacionESRequest> solicitudesMailAprobacionES, ObtenerOrdenDeCompraConsumerMOA obtenerOrdenConsumer,
            List<TablaSap> centrosSap, List<TablaSap> almacenesSap, string solpedNumber = null, string proveedorCodigo = null)
        {
            try
            {
                var nuevaAprobacion = GuardarDatosES(posiciones, userMail, 0, false, reporte, solpedNumber, proveedorCodigo);
                ActualizarAdjuntosConES(idAdjuntos, nuevaAprobacion.NRO_ES_LOCAL);

                var respuestaCrearES = new EntradaServicioCreateRespuestaDto
                {
                    Type = "S",
                    Message = $"Se generó la entrada de servicio {nuevaAprobacion.NRO_ES_LOCAL} en estado {nuevaAprobacion.Estado_certificacion}, a verificar por Contratante o Solicitante."
                };

                if (nuevaAprobacion != null)
                {
                    var aprobacionesES = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == nuevaAprobacion.NRO_ES_LOCAL);

                    if (aprobacionesES.Count > 0)
                    {
                        var orderParams = new OrderParamsDto { OrdenCompraId = nuevaAprobacion.NRO_OC };
                        var proveedor = orderService.BuscarProveedor(orderParams);

                        var aprobadorMail = aprobacionesES[0].Aprobador_CDS;
                        var user = repositorioEntradaServicio.Listar<Usuario>(x => x.Mail == aprobadorMail).FirstOrDefault();
                        var userId = (user != null ? user.Id : 0);

                        AgregarPosicionASolicitudesMailAprobacionES(solicitudesMailAprobacionES, aprobadorMail, userId, proveedor, aprobacionesES, obtenerOrdenConsumer, centrosSap, almacenesSap);

                        aprobacionesES.ForEach(x => x.Notificaciones_enviadas = true);
                        repositorioEntradaServicio.GuardarCambios();
                    }
                }
                return respuestaCrearES;
            }
            catch (AggregateException ae)
            {
                var messageBuilder = new StringBuilder();

                messageBuilder
                    .Append(ae.Message)
                    .AppendLine(":");

                Logger.Log.Error(ae);

                foreach (Exception e in ae.InnerExceptions)
                {
                    messageBuilder.AppendLine(e.Message);
                    Logger.Log.Error(e);
                }
                return new EntradaServicioCreateRespuestaDto { Type = "E", Message = messageBuilder.ToString() };
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
                return new EntradaServicioCreateRespuestaDto { Type = "E", Message = e.Message };
            }
        }

        private async Task<bool> NotifyCreation(List<Aprobaciones> completeAp, Proveedor prov, int userId, string destinatario)
        {
            var obtenerOrdenConsumer = new ObtenerOrdenDeCompraConsumerMOA(repositorioEntradaServicio);
            var centrosSap = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Centro");
            var almacenesSap = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Almacen");

            var solicitudesMailAprobacionES = new List<MailAprobacionESRequest>();
            AgregarPosicionASolicitudesMailAprobacionES(solicitudesMailAprobacionES, destinatario, userId, prov, completeAp, obtenerOrdenConsumer, centrosSap, almacenesSap);

            emailCertificationService.EnviarMailAprobacion(solicitudesMailAprobacionES.First());

            return true;
        }

        private void AgregarPosicionASolicitudesMailAprobacionES(List<MailAprobacionESRequest> solicitudesMailAprobacionES, string destinatarioMail, int usuarioId,
            Proveedor proveedor, List<Aprobaciones> aprobaciones, ObtenerOrdenDeCompraConsumerMOA obtenerOCConsumer, List<TablaSap> centrosSap, List<TablaSap> almacenesSap)
        {
            solicitudesMailAprobacionES = solicitudesMailAprobacionES ?? new List<MailAprobacionESRequest>();

            var solicitudAprobador = solicitudesMailAprobacionES.FirstOrDefault(s => s.DestinatarioMail == destinatarioMail);
            if (solicitudAprobador == null)
            {
                solicitudAprobador = new MailAprobacionESRequest { DestinatarioMail = destinatarioMail, UsuarioId = usuarioId, Posiciones = new List<MailAprobacionESPosicion>() };
                solicitudesMailAprobacionES.Add(solicitudAprobador);
            }

            var detalleOCDto = obtenerOCConsumer.ObtenerDetalleDeOrdenDeCompra(aprobaciones.First().NRO_OC, centrosSap, almacenesSap, true);
            var reportesES = NuevoReporteReasignacion(aprobaciones, detalleOCDto, detalleOCDto.Posiciones[0].MonedaDescripcion).GetAwaiter().GetResult();

            var nrosCertificaciones = aprobaciones.Select(ap => ap.NRO_ES_LOCAL);
            var adjuntosES = repositorioEntradaServicio.Listar<AdjuntosEntradasDeServicio>(adj => nrosCertificaciones.Contains(adj.NroESTemporal));

            var nuevaPosicion = new MailAprobacionESPosicion
            {
                ProveedorRazonSocial = proveedor.RazonSocial,
                Aprobacion = aprobaciones.First(),
                Reportes = reportesES,
                Adjuntos = adjuntosES
            };

            solicitudAprobador.Posiciones.Add(nuevaPosicion);
        }

        private async Task<List<ReporteDto>> NuevoReporteReasignacion(List<Aprobaciones> esTemp, DetalleOrdenDeCompraDto detalleOrdendeCompra, string moneda)
        {
            const string pendienteAprobacion = "Pendiente Aprobación";
            List<ReporteDto> nuevoReporte = new List<ReporteDto>();
            foreach (Aprobaciones ap in esTemp)
            {
                ReporteDto reporte = new ReporteDto();
                int nroLinea = int.Parse(ap.Nro_linea);
                string package = ap.Planned_line;
                long nroPosicion = long.Parse(ap.NRO_POS);
                decimal cantidadACertificar = Convert.ToDecimal(ap.Cantidad_a_certificar, CultureInfo.InvariantCulture);
                decimal porcentajeACertificar = Convert.ToDecimal(ap.Porcentaje_a_certificar, CultureInfo.InvariantCulture);

                decimal totalACertificar = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_OC == ap.NRO_OC && x.NRO_POS == ap.NRO_POS
                     && x.Nro_linea == ap.Nro_linea && x.Estado_certificacion == pendienteAprobacion)
                    .Select(a => new { Cantidad = Convert.ToDecimal(a.Cantidad_a_certificar, CultureInfo.InvariantCulture) })
                    .Sum(a => a.Cantidad);

                var position = detalleOrdendeCompra.Posiciones.First(x => x.NumeroPosicion == nroPosicion);
                var item = position.Items.First(x => x.NumeroLinea == nroLinea && x.LINE_NO == package);

                //cantidad real es cantidad anterior

                item.CantidadReal = Convert.ToDecimal(ap.Cantidad_Anterior);

                //if (totalACertificar > cantidadACertificar)
                //{
                //    item.CantidadReal = item.CantidadReal + (totalACertificar - cantidadACertificar);
                //} else if(totalACertificar < cantidadACertificar)
                //{
                //    item.CantidadReal = item.CantidadReal + (cantidadACertificar - totalACertificar);
                //} else if(totalACertificar == cantidadACertificar)
                //{
                //    item.CantidadReal = item.CantidadReal + totalACertificar;
                //}

                double res = Convert.ToDouble((item.CantidadReal * 100) / item.Cantidad);
                item.Porcentaje = res.ToString("0.##", CultureInfo.InvariantCulture);
                if (item.Porcentaje.EndsWith(".00"))
                {
                    var redondeo = Math.Round(res);
                    item.Porcentaje = res.ToString(CultureInfo.InvariantCulture);
                }

                reporte.NumeroLinea = nroLinea;
                reporte.ServicioNumero = int.Parse(ap.Nro_servicio);
                reporte.Descripcion = ap.Descripcion_ES;
                reporte.Cantidad = (double)ap.Cantidad;
                reporte.UM = ap.UM;
                reporte.Importe = (decimal)item.Importe;
                reporte.NroPosicion = nroPosicion.ToString();
                reporte.Porcentaje = item.Porcentaje;
                reporte.CantidadReal = (decimal)item.CantidadReal;
                reporte.CantidadACertificar = cantidadACertificar;
                reporte.PorcentajeACertificar = porcentajeACertificar;
                reporte.Moneda = moneda;
                nuevoReporte.Add(reporte);
            }
            return nuevoReporte;
        }

        /// <summary>
        /// MMSN-601: Metodo para validar si los valores de detalleSolPed estan vacios 
        /// </summary>
        private bool EmptySolPedValues(SolpESDto detalleSolPed)
        {
            bool result = false;
            if (detalleSolPed.Email.IsNullOrWhiteSpace())
            {
                if (detalleSolPed.SupervisorTrabajo == null || detalleSolPed.SupervisorTrabajo.Count == 0 || (detalleSolPed.SupervisorTrabajo != null && detalleSolPed.SupervisorTrabajo[0].IsNullOrWhiteSpace()))
                {
                    if (detalleSolPed.Posiciones == null || detalleSolPed.Posiciones.Count == 0)
                    {
                        result = true;
                    }
                    else
                    {
                        bool hasValue = false;
                        foreach (var pos in detalleSolPed.Posiciones)
                        {
                            if (!pos.Solicitante.IsNullOrWhiteSpace())
                            {
                                hasValue = true;
                            }
                        }
                        if (!hasValue)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// MMSN-602: Obtiene número de ES desde el mensaje, necesario registrar en tabla Aprobaciones.
        /// </summary>
        private int GetESNumber(string message)
        {
            int ESNumber = 0;

            Regex regex = new Regex(@"\d+");

            MatchCollection matches = regex.Matches(message);

            // Iterate through matches and extract the first number found
            foreach (Match match in matches)
            {
                if (int.TryParse(match.Value, out ESNumber))
                {
                    // If successfully parsed, break the loop
                    break;
                }
            }
            return ESNumber;
        }

        /// <summary>
        /// MMSN-602: Guardar datos en tabla aprobaciones - Aprobación - Descripción de campos en Entity
        /// </summary>
        private Aprobaciones GuardarDatosES(EntradaServicioCreateParamsDto posicion, string userMail, int ESNumber,
            bool auto, List<ReporteDto> reporte, string solPedNumber = null, string proveedor = null)
        {
            Aprobaciones temp = new Aprobaciones();
            //MMSN-1066 - Derivacion automatica del suplente
            var user = new Usuario();

            #region CargaDatosCabecera
            DateTime dateDocument;

            if (String.IsNullOrEmpty(posicion.EntrySheetHeader.FechaDocumento))
            {
                posicion.EntrySheetHeader.FechaDocumento = DateTime.Today.ToString("yyyy-MM-dd");
            }

            if (DateTime.TryParseExact(posicion.EntrySheetHeader.FechaDocumento, "yyyy-MM-dd",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out dateDocument))
            {
                temp.Fecha_Documento = dateDocument;
            }
            DateTime dateAccounting;
            if (DateTime.TryParseExact(posicion.EntrySheetHeader.FechaContabilizacion, "yyyy-MM-dd",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None, out dateAccounting))
            {
                temp.Fecha_Contabilizacion = dateAccounting;
            }

            temp.Referencia = posicion.EntrySheetHeader.DocumentoReferenciaNumero;
            temp.Fecha_Carga_ES = DateTime.Today;
            temp.Notificaciones_enviadas = false;
            temp.Ingresante_CDS = userMail;
            temp.Proveedor = proveedor;


            //Datos dependientes de aprobación automatica o no
            if (auto)
            {
                temp.NRO_ES_SAP = ESNumber;
                temp.Estado_certificacion = "Aprobada";
                temp.Aprobada_automaticamente = true;
                temp.Aprobador_CDS = userMail;
                temp.Fecha_aprobacion = DateTime.Today;
            }
            else
            {
                temp.Estado_certificacion = "Pendiente Aprobación";
                temp.Aprobada_automaticamente = false;
            }
            #endregion


            //Datos SolPed
            #region DatosSolPed
            // Pendiente Carga temp.Area, ya que se necesitan los datos de MMSN-726
            try
            {
                if (!string.IsNullOrEmpty(solPedNumber))
                {
                    SolpESDto detalleSolPed = new SolpESDto();
                    try
                    {
                        detalleSolPed = comprasService.TraerSolpPorNumero(solPedNumber);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    if (!EmptySolPedValues(detalleSolPed))
                    {
                        //Busqueda Fiscal Contrato
                        if (!string.IsNullOrEmpty(detalleSolPed.Email))
                        {
                            if (detalleSolPed.Email.Contains("@"))
                            {
                                temp.Fiscal_SOLPED = detalleSolPed.Email;
                                var usuario = repositorioEntradaServicio.Obtener<Usuario>(x => x.Mail == detalleSolPed.Email);
                                if (usuario != null)
                                {
                                    user = usuario;
                                    temp.Suplente = usuario.Suplente;
                                }
                            }
                        }
                        else if (detalleSolPed.SupervisorTrabajo != null
                            && (detalleSolPed.SupervisorTrabajo.Count > 0 && !string.IsNullOrEmpty(detalleSolPed.SupervisorTrabajo[0])))
                        {
                            //Busqueda por Supervisor Trabajo
                            if (detalleSolPed.SupervisorTrabajo[0].Contains("@"))
                            {
                                temp.Fiscal_SOLPED = detalleSolPed.SupervisorTrabajo[0];
                                var usuario = repositorioEntradaServicio.Obtener<Usuario>(x => x.Mail == temp.Fiscal_SOLPED);
                                if (usuario != null)
                                {
                                    user = usuario;
                                    temp.Suplente = usuario.Suplente;
                                }
                            }
                            else
                            {
                                string aprobador = detalleSolPed.SupervisorTrabajo[0].Replace(" ", "");
                                aprobador = aprobador.ToUpper();
                                var usuario = repositorioEntradaServicio.Obtener<Usuario>(x => x.UsuarioSap.ToUpper() == aprobador);
                                if (usuario != null)
                                {
                                    user = usuario;
                                    temp.Fiscal_SOLPED = usuario.Mail;
                                    temp.Suplente = usuario.Suplente;
                                }
                            }
                        }
                        else if (detalleSolPed.Posiciones != null && detalleSolPed.Posiciones.Count > 0)
                        {
                            //Busqueda por Solicitante
                            foreach (var pos in detalleSolPed.Posiciones)
                            {
                                if (pos.Solicitante != null)
                                {
                                    string solicitante = pos.Solicitante.Replace(" ", "");
                                    solicitante = solicitante.ToUpper();
                                    var usuario = repositorioEntradaServicio.Obtener<Usuario>(x => x.UsuarioSap.ToUpper() == solicitante);
                                    if (usuario != null)
                                    {
                                        user = usuario;
                                        temp.Fiscal_SOLPED = usuario.Mail;
                                        temp.Suplente = usuario.Suplente;

                                    }
                                }
                            }
                        }

                    }
                    if (!auto)
                    {
                        temp.Aprobador_CDS = temp.Fiscal_SOLPED;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e);
            }
            #endregion

            //MMSN-1066 - Derivación automatica del suplente.
            #region DerivacionAutomatica
            if (!auto && user != null && user.Id != 0 && !string.IsNullOrEmpty(user.Suplente))
            {
                UsuarioReasignacion periodo = repositorioEntradaServicio.Listar<UsuarioReasignacion>(x => x.Usuario_Id == user.Id).ToList().LastOrDefault();
                if (periodo != null)
                {
                    //Comprobar fechaDesde y fechaHasta
                    if (periodo.FechaDesde <= DateTime.Today && periodo.FechaHasta >= DateTime.Today)
                    {
                        temp.Aprobador_CDS = user.Suplente;
                        temp.Suplente = temp.Fiscal_SOLPED;
                    }
                }
            }
            #endregion

            if (user != null && user.Id != 0 && user.Externo == true && !string.IsNullOrEmpty(user.Suplente))
            {
                var usuarioSuplente = repositorioEntradaServicio.Obtener<Usuario>(x => x.Mail == user.Suplente);
                temp.Aprobador_CDS = user.Suplente;
                temp.Suplente = usuarioSuplente.Suplente;
                temp.Fiscal_SOLPED = user.Suplente;
            }

            //Datos OC
            temp.NRO_OC = posicion.EntrySheetHeader.OrdenCompraNumero;
            temp.NRO_POS = posicion.EntrySheetHeader.OrdenCompraPosicionNumero;
            //Monto Total
            if (!string.IsNullOrEmpty(posicion.EntrySheetHeader.MontoTotalACertificar))
            {
                temp.Monto = double.Parse(posicion.EntrySheetHeader.MontoTotalACertificar, CultureInfo.InvariantCulture);
            }

            //Obtener último registro para nuevo número
            // Aprobaciones ultimoRegistro = new Aprobaciones();
            long ultimoRegistro = 0;
            try
            {
                ultimoRegistro = repositorioEntradaServicio.ExecuteQuery<long>("EXEC ObtenerSiguienteValorSecuencia").Single();

                //ultimoRegistro = repositorio.Listar<Aprobaciones>().LastOrDefault();
            }
            catch (Exception e)
            {
                SustitucionMOAWS.Logger.Log.Error(e);
                throw e;
            }

            string newESLocal = string.Empty;

            if (ultimoRegistro == 0)
            {
                //Primer registro en tabla
                temp.NRO_ES_LOCAL = "T_0000000001";
            }
            else
            {
                newESLocal = "T_" + ultimoRegistro.ToString("D10");

                temp.NRO_ES_LOCAL = newESLocal;
            }

            //MontoTotal = Suma de los montos a certificar de cada ES A APROBAR
            double monto_total = 0;
            List<Aprobaciones> toSave = new List<Aprobaciones>();

            #region CargaDeDatosPorItem
            //Datos por Item en ES
            foreach (EntrySheetServiceItemSection esItem in posicion.EntrySheetServices.Items)
            {
                temp.Descripcion_ES = esItem.Descripcion;

                //Copiar lo cargado hasta ahora
                var aprobacion = DeepCopy(temp);
                try
                {
                    if (!string.IsNullOrEmpty(esItem.ItemQuantity)) aprobacion.Cantidad = double.Parse(esItem.ItemQuantity, CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(esItem.ItemGrossPrice)) aprobacion.Monto = double.Parse(esItem.ItemGrossPrice, CultureInfo.InvariantCulture);

                    aprobacion.Nro_linea = esItem.ExternalLineNumber;
                    aprobacion.Nro_servicio = esItem.Service;
                    aprobacion.Texto_breve_servicio = esItem.ShortText.Trim();
                    aprobacion.UM = esItem.UM;
                    aprobacion.Cantidad_a_certificar = esItem.Quantity;
                    aprobacion.Porcentaje_a_certificar = esItem.Percentage;
                    aprobacion.Planned_package = esItem.PlannedPackage;
                    aprobacion.Planned_line = esItem.PlannedLine;

                    ReporteDto itemReport = reporte.Find(report => report.Id == esItem.PlannedPackage && report.LINE_NO.ToString() == esItem.PlannedLine);
                    if (itemReport != null)
                    {
                        aprobacion.Cantidad_Anterior = Decimal.ToDouble(itemReport.CantidadReal);
                    }

                    if (!string.IsNullOrEmpty(esItem.CertificationAmount))
                    {
                        aprobacion.Monto_a_certificar = double.Parse(esItem.Quantity, CultureInfo.InvariantCulture) * double.Parse(esItem.ItemGrossPrice, CultureInfo.InvariantCulture);
                        monto_total = (double)(monto_total + aprobacion.Monto_a_certificar);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error(e);
                    throw e;
                }

                toSave.Add(aprobacion);
            }
            #endregion

            if (toSave.Count > 0)
            {
                //Agregar Monto Total y grabar
                foreach (Aprobaciones ap in toSave)
                {
                    ap.Monto_total = monto_total;
                    repositorioEntradaServicio.Agregar<Aprobaciones>(ap);
                }
                //Transaccion
                try
                {
                    repositorioEntradaServicio.GuardarCambios();
                }
                catch (Exception e)
                {
                    Logger.Log.Error(e);
                    throw e;
                }
            }

            Task.Run(() => GenerateAndSaveReportInBlob(reporte, newESLocal)).Wait();

            //Para mensaje de retorno de ES Temporal (sin aprobación automatica) se necesita mostrar datos de NRO_ES_LOCAL y estado.
            return temp;
        }

        private async Task GenerateAndSaveReportInBlob(List<ReporteDto> reporte, string blobReference)
        {
            if (reporte.Count > 0)
            {
                await this._reporteESService.BuildReportES(reporte, blobReference).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// MMSN-602 - Copiar objeto sin sobreescribir datos
        /// </summary>
        private Aprobaciones DeepCopy(Aprobaciones source)
        {
            Aprobaciones copy = new Aprobaciones();

            // Get all properties of the Aprobacion class
            PropertyInfo[] properties = typeof(Aprobaciones).GetProperties();

            // Iterate through each property and copy its value from source to copy
            foreach (PropertyInfo property in properties)
            {
                // Check if the property can be written to
                if (property.CanWrite)
                {
                    // Get the value of the property from source
                    object value = property.GetValue(source);

                    // Set the value of the property in copy
                    property.SetValue(copy, value);
                }
            }

            return copy;
        }

        private async Task<bool> NotifyApproval(EmailDetailCertificateDto emailDetailCertificateDto, string blobReference)
        {
            await emailCertificationService.SendAprobalProviderEmail(emailDetailCertificateDto, blobReference);
            return true;
        }

        /// <summary>
        /// MMSN-1021 - Chequea los estados de las ES seleccionadas, por si han sido cambiadas y no ha sido actualizado el listado
        /// </summary>
        /// <param name="ESList">Lista de entradas de servicio</param>
        /// <returns>OK - ES en pendiente de aprobación
        /// Modificado - ES en un estado distinto de pendiente de aprobación
        /// Vacia - Lista sin elementos - Error</returns>
        private string CheckESStatus(List<Aprobaciones> ESList)
        {
            string res = "OK";

            if (ESList.Count > 0)
            {
                foreach (Aprobaciones ap in ESList)
                {
                    if (ap.Estado_certificacion != "Pendiente Aprobación")
                    {
                        res = "Modificado -" + ap.Estado_certificacion;
                        break;
                    }
                }
            }
            else
            {
                res = "Vacia";
            }

            return res;
        }

        private DetalleOrdenDeCompraDto ObtenerDetalleOrdenDeCompra(string nroOC)
        {
            var centros = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Centro");
            var almacenes = repositorioEntradaServicio.Listar<TablaSap>(a => a.Tabla == "Almacen");

            var detalleOC = new ObtenerOrdenDeCompraConsumerMOA(repositorioEntradaServicio).ObtenerDetalleDeOrdenDeCompra(nroOC, centros, almacenes, true);

            var aprobaciones = repositorioEntradaServicio.Listar<Aprobaciones>(x => x.NRO_OC == nroOC && x.Estado_certificacion == "Pendiente Aprobación");

            foreach (var ap in aprobaciones)
            {
                var nroLinea = int.Parse(ap.Nro_linea);
                var nroPosicion = long.Parse(ap.NRO_POS);

                var es = new EntradaServicioDto
                {
                    TemporalId = ap.NRO_ES_LOCAL,
                    Cantidad = decimal.Parse(ap.Cantidad_a_certificar, CultureInfo.InvariantCulture),
                    itemNumero = ap.Planned_package,
                    ESS_LINE_NO = ap.Planned_line,
                    ESS_PCKG_NO = ap.Planned_package,
                    Fecha = ap.Fecha_Carga_ES.ToString(),
                    FechaContabilizacion = ((DateTime)ap.Fecha_Contabilizacion).ToString("yyyy-MM-dd"),
                    FechaDocumentoString = ((DateTime)ap.Fecha_Documento).ToString("dd/MM/yyyy"),
                    SePuedeBorrar = true,
                    Referencia = ap.Referencia,
                    Ingresante = ap.Ingresante_CDS,
                    IdES = ap.ID
                };

                var position = detalleOC.Posiciones.FirstOrDefault(x => x.NumeroPosicion == nroPosicion);

                if (position != null)
                {
                    var item = position.Items.FirstOrDefault(x => x.NumeroLinea == nroLinea);

                    if (item != null)
                    {
                        item.EntradasServicio = item.EntradasServicio ?? new List<EntradaServicioDto>();
                        item.EntradasServicio.Add(es);
                        item.CantidadReal += es.Cantidad;

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
            return detalleOC;
        }

        private EntradaServicioCreateParamsDto ObtenerDatosPosicionCertificar(PosicionDto posicionOC)
        {
            var crearESParamsDto = new EntradaServicioCreateParamsDto
            {
                EntrySheetHeader = new EntrySheetHeaderSection
                {
                    Descripcion = posicionOC.Descripcion,
                    OrdenCompraNumero = posicionOC.NroOrdenCompra,
                    OrdenCompraPosicionNumero = posicionOC.NumeroPosicion.ToString(),
                    DocumentoReferenciaNumero = "", // Corresponde al campo 'Referencia Remito' en la creación de la Certificación
                    FechaDocumento = DateTime.Today.ToString("yyyy-MM-dd"),
                    FechaContabilizacion = DateTime.Today.ToString("yyyy-MM-dd"),
                    MontoTotalACertificar = (posicionOC.PrecioTotal ?? 0).ToString(CultureInfo.InvariantCulture)
                },
                EntrySheetServices = new EntrySheetServiceSection { Items = new List<EntrySheetServiceItemSection>() }
            };

            foreach (var itemPosicion in posicionOC.Items)
            {
                if (!ItemTienePorcentajeACertificar(itemPosicion) || !ItemTieneMontoValidoACertificar(itemPosicion)) { continue; }

                var itemACertificar = new EntrySheetServiceItemSection
                {
                    ExternalLineNumber = itemPosicion.NumeroLinea.ToString().PadLeft(10, '0'),
                    Service = itemPosicion.ServicioNumero?.ToString() ?? "0",
                    Quantity = ((itemPosicion.Cantidad ?? 0) - (itemPosicion.CantidadReal ?? 0)).ToString(CultureInfo.InvariantCulture),
                    ItemQuantity = (itemPosicion.Cantidad ?? 0).ToString(CultureInfo.InvariantCulture),
                    GrossPrice = itemPosicion.PrecioBruto ?? 0,
                    ItemGrossPrice = itemPosicion.ImporteString,
                    CertificationAmount = itemPosicion.Monto.ToString(CultureInfo.InvariantCulture),
                    PlannedPackage = itemPosicion.Id,
                    PlannedLine = itemPosicion.LINE_NO,
                    Descripcion = itemPosicion.Descripcion,
                    ShortText = itemPosicion.Descripcion ?? posicionOC.Descripcion,
                    UM = itemPosicion.UM,
                    Percentage = CalcularPorcentajeACertificar(itemPosicion).ToString(CultureInfo.InvariantCulture)
                };
                crearESParamsDto.EntrySheetServices.Items.Add(itemACertificar);
            }

            return crearESParamsDto;
        }

        private bool ItemTienePorcentajeACertificar(ItemDto itemDto)
        {
            return
                decimal.TryParse(itemDto.Porcentaje, out decimal valorPorcentaje) &&
                valorPorcentaje < 100;
        }

        private bool ItemTieneMontoValidoACertificar(ItemDto itemDto)
        {
            var monto = itemDto.Importe ?? 0;
            var cantidad = itemDto.Cantidad ?? 0;
            var cantidadReal = itemDto.CantidadReal ?? 0;
            var cantidadACertificar = cantidad - cantidadReal;
            var montoACertificar = (cantidadACertificar * monto) / cantidad;

            return montoACertificar > 0;
        }

        private decimal CalcularPorcentajeACertificar(ItemDto itemDto)
        {
            var cantidadACertificar = (itemDto.Cantidad ?? 0) - (itemDto.CantidadReal ?? 0);
            var porcentajeACertificar = (cantidadACertificar * 100) / itemDto.Cantidad;
            return porcentajeACertificar ?? 0;
        }

        private void CrearEntradaServicioCertificacionAutomatica(EntradaServicioCreateParamsDto crearESParamsDto, Solp solpACertificar, List<MailAprobacionESRequest> solicitudesMailAprobacionES,
            ObtenerOrdenDeCompraConsumerMOA obtenerOrdenConsumer, List<TablaSap> centrosSap, List<TablaSap> almacenesSap, string proveedorCodigo = null)
        {
            Logger.Log.Info("EntradaServicioService.CrearEntradaServicioCertificacionAutomatica");

            var nroSolp = solpACertificar.NroSolp;
            var mailUsuarioSolicitante = solpACertificar.UsuarioCreacion.Mail;
            var mailResponsableTrabajo = solpACertificar.Pliego.Email;

            var crearCertificacionTemporal = !mailUsuarioSolicitante.Equals(mailResponsableTrabajo);

            if (crearCertificacionTemporal)
            {
                CrearEntradaServicioTemporal(crearESParamsDto, mailUsuarioSolicitante, new List<ReporteDto>(), new List<string>(), solicitudesMailAprobacionES, obtenerOrdenConsumer,
                    centrosSap, almacenesSap, nroSolp, proveedorCodigo);
            }
            else
            {
                var crearESResult = new CrearEntradaDeServicioConsumerMOA().CrearEntradaServicio(crearESParamsDto);

                if (crearESResult.Type == "I" && crearESResult.Id == "SE")
                {
                    var nroES = GetESNumber(crearESResult.Message);
                    GuardarDatosES(crearESParamsDto, mailUsuarioSolicitante, nroES, true, new List<ReporteDto>(), nroSolp, proveedorCodigo);
                }
                else
                {
                    throw new Exception(crearESResult.ToString());
                }
            }
        }
    }
}

