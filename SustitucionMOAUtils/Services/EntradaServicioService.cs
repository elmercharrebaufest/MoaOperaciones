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
using SustitucionMOAWS.ScatoComandosWebService;
using SustitucionMOAModel.Entities;
using System;
using System.CodeDom;
using Quartz.Util;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAUtils.Services.Email;
using System.IO;
using SustitucionMOAModel.Models.ViewModel;
using SustitucionMOAModel.Models.WSMapMOA;
using DocumentFormat.OpenXml.Office2010.Excel;
using SustitucionMOAUtils.Logger;

namespace SustitucionMOAUtils.Services

{
    public class EntradaServicioService : IEntradaServicioService
    {
        protected readonly IRepositorio repositorio;
        private readonly IConsultaService consultaService;
        private readonly OrderService orderService;
        private readonly IComprasService comprasService;
        private readonly IEmailCertificationService emailCertificationService;
        private readonly IEmailFasService emailFasService;
        //private readonly ILiquidacionService _liquidacionService;
        //private OrderParamsDto parametros;

        public EntradaServicioService(IConsultaService consultaService, IRepositorio repositorio, OrderService orderService, IComprasService comprasService, IEmailCertificationService emailCertificationService, IEmailFasService emailFasService)
        {
            this.repositorio = repositorio;
            this.consultaService = consultaService;
            this.orderService = orderService;
            this.comprasService = comprasService;
            this.emailCertificationService = emailCertificationService;
            this.emailFasService = emailFasService;
            //_liquidacionService = liquidacionService;
        }

        public async Task<List<EntradaServicioCabeceraDto>> ObtenerEntradasServicioCompleta(EntradaServicioParamsDto parametros, SustitucionMOAModel.Dto.UsuarioDto usuario)
        {
            List<EntradaServicioCabeceraDto> Documentos = await ServicioSAP_EntradasServicioCabecera(parametros, usuario);
            Documentos = OrdenarEntradasServicio(Documentos);

            return Documentos;
        }

        /// <summary>
        /// Realiza ordenamiento del objeto OrdenCompraDto según la columna y el tipo de orden especificados
        /// </summary>
        public List<EntradaServicioCabeceraDto> OrdenarEntradasServicio(List<EntradaServicioCabeceraDto> ordenes)
        {
            return ordenes.OrderByDescending(es => es.FechaCreacionDateTime).ThenByDescending(es => es.EntradaServicio).ToList();
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
        public async Task<List<EntradaServicioCabeceraDto>> ServicioSAP_EntradasServicioCabecera(EntradaServicioParamsDto parametros, SustitucionMOAModel.Dto.UsuarioDto usuario)
        {
            //Obtiene Cabeceras de Entradas de Servicio
            List<EntradaServicioCabeceraDto> EntradasServicioCabecera = await new ObtenerCabecerasEntradaServicioConsumerMOA().ObtenerEntradasServicioCabeceraAsync(parametros.FechaInicio);

            List<EntradaServicioCabeceraDto> EntradasServicio = new List<EntradaServicioCabeceraDto>();

            //Se filtran por las OC tomando las que empiezan con 412
            EntradasServicioCabecera = EntradasServicioCabecera.Where(x => x.OrdenCompra.StartsWith("412")).ToList();

            // Filtra por número de documento, si se proporciona el parámetro
            if (parametros.DocumentoNumero != null)
                EntradasServicioCabecera = EntradasServicioCabecera.Where(orden => orden.EntradaServicio.ToString() == parametros.DocumentoNumero).ToList();

            OrderParamsDto ordenParams = new OrderParamsDto();

            // ES APROBADAS
            try
            {
                foreach (var documento in EntradasServicioCabecera)
                {
                    string nroDoc = documento.EntradaServicio.ToString();
                    int nro_es_sap = int.Parse(nroDoc);

                    DateTime fecha = DateTime.Parse(documento.FechaCreacion);
                    string fechaFormateada = fecha.ToString("dd/MM/yyyy");
                    documento.FechaCreacion = fechaFormateada;

                    // Se obtiene detalle de una ES
                    List<EntradaServicioDetalleDto> entradasServicioDetalleSAP = new ObtenerEntradaDeServicioPorNumeroConsumerMOA().ObtenerEntradaServicioDetalle(nroDoc);
                    documento.entradaServicioDetalle = entradasServicioDetalleSAP;

                    documento.Estado = "Aprobada";

                    if (documento.entradaServicioDetalle != null && documento.entradaServicioDetalle.Count > 0)
                    {
                        ordenParams.OrdenCompraId = documento.entradaServicioDetalle[0].OrdenCompra;

                        Proveedor prov = orderService.BuscarProveedor(ordenParams);

                        documento.Proveedor = prov.RazonSocial ?? "-";

                        documento.CUIT = prov.CUIT ?? "-";
                    }

                    // Se obtiene detalle de la APROBACIÓN de la Entrada de Servicio
                    List<Aprobaciones> ESTemporales;

                    if (parametros.VerTodo)
                    {
                        if (usuario.Permisos.Contains("VER TODOS LOS ESTADOS DE ES"))
                        {
                            ESTemporales = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_SAP == nro_es_sap);
                        }
                        else
                        {
                            ESTemporales = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_SAP == nro_es_sap
                                && (x.Ingresante_CDS == usuario.Mail || x.Fiscal_SOLPED == usuario.Mail || x.Aprobador_CDS == usuario.Mail));
                        }
                    }
                    else
                    {
                        ESTemporales = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_SAP == nro_es_sap
                            && (x.Ingresante_CDS == usuario.Mail || x.Fiscal_SOLPED == usuario.Mail || x.Aprobador_CDS == usuario.Mail));
                    }



                    if (ESTemporales != null && ESTemporales.Count > 0)
                    {
                        Dictionary<string, Aprobaciones> detallesAprobacionPorLinea = ESTemporales.ToDictionary(detalle => detalle.Planned_line);

                        // Iterar sobre los detalles de la entrada de servicio
                        foreach (EntradaServicioDetalleDto detalleSAP in documento.entradaServicioDetalle)
                        {
                            // Verificar si hay detalles de aprobación correspondientes
                            if (detallesAprobacionPorLinea.TryGetValue(detalleSAP.NumeroLinea, out Aprobaciones detalle))
                            {
                                detalleSAP.NumeroLinea = detalle.Nro_linea;
                                detalleSAP.Descripcion = detalle.Descripcion_ES;
                                detalleSAP.TextoBreveServicio = detalle.Texto_breve_servicio;
                                detalleSAP.CantidadCertificar = detalle.Cantidad_a_certificar;
                                detalleSAP.PorcentajeCertificar = detalle.Porcentaje_a_certificar;
                                detalleSAP.MontoCertificar = detalle.Monto_a_certificar;
                                detalleSAP.NroRemito = detalle.Referencia;
                                detalleSAP.CodigoServicio = detalle.Nro_servicio;
                                documento.MotivoRechazo = detalle.Motivo_rechazo;
                                documento.NumeroCertificacion = detalle.NRO_ES_LOCAL;
                                documento.Ingresante = detalle.Ingresante_CDS;
                                documento.Aprobador = detalle.Aprobador_CDS;
                                documento.Suplente = detalle.Suplente;
                                documento.Fiscal = detalle.Fiscal_SOLPED;
                                documento.Descripcion = detalle.Texto_breve_servicio;
                                DateTime fechaAprobacionFormateada = (DateTime)detalle.Fecha_aprobacion;
                                documento.FechaAprobacion = fechaAprobacionFormateada.ToString("dd/MM/yyyy");
                            }
                        }

                        EntradasServicio.Add(documento);

                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }

            // Busqueda Entrada Servicios cargadas en la tabla aprobaciones.
            List<Aprobaciones> temporales = new List<Aprobaciones>();
            if (parametros.VerTodo && usuario.Permisos.Contains("VER TODOS LOS ESTADOS DE ES"))
            {
                temporales = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_SAP == null);
            } else {
                temporales = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_SAP == null && (x.Ingresante_CDS == usuario.Mail || x.Fiscal_SOLPED == usuario.Mail || x.Aprobador_CDS == usuario.Mail));
            }

            try
            {
                Dictionary<string, EntradaServicioCabeceraDto> diccionarioES = new Dictionary<string, EntradaServicioCabeceraDto>();

                foreach (Aprobaciones temporal in temporales)
                {
                    string nroEsLocal = temporal.NRO_ES_LOCAL;

                    if (!diccionarioES.ContainsKey(nroEsLocal))
                    {
                        ordenParams.OrdenCompraId = temporal.NRO_OC;
                        EntradaServicioCabeceraDto entradaServicioTemp = MapEntradaServicioCabecera(temporal, ordenParams);
                        diccionarioES.Add(nroEsLocal, entradaServicioTemp);
                    }

                    EntradaServicioDetalleDto detalleEntradaServicioTemp = MapEntradaServicioDetalle(temporal);
                    diccionarioES[nroEsLocal].entradaServicioDetalle.Add(detalleEntradaServicioTemp);
                }

                foreach (var kvp in diccionarioES)
                {
                    EntradasServicio.Add(kvp.Value);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return EntradasServicio;
        }

        /// <summary>
        /// Mapeo para cargar la cabecera de la ES
        /// </summary>
        /// <param name="temporal"></param>
        /// <param name="ordenParams"></param>
        /// <returns>entradaServicioTemp</returns>
        public EntradaServicioCabeceraDto MapEntradaServicioCabecera(Aprobaciones temporal, OrderParamsDto ordenParams)
        {
            DateTime fechaFormateada = (DateTime)temporal.Fecha_Carga_ES;

            EntradaServicioCabeceraDto entradaServicioTemp = new EntradaServicioCabeceraDto
            {
                ID = temporal.ID,
                OrdenCompra = temporal.NRO_OC,
                Descripcion = temporal.Texto_breve_servicio,
                MontoTotal = temporal.Monto_total.ToString(),
                FechaCreacion = fechaFormateada.ToString("dd/MM/yyyy"),
                FechaCreacionDateTime = temporal.Fecha_Carga_ES,
                EntradaServicio = temporal.NRO_ES_LOCAL,
                Estado = temporal.Estado_certificacion,
                MotivoRechazo = temporal.Motivo_rechazo,
                NumeroCertificacion = temporal.NRO_ES_LOCAL,
                Ingresante = temporal.Ingresante_CDS,
                Aprobador = temporal.Aprobador_CDS,
                Suplente = temporal.Suplente,
                Fiscal = temporal.Fiscal_SOLPED
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
                NumeroLinea = temporal.Nro_linea,
                UM = temporal.UM,
                Descripcion = temporal.Descripcion_ES,
                TextoBreveServicio = temporal.Texto_breve_servicio,
                Monto = Convert.ToDecimal(temporal.Monto),
                CantidadCertificar = temporal.Cantidad_a_certificar,
                PorcentajeCertificar = temporal.Porcentaje_a_certificar,
                MontoCertificar = temporal.Monto_a_certificar,
                NroRemito = temporal.Referencia,
                CodigoServicio = temporal.Nro_servicio,
                FechaPrestacion = temporal.Fecha_Documento?.ToString("MM/dd/yyyy")
            };

            return detalleEntradaServicioTemp;
        }

        /// <summary>
        /// Borrar Entrada de Servicio indicando su número de documento
        /// Actualmente hay varias incognicas con respecto a las condiciones que debe cumplir una ES para poder ser borrada
        /// </summary>
        /// <returns></returns>
        public string BorrarEntradaServicio(EntradaServicioParamsDto parametros)
        {
            string result = "";
            if (parametros.DocumentoNumero.Contains("\""))
            {
                parametros.DocumentoNumero = parametros.DocumentoNumero.Replace("\\", "").Replace("\"", "");
            }
            if (parametros.DocumentoNumero.Contains("T_"))
            {
                //Temporal - hard delete
                List<Aprobaciones> apToDelete = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == parametros.DocumentoNumero);
                foreach (Aprobaciones ap in apToDelete)
                {
                    repositorio.Remover<Aprobaciones>(ap);
                }
                repositorio.GuardarCambios();
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


        public string CrearEntradaServicioAnt(EntradaServicioCreateParamsDto parametros)
        {
            string result = new CrearEntradaDeServicioConsumerMOA().CrearEntradaServicio(parametros);


            return result;
        }

        /// <summary>
        /// MMSN-601: Validación de información ingresante c/ SolPed
        /// </summary>
        /// <param name="parametros"></param>
        /// <param name="userMail"></param>
        /// <returns></returns>
        public EntradaServicioCreateRespuestaDto ValidarIngresante(EntradaServicioCreateParamsDto parametros, string userMail)
        {
            //MMSN-601 agregar lógica entrada servicio automatica- temporal, nro solped en parametros.Header.Solp
            //1 - Obtener información asociada a SolPed
            SolpESDto detalleSolPed = new SolpESDto();
            try
            {
                if (parametros.EntrySheetHeader.SolPedNumber != null)
                {
                    string solPedNumber = parametros.EntrySheetHeader.SolPedNumber;
                    detalleSolPed = comprasService.TraerSolpPorNumero(solPedNumber);
                }
            }
            catch (Exception e)
            {
                EntradaServicioCreateRespuestaDto response = new EntradaServicioCreateRespuestaDto();
                response.Type = "S";
                response.Message = e.Message;
                return response;
            }

            //2 - Comparar datos SolPed para certificar automaticamente o WKF de aprobaciones
            bool auto = false;
            bool difSolicitante = false;

            //2a - Comparar Fiscal/Email con usuario FE
            if (userMail == detalleSolPed.FiscalContrato || userMail == detalleSolPed.Email)
            {
                auto = true;
            }
            else if (detalleSolPed.SupervisorTrabajo.Count > 0 && userMail == detalleSolPed.SupervisorTrabajo[0])
            {
                //2b - Si el supervisor del trabajo es el mismo que el usuario ingresante
                auto = true;
            }
            else if (detalleSolPed.Posiciones.Count > 0)
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
                            var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.UsuarioSap == solicitante.ToUpper());
                            if (usuario != null && usuario.Mail == userMail)
                            {
                                auto = true;
                                difSolicitante = false;
                            }
                            else if(usuario != null && usuario.Mail != userMail)
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
            //Aca - Si los 3 datos estan vacios o no vienen -> “No se encuentra fiscal en la Sol. Ped. Ingresada. Por favor, verificar con el creador de la misma”. 
            if (auto == false)
            {
                bool empty = EmptySolPedValues(detalleSolPed);
                if (empty || difSolicitante)
                {
                    EntradaServicioCreateRespuestaDto emptySolPed = new EntradaServicioCreateRespuestaDto();
                    emptySolPed.Type = "S";
                    emptySolPed.Message = "No se encuentra fiscal en la Sol. Ped. Ingresada. Por favor, verificar con el creador de la misma";
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

        public async Task<EntradaServicioCreateRespuestaDto> CrearEntradaServicio(EntradaServicioCreateParamsDto parametros, string userMail)
        {

            // 3 - Si alguna de las validaciones es correcta, alta automatica.
            EntradaServicioCreateRespuestaDto result = await new CrearEntradaDeServicioConsumerMOA().CrearEntradaServicioAsync(parametros);

            ////MMSN-602 - Cargar en tabla aprobaciones si se creo la ES.
            if (result.Type == "I" && result.Id == "SE")
            {
                int ESNumber = GetESNumber(result.Message);
                Aprobaciones ap = GuardarDatosES(parametros, userMail, ESNumber, true);
            }


            return result;
        }

        public EntradaServicioCreateRespuestaDto CrearEntradaServicioTemporal(EntradaServicioCreateParamsDto parametros, string userMail)
        {


            EntradaServicioCreateRespuestaDto result = new EntradaServicioCreateRespuestaDto();
            try
            {
                Aprobaciones ap = GuardarDatosES(parametros, userMail, 0, false);
                result.Type = "S";

                result.Message = $"Se generó la entrada de servicio {ap.NRO_ES_LOCAL} en estado {ap.Estado_certificacion}, a verificar por Contratante o Solicitante.";

                //MMSN - 605
                if (ap != null)
                {
                    //Todos los registros con mismo NRO_ES_LOCAL
                    List<Aprobaciones> completeAp = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == ap.NRO_ES_LOCAL);
                    //Buscar Proveedor
                    OrderParamsDto orderParams = new OrderParamsDto();
                    orderParams.OrdenCompraId = ap.NRO_OC;
                    Proveedor prov = new Proveedor();
                    prov = orderService.BuscarProveedor(orderParams);

                    _= NotifyCreation(completeAp, prov);
                    //emailCertificationService.EnviarMailAprobacion(completeAp, prov);

                    //MMSN-1010
                    if(completeAp.Count > 0)
                    {
                        foreach (Aprobaciones aprobacion in completeAp)
                        {
                            aprobacion.Notificaciones_enviadas = true;
                        }
                        repositorio.GuardarCambios();
                    }

                }
            }
            catch (Exception e)
            {
                result.Type = "E";
                result.Message = e.Message;
            }


            return result;
        }

        private async Task<bool> NotifyCreation(List<Aprobaciones> completeAp, Proveedor prov)
        {
           await emailCertificationService.EnviarMailAprobacion(completeAp, prov);
           return true;
        }


        /// <summary>
        /// MMSN-601: Metodo para validar si los valores de detalleSolPed estan vacios 
        /// </summary>
        /// <returns></returns>
        private bool EmptySolPedValues(SolpESDto detalleSolPed)
        {
            bool result = false;
            if (detalleSolPed.FiscalContrato.IsNullOrWhiteSpace())
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
        /// <param name="message"></param>
        /// <returns></returns>
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
        /// <param name="parametros"></param>
        /// <param name="userMail"></param>
        private Aprobaciones GuardarDatosES(EntradaServicioCreateParamsDto parametros, string userMail, int ESNumber, bool auto)
        {
            Aprobaciones temp = new Aprobaciones();

            #region CargaDatosCabecera
            DateTime dateDocument;

            //MMSN-991
            if (String.IsNullOrEmpty(parametros.EntrySheetHeader.FechaDocumento))
            {
                parametros.EntrySheetHeader.FechaDocumento = DateTime.Today.ToString("yyyy-MM-dd");
            }

            if (DateTime.TryParseExact(parametros.EntrySheetHeader.FechaDocumento, "yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None, out dateDocument))
            {
                temp.Fecha_Documento = dateDocument;
            }
            DateTime dateAccounting;
            if (DateTime.TryParseExact(parametros.EntrySheetHeader.FechaContabilizacion, "yyyy-MM-dd",
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None, out dateAccounting))
            {
                temp.Fecha_Contabilizacion = dateAccounting;
            }

            temp.Referencia = parametros.EntrySheetHeader.DocumentoReferenciaNumero;
            temp.Fecha_Carga_ES = DateTime.Today;
            temp.Notificaciones_enviadas = false;
            temp.Ingresante_CDS = userMail;

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
                string solPedNumber = parametros.EntrySheetHeader.SolPedNumber;
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
                    if (!string.IsNullOrEmpty(detalleSolPed.FiscalContrato))
                    {
                        if (detalleSolPed.FiscalContrato.Contains("@"))
                        {
                            temp.Fiscal_SOLPED = detalleSolPed.FiscalContrato;
                            var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.Mail == detalleSolPed.FiscalContrato);
                            if (usuario != null)
                            {
                                temp.Suplente = usuario.Suplente;
                            }
                        }
                        else
                        {
                            string fiscal = detalleSolPed.FiscalContrato.Replace(" ", "");
                                fiscal = fiscal.ToUpper();
                                var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.UsuarioSap == fiscal);
                                if (usuario != null)
                            {
                                temp.Fiscal_SOLPED = usuario.Mail;
                                temp.Suplente = usuario.Suplente;
                            }
                        }
                    }
                    else if (detalleSolPed.SupervisorTrabajo != null && (detalleSolPed.SupervisorTrabajo.Count > 0 && !string.IsNullOrEmpty(detalleSolPed.SupervisorTrabajo[0])))
                    {
                        //Busqueda por Supervisor Trabajo
                        if (detalleSolPed.SupervisorTrabajo[0].Contains("@"))
                        {
                            temp.Fiscal_SOLPED = detalleSolPed.SupervisorTrabajo[0];
                                var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.Mail == temp.Fiscal_SOLPED);
                                if (usuario != null)
                            {
                                temp.Suplente = usuario.Suplente;
                            }
                        }
                        else
                        {
                            string fiscal = detalleSolPed.SupervisorTrabajo[0].Replace(" ", "");
                                fiscal = fiscal.ToUpper();
                                var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.UsuarioSap == fiscal);
                                if (usuario != null)
                            {
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
                                    var usuario = repositorio.Obtener<SustitucionMOAModel.Entities.Usuario>(x => x.UsuarioSap == solicitante);
                                    if (usuario != null)
                                {
                                    temp.Fiscal_SOLPED = usuario.Mail;
                                    temp.Suplente = usuario.Suplente;
                                    // Pendiente Carga temp.Area, ya que se necesitan los datos de MMSN-726
                                    /*
                                     * Tentativo - Falta Testear y posible optimización
                                     *                                     * 
                                     * var idArea = repositorio.Obtener<Usuario_Area>(x => x.Usuario_ID == usuario.Id);
                                     * if(idArea != null){
                                     *  var Area = repositorio.Obtener<Area>(x => x.ID_Area == idArea.Area_ID);
                                     *  if(Area != null) temp.Area_Fiscal = Area.NombreArea
                                     *  
                                     * }
                                     *   
                                     */
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
            catch(Exception e)
            {
                Logger.Log.Info(e.Message);
            }
            #endregion



            //Datos OC
            temp.NRO_OC = parametros.EntrySheetHeader.OrdenCompraNumero;
            temp.NRO_POS = parametros.EntrySheetHeader.OrdenCompraPosicionNumero;
            //Monto Total
            if (!string.IsNullOrEmpty(parametros.EntrySheetHeader.MontoTotalACertificar))
            {
                temp.Monto = double.Parse(parametros.EntrySheetHeader.MontoTotalACertificar, System.Globalization.CultureInfo.InvariantCulture);
            }

            //Obtener último registro para nuevo número
            Aprobaciones ultimoRegistro = new Aprobaciones();
            try
            {
                ultimoRegistro = repositorio.Listar<Aprobaciones>().LastOrDefault();
            }
            catch (Exception e)
            {
                throw e;
            }


            if (ultimoRegistro == null || String.IsNullOrEmpty(ultimoRegistro.NRO_ES_LOCAL))
            {
                //Primer registro en tabla
                temp.NRO_ES_LOCAL = "T_0000000001";
            }
            else
            {
                string nroLocal = ultimoRegistro.NRO_ES_LOCAL.Substring(2);

                int number = int.Parse(nroLocal);

                number++;

                string newESLocal = "T_" + number.ToString("D10");

                temp.NRO_ES_LOCAL = newESLocal;

            }

            //MontoTotal = Suma de los montos a certificar de cada ES A APROBAR
            double monto_total = 0;
            List<Aprobaciones> toSave = new List<Aprobaciones>();

            #region CargaDeDatosPorItem
            //Datos por Item en ES
            foreach (EntrySheetServiceItemSection esItem in parametros.EntrySheetServices.Items)
            {
                temp.Descripcion_ES = esItem.Descripcion;

                Aprobaciones aprobacion = new Aprobaciones();

                //Copiar lo cargado hasta ahora
                aprobacion = DeepCopy(temp);
                try
                {
                    if (!string.IsNullOrEmpty(esItem.ItemQuantity)) aprobacion.Cantidad = int.Parse(esItem.ItemQuantity, System.Globalization.CultureInfo.InvariantCulture);
                    if (!string.IsNullOrEmpty(esItem.ItemGrossPrice)) aprobacion.Monto = double.Parse(esItem.ItemGrossPrice, System.Globalization.CultureInfo.InvariantCulture);

                    aprobacion.Nro_linea = esItem.ExternalLineNumber;
                    aprobacion.Nro_servicio = esItem.Service;
                    aprobacion.Texto_breve_servicio = esItem.ShortText;
                    aprobacion.UM = esItem.UM;
                    aprobacion.Cantidad_a_certificar = esItem.Quantity;
                    aprobacion.Porcentaje_a_certificar = esItem.Percentage;
                    aprobacion.Planned_package = esItem.PlannedPackage;
                    aprobacion.Planned_line = esItem.PlannedLine;

                    if (!string.IsNullOrEmpty(esItem.CertificationAmount))
                    {
                        aprobacion.Monto_a_certificar = double.Parse(esItem.CertificationAmount, System.Globalization.CultureInfo.InvariantCulture);
                        monto_total = (double)(monto_total + aprobacion.Monto_a_certificar);
                    }
                }
                catch (Exception e)
                {
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
                    repositorio.Agregar<Aprobaciones>(ap);
                }
                //Transaccion
                try
                {
                    repositorio.GuardarCambios();
                }
                catch (Exception e)
                {
                    throw e;
                }


            }

            //Para mensaje de retorno de ES Temporal (sin aprobación automatica) se necesita mostrar datos de NRO_ES_LOCAL y estado.
            return temp;

        }

        /// <summary>
        /// MMSN-602 - Copiar objeto sin sobreescribir datos
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        Aprobaciones DeepCopy(Aprobaciones source)
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

        /// <summary>
        /// Aprobación de las ES en estado Pendiente Aprobación
        /// Se carga la EntrySheetHeader y el EntrySheetServices para su correcta aprobación
        /// </summary>
        /// <param name="nro_es_local"></param>
        /// <returns></returns>
        public async Task<EntradaServicioCreateRespuestaDto> AprobarEntradaDeServicio(string nro_es_local)
        {
            List<Aprobaciones> EntradasDeServicioTemp = repositorio.Listar<SustitucionMOAModel.Entities.Aprobaciones>(x => x.NRO_ES_LOCAL == nro_es_local);
            EntradaServicioCreateRespuestaDto result = new EntradaServicioCreateRespuestaDto();
            string status = CheckESStatus(EntradasDeServicioTemp);
            if(status.Contains("Modificado"))
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

                if(originalFC.HasValue &&
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

                    emailDetailCertificateDto.Descripcion = EntradasDeServicioTemp[0].Descripcion_ES;
                    emailDetailCertificateDto.FechaCertificacion = EntradasDeServicioTemp[0].Fecha_Contabilizacion?.ToString("yyyy-MM-dd");
                    emailDetailCertificateDto.Destinatario = EntradasDeServicioTemp[0].Ingresante_CDS;
                    emailDetailCertificateDto.Proveedor = "";
                    emailDetailCertificateDto.Importe = EntradasDeServicioTemp[0].Importe.ToString();
                    emailDetailCertificateDto.MontoTotal = EntradasDeServicioTemp[0].Monto_total.ToString();

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
                                GrossPrice = (decimal)DividirConDosDecimales((double)ES.Monto, (int)ES.Cantidad),
                                PlannedPackage = ES.Planned_package,
                                PlannedLine = ES.Planned_line
                            };
                            EntradaServicioSapParams.EntrySheetServices.Items.Add(item);
                        }

                        serviceDetailDto.UM = ES.UM;
                        serviceDetailDto.Descripcion = ES.Descripcion_ES;
                        serviceDetailDto.Porcetaje = ES.Porcentaje_a_certificar;
                        serviceDetailDto.Cantidad = ES.Cantidad.ToString();
                        serviceDetailDto.Monto = ES.Monto.ToString();


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
                    throw e;
                }

                if (result.Type == "I" && result.Id == "SE")
                {
                    int ESNumber = GetESNumber(result.Message);
                    emailDetailCertificateDto.NumeroCertificacion = ESNumber.ToString();
                    foreach (var ES in EntradasDeServicioTemp)
                    {
                        if (ES.Estado_certificacion == "Pendiente Aprobación")
                        {
                            try
                            {
                                ES.NRO_ES_SAP = ESNumber;
                                ES.Estado_certificacion = "Aprobada";
                                ES.Fecha_aprobacion = DateTime.Today;
                                repositorio.GuardarCambios();
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                    }
                } else
                {
                    emailDetailCertificateDto.NumeroCertificacion = EntradasDeServicioTemp[0].NRO_ES_LOCAL;
                }
            }

            await emailCertificationService.SendAprobalProviderEmail(emailDetailCertificateDto);

            return result;
        }

        /// <summary>
        /// Rechazo de ES temporal, Se notifica al usuario por correo
        /// </summary>
        /// <param name="rechazo"></param>
        /// <returns></returns>
        public EntradaServicioRejectRespuestaDto RechazarEntradaDeServicio(EmailDetailCertificateDto rechazo)
        {
            List<Aprobaciones> EntradasDeServicioTemp = repositorio.Listar<SustitucionMOAModel.Entities.Aprobaciones>(x => x.NRO_ES_LOCAL == rechazo.NumeroCertificacion);
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
                        repositorio.GuardarCambios();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    try
                    {
                        rechazo.Importe = ES.Importe.ToString();
                        rechazo.GeneradoPor = ES.Aprobador_CDS;
                        _ = NotifyRejection(rechazo);
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }
            ret.result = EntradasDeServicioTemp;

            return ret;
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

            if(ESList.Count > 0)
            {
                foreach(Aprobaciones ap in ESList)
                {
                    if(ap.Estado_certificacion != "Pendiente Aprobación")
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

        public async Task<bool> NotifyRejection(EmailDetailCertificateDto emailDetailCertificateDto)
        {
            bool response = false;
            var aprobaciones = repositorio.Obtener<Aprobaciones>(a => a.NRO_ES_LOCAL == emailDetailCertificateDto.NumeroCertificacion);

            if (emailDetailCertificateDto != null && !string.IsNullOrEmpty(emailDetailCertificateDto.Destinatario))
            {
                emailDetailCertificateDto.GeneradoPor = aprobaciones.Aprobador_CDS;

                await emailCertificationService.SendNotifyRejectionEmail(emailDetailCertificateDto);

                if (aprobaciones.Notificaciones_enviadas == false)
                {
                    aprobaciones.Notificaciones_enviadas = true;

                    repositorio.GuardarCambios();
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
                toReturn = repositorio.Listar<Aprobaciones>(x => x.NRO_ES_LOCAL == nroESLocal).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }


            return toReturn;
        }


        /// <summary>
        /// Reasigna el suplente del usuario fiscal a la CES como Aprobador
        /// Reasigna al fiscal de nuevo como Aprobador
        /// Solo el fiscal puede reasignar.
        /// </summary>
        /// <param name="nro_es_local"></param>
        /// <param name="suplente"></param>
        /// <returns></returns>
        public string ReasignarSuplente(string nro_es_local, string suplente) {
            try
            {
                Aprobaciones esTemporalPendienteAprobacion = repositorio.Obtener<Aprobaciones>(t => t.NRO_ES_LOCAL == nro_es_local);
                if (esTemporalPendienteAprobacion != null)
                {
                    if (suplente == esTemporalPendienteAprobacion.Fiscal_SOLPED)
                    {
                        esTemporalPendienteAprobacion.Suplente = esTemporalPendienteAprobacion.Aprobador_CDS;
                        esTemporalPendienteAprobacion.Aprobador_CDS = esTemporalPendienteAprobacion.Fiscal_SOLPED;
                    } else
                    {
                        esTemporalPendienteAprobacion.Suplente = esTemporalPendienteAprobacion.Fiscal_SOLPED;
                        esTemporalPendienteAprobacion.Aprobador_CDS = suplente;
                    }
                    repositorio.GuardarCambios();
                } else
                {
                    return "Nro de entrada servicio no encontrado.";
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "Se reasigno el suplente de la Entrada de servicio éxitosamente.";
        }

        public string ActualizarInformacionIngresante(IngresanteInfoEditableDto info)
        {
            try
            {
                Aprobaciones ESTemporal = repositorio.Obtener<Aprobaciones>(t => t.ID == info.ID);
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
                    } else if (info.ColumnaEditar == "DescripcionES")
                    {
                        ESTemporal.Descripcion_ES = info.NuevoValor;
                    } else if (info.ColumnaEditar == "Remito")
                    {
                        ESTemporal.Referencia = info.NuevoValor;
                    }
                    repositorio.GuardarCambios();
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
    }
}

