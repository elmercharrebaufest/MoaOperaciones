using SustitucionMOAFotmatter;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerSolpWebServiceMOA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerSolpConsumerMOA : IObtenerSolpConsumerMOA
    {
        private readonly SI_MMRFC_OBTENER_SOLPEDClient service;
        private const string COMP_CODE = "MOA";

        public ObtenerSolpConsumerMOA()
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_SOLPED&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_OBTENER_SOLPEDClient(SAPCredential.CrearSapLongBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public ObtenerSolpSAPResponse RequestSolpWithNroAndDates(ObtenerSolpRequest req)
        {
            try
            {
                //Fecha Solicitud Fin
                //IM_PREQ_DATE_F: Actúa como filtro de Fecha de SOLPED (desde....)
                // la fecha fin es desde????????????????????????????
                string IM_PREQ_DATE_F = SAPFormatter.PrepararFecha(req.FechaHasta);

                //Fecha Solicitud Inicio
                //IM_PREQ_DATE_I: Actúa como filtro de Fecha de SOLPED (hasta....)
                string IM_PREQ_DATE_I = SAPFormatter.PrepararFecha(req.FechaDesde);

                //Numero de SOLPED
                //IM_PREQ_NO: Permite buscar 1 solo número de SOLPED a la vez. Sino se introduce el campo, el sistema devuevle todo lo existente
                //            en el periodo de tiempo ingresado en IM_PREQ_DATE_I y IM_PREQ_DATE_F.
                string IM_PREQ_NO = req.NumeroSolp;


                string IM_SERVICES = "X";
                string IM_ACCOUNT_ASSIGNMENT = "X";
                string IM_DELIVERY_ADDRESS = "X";
                string IM_ITEM_TEXT = "X";
                string IM_HEADER_TEXT = "X";

                ZMPES5640[] IM_USUARIOS = new ZMPES5640[0];

                //200 exito - 400 error
                var result = service.SI_MMRFC_OBTENER_SOLPED(
                            IM_ACCOUNT_ASSIGNMENT,
                            "",
                            "",
                            "",
                            IM_DELIVERY_ADDRESS,
                            IM_HEADER_TEXT,
                            "",
                            IM_ITEM_TEXT,
                            "",
                            IM_PREQ_DATE_F,
                            IM_PREQ_DATE_I,
                            IM_PREQ_NO,
                            "",
                            IM_SERVICES,
                            IM_USUARIOS,
                            out string EX_EXITO,
                            out ZMPES5740[] EX_PRACCOUNT,
                            out ZMPES7110[] EX_PRADDRDELIVERY,
                            out ZMPES7120[] EX_PRCOMPONENTS,
                            out ZMPES7140[] EX_PRHEADERTEXT,
                            out ZMPES5670[] EX_PRITEM,
                            out ZMPES7130[] EX_PRIMETEXT,
                            out BAPIRETURN[] EX_RETURN,
                            out ZMPES5770[] EX_SERVICEACCOUNT,
                            out ZMPES5730[] EX_SERVICELINES);

                /*  •	Datos a nivel posición de SOLPED (EX_PRITEM)
                    •	Datos de dirección de la posición de la SOLPED (EX_PRADDRDELIVERY)
                    •	Datos de imputación a nivel posición de la SOLPED (EX_PRACCOUNT)
                    •	Datos de suposiciones de Servicios (EX_SERVICELINES). Esto se da cuando EX_PRITEM-ITEM_CAT = "9"
                    •	Datos de imputación a nivel suposiciones (EX_SERVICEACCOUNT). Involucra solo porcentajes y montos.
                    •	Mensajes del WS, procesados por SAP (EX_RETURN)
                    •	Variable de status de ws (EX_EXITO)
                */
                var archivos = result.ToList() ?? new List<ZMPES7100>();
                return Map(EX_PRACCOUNT, EX_PRADDRDELIVERY, EX_PRCOMPONENTS, EX_PRITEM, EX_RETURN, EX_SERVICEACCOUNT, EX_SERVICELINES, archivos, EX_PRHEADERTEXT);

            }
            catch (Exception e)
            {
                throw;
            }
        }

        private ObtenerSolpSAPResponse Map(ZMPES5740[] tipoImputaciones, //EX_PRACCOUNT
                                          ZMPES7110[] direccionesPosicion, //EX_PRADDRDELIVERY
                                          ZMPES7120[] eX_PRCOMPONENTS, //eX_PRCOMPONENTS
                                          ZMPES5670[] posiciones, //EX_PRITEM
                                          BAPIRETURN[] mensajes, //EX_RETURN
                                          ZMPES5770[] imputacionesSuposiciones, //EX_SERVICEACCOUNT
                                          ZMPES5730[] suposicionesServicios,
                                          List<ZMPES7100> archivos,
                                          ZMPES7140[] textosCabecera) //EX_PRHEADERTEXT
        {
            var result = new ObtenerSolpSAPResponse();

            if (mensajes != null)
            {
                if (mensajes.Length > 0)
                {
                    result.Error = new ErrorObtenerSOLP
                    {
                        Codigo = mensajes[0].CODE,
                        Mensaje = mensajes[0].MESSAGE,
                        rTipo = mensajes[0].TYPE
                    };
                }
            }

            /*
             PREQ_NO: número de Solicitud. 
                PREQ_ITEM: Numero de Posición
                SERIAL_NO: Dato de manejo interno del sistema y se usa para relaciones entre tablas.
                DELETE_IND: Inidca si la posición de imputación está o no activa. 
                QUANTITY: indican las cantidades imputadas a nivel posición al objeto de costo.
                DISTR_PERC: Porcentaje de distribución.
                NET_VALUE: Precio imputado a la posición.
                GL_ACCOUNT: Cuenta contable imputada la posición de la SOLPED.
                BUS_AREA: División de la empresa sobre la cual imputa la posición.
                COSTCENTER: Centro de costo
                ASSET_NO: Numero de Activo Fijo sobre la cual imputa.
                SUB_NUMBER: Sub numero de Activo fijo 
                ORDERID: Orden de inverisión o mantenimiento sobre la cual imputa.
                CO_AREA: Organización de CO.
                PROFIT_CTR: Centro de beneficio.

                Todos estos objetos van a venir completos segun el tipo de imputación. Por ejemplo, si la imputación es del tipo (EX_PREITEM-ACCTASSCAT) = "K", la tabla va a pasar como parámetro el campo COSTCENTER. 
                Resto de campos solo a nivel informativo.
             */


            result.ObservacionesGeneracion = textosCabecera.Length > 0 ? textosCabecera.Where(x => x.TEXT_ID == "B01").FirstOrDefault().TEXT_LINE : "";


            result.Archivos = archivos.Select(x => new ArchivoSolpDto
            {
                DocId = x.DOC_ID,
                Nombre = x.OBJ_DESCR,
                Tipo = x.OBJ_TYPE
            }).ToList();

            if (result.TipoImputaciones == null)
            {
                result.TipoImputaciones = new List<TipoImputacionSAP>();
            }
            foreach (var tipoImputacion in tipoImputaciones)
            {
                result.TipoImputaciones.Add(new TipoImputacionSAP
                {
                    NumeroSolicitud = tipoImputacion.PREQ_NO,
                    NumeroPosicion = tipoImputacion.PREQ_ITEM,
                    NumeroDeSerie = tipoImputacion.SERIAL_NO,
                    ImputacionActiva = tipoImputacion.DELETE_IND,
                    CantidadesImputadas = tipoImputacion.QUANTITY,
                    CantidadesImputadasString = SAPFormatter.FormatearCantidad(tipoImputacion.QUANTITY, ""),
                    PorcentajeDistribucion = tipoImputacion.DISTR_PERC,
                    PorcentajeDistribucionString = SAPFormatter.FormatearPorcentaje(tipoImputacion.DISTR_PERC),
                    PrecioNetoImputado = tipoImputacion.NET_VALUE,
                    PrecioNetoImputadoString = SAPFormatter.FormatearMonto(tipoImputacion.NET_VALUE, "ARP"),
                    CuentaContableImputada = tipoImputacion.GL_ACCOUNT,
                    DivisionImputada = tipoImputacion.BUS_AREA,
                    CentroDeCosto = tipoImputacion.COSTCENTER,
                    NumeroActivoFijo = tipoImputacion.SUB_NUMBER,
                    IdOrden = tipoImputacion.ORDERID,
                    COArea = tipoImputacion.CO_AREA,
                    CentroDeBeneficio = tipoImputacion.PROFIT_CTR,
                    NumeroOrdenDeCompra = tipoImputacion.UNLOAD_PT
                });
            }

            /*
             * 
                PREQ_NO: número de Solicitud. 
                PREQ_ITEM: Numero de Posición
                NAME: Nombre de la ubicación. Por lo general para la implementación se corresponde con el nombre del centro logistico.
                POSTL_COD1: Código Postal
                CITY: Ciudad
                STREET: Calle
                STREET_NO: Número de dirección
                TEL1_NUMBR: Telefono.

            */

            if (result.Direcciones == null)
            {
                result.Direcciones = new List<DireccionSolpSAP>();
            }
            foreach (var direccionPosicion in direccionesPosicion)
            {
                result.Direcciones.Add(new DireccionSolpSAP
                {
                    NumeroSolicitud = direccionPosicion.PREQ_NO,
                    NumeroPosicion = direccionPosicion.PREQ_ITEM,
                    NombreUbicacion = direccionPosicion.NAME,
                    CodigoPostal = direccionPosicion.POSTL_COD1,
                    Ciudad = direccionPosicion.CITY,
                    Calle = direccionPosicion.STREET,
                    Numero = direccionPosicion.HOUSE_NO,
                    Telefono = direccionPosicion.TEL1_NUMBR,
                });
            }

            /*
             *  PREQ_NO: número de Solicitud. 
                PREQ_ITEM: Numero de Posición
                DOC_TYPE: Clase de Documento
                DELETE_IND: Indica si la posición está borrada o no. Si es igual a espacio en blanco, está activa.
                PCKG_NO: Este campo es utilizado para enlazar la posición con las suposiciones de servicios.
                CREATE_IND: indica desde donde fue creada una solepd. Si la misma se creo desde F = Orden, la misma no se puede editar desde la parte WEB.
                REL_IND: indicador de liberación, si la misma es igual a 2 = Liberada.
                PROC_STAT: Indica status de la SOLPED
                PUR_GROUP: Grupo de compras
                CREATED_BY: usuario que creo la solicitud
                PREQ_NAME: nombre del solicitante
                SHORT_TEXT: Texto de la posición.  Si la misma tiene Código de Material, el texto no se debiera poder modificar.
                MATERIAL: código de la material
                PLANT: Centro logistico
                STORE_LOC: Almacén
                TRACKINGNO: Número de requerimiento interno de la compañia.
                MATL_GROUP: Grupo de artículo
                QUANTITY: Cantidad
                UNIT: Unidad de medida 
                PREQ_DATE: Fecha Solicitud
                DELIV_DATE: Fecha entrega
                REL_DATE: Fecha estimada liberación
                GR_PR_TIME
                PREQ_PRICE: Precio de la solped
                PRICE_UNIT: Moneda
                ITEM_CAT: Indica el tipo de Posición. Ojo, la posición 9 en la pantalla de SAP se representa como una "F", pero internamente como un "9".
                ACCTASSCAT: Tipo de imputación:
                DES_VENDOR: Proveedor deseado (código de proveedor)
                FIXED_VEND: Proveedor fijo (código de proveedor)
                PURCH_ORG: Organización de compras
                AGREEMENT: código o numero de contrato Marco
                AGMT_ITEM: posición del contrato marco.
                PO_NUMBER: número del Pedido de compras asignado. Esto se da cuando la solped se transforma en Pedido.
                PO_ITEM: Posición del Pedido de Compras.
                PO_DATE: Fecha del pedido de compras
                CLOSED: Posición concluida. Indica que la posición fue satisfecha. 
                CURRENCY: Moneda
                PLND_DELRY: cantidad de días para la entrega.
                REQ_BLOCKED: indica que la Posición bloqueada.

            */

            result.Posiciones = new List<PosicionSolpSAP>();
            foreach (var posicion in posiciones)
            {
                result.Posiciones.Add(new PosicionSolpSAP
                {
                    NumeroSolicitud = posicion.PREQ_NO,
                    NumeroPosicion = posicion.PREQ_ITEM,
                    TipoDocumento = posicion.DOC_TYPE,
                    ImputacionActiva = posicion.DELETE_IND,
                    NumeroPaquete = posicion.PCKG_NO,
                    OrigenCreacion = posicion.CREATE_IND,
                    IndicadorDeLiberacion = posicion.REL_IND,
                    EstadoSolp = posicion.PROC_STAT,
                    GrupoCompras = posicion.PUR_GROUP,
                    UsuarioCreado = posicion.CREATED_BY,
                    NombreSolicitante = posicion.PREQ_NAME,
                    TextoPosicion = posicion.SHORT_TEXT,
                    Material = posicion.MATERIAL,
                    CentroLogistico = posicion.PLANT,
                    Almacen = posicion.STORE_LOC,
                    NumeroRequerimientoInterno = posicion.TRACKINGNO,
                    GrupoArticulo = posicion.MATL_GROUP,
                    Cantidad = posicion.QUANTITY,
                    UnidadMedida = posicion.UNIT,
                    CantidadString = SAPFormatter.FormatearCantidad(posicion.QUANTITY, posicion.UNIT),
                    FechaSolicitud = SAPFormatter.FormatearFecha(posicion.PREQ_DATE),
                    FechaEntrega = SAPFormatter.FormatearFecha(posicion.DELIV_DATE),
                    FechaEntregaDate = SAPFormatter.GetDateTime(posicion.DELIV_DATE),
                    FechaEstimadaLiberacion = SAPFormatter.FormatearFecha(posicion.REL_DATE),
                    DiasTratamientoEntrada = posicion.GR_PR_TIME,
                    PrecioSolp = posicion.PREQ_PRICE,
                    MonedaPrecio = posicion.PRICE_UNIT,
                    MonedaPrecioString = SAPFormatter.FormatearMonto(posicion.PRICE_UNIT, posicion.CURRENCY),
                    PrecioSolpString = SAPFormatter.FormatearMonto(posicion.PREQ_PRICE, posicion.CURRENCY),
                    Tipo = posicion.ITEM_CAT,
                    TipoImputacion = posicion.ACCTASSCAT,
                    ProveedorDeseado = posicion.DES_VENDOR,
                    ProveedorFijo = posicion.FIXED_VEND,
                    OrganizacionCompras = posicion.PURCH_ORG,
                    NumeroContratoMarco = posicion.AGREEMENT,
                    ProveedorFijoRazonSocial = posicion.NAME1,
                    PosicionContratoMarco = posicion.AGMT_ITEM,
                    NumeroPedido = posicion.PO_NUMBER,
                    PosicionPedido = posicion.PO_ITEM,
                    FechaPedido = posicion.PO_DATE,
                    EsPosicionConcluida = posicion.CLOSED,
                    Moneda = posicion.CURRENCY,
                    CantidadDiasEntrega = posicion.PLND_DELRY,
                    EstaBloqueada = posicion.REQ_BLOCKED,
                    EstadoSolpSap = posicion.PROCSTAT,
                    EstadoPosicion = posicion.DELETE_IND,
                    FechaEstimadaLiberacionDate = SAPFormatter.GetDateTime(posicion.REL_DATE),
                    Ordered = posicion.ORDERED,
                });
            }


            /*
            ZMPES5770[] imputacionesSuposiciones, //EX_SERVICEACCOUNT
             * 
                PREQ_NO	EBELN	Numero de SOLPED
                DOC_ITEM	EBELP	Numero de posición de Pedido
                OUTLINE	OUTLINE_NO	Número de estructuración
                SRV_LINE	EXTROW	Número de línea
                SERIAL_NO	NUMKN	Número correlativo asign. imputación línea de servicio
                SERIAL_NO_ITEM	ESKL_ZEKKN	Número actual de la imputación en ESKN
                DEL_IND	DEL	Indicador de borrado
                QUANTITY	MENGEV	Cantidad con signo +/-
                PERCENT	WPROZ	Porcentaje de repartición del valor de imputación
                NET_VALUE	SNETWR	Valor neto de la posición


             */

            if (result.ImputacionesSuposiciones == null)
            {
                result.ImputacionesSuposiciones = new List<ImputacionSuposicionSAP>();
            }
            foreach (var imputacionSuposicion in imputacionesSuposiciones)
            {
                result.ImputacionesSuposiciones.Add(new ImputacionSuposicionSAP
                {
                    NumeroSolicitud = imputacionSuposicion.PREQ_NO,
                    NumeroPosicion = imputacionSuposicion.DOC_ITEM,
                    NumeroEstructuracion = imputacionSuposicion.OUTLINE,
                    NumeroSubPosicion = imputacionSuposicion.SRV_LINE,
                    ImputacionLineaServicio = imputacionSuposicion.SERIAL_NO,
                    NumeroActualImputacion = imputacionSuposicion.SERIAL_NO_ITEM,
                    EstaBorrado = imputacionSuposicion.DEL_IND,
                    Cantidad = imputacionSuposicion.QUANTITY,
                    CantidadString = SAPFormatter.FormatearCantidad(imputacionSuposicion.QUANTITY, ""),
                    PorcentajeReparticionImputacion = imputacionSuposicion.PERCENT,
                    PorcentajeReparticionImputacionString = SAPFormatter.FormatearPorcentaje(imputacionSuposicion.PERCENT),
                    ValorNetoPosicion = imputacionSuposicion.NET_VALUE,
                    ValorNetoPosicionString = SAPFormatter.FormatearMonto(imputacionSuposicion.NET_VALUE, "ARP"),
                });
            }


            /*
             ZMPES5730[] suposicionesServicios) //EX_SERVICELINES

                PREQ_NO: número de Solicitud. 
                DOC_ITEM: Numero de Posición. Es el mismo valor que se entrega en (PREQ_ITEM)
                OUTLINE: incluido por relación de tablas.
                SRV_LINE: Número de subposición.
                DEL_IND: Indica si está borrada las posición. Borrada <> " "
                SERVICE: Código de Servicio. Si este campo viene lleno, el texto no se puede editar (SHORT_TEXT)
                SHORT_TEXT: Descripción del servicio. El servicio puede ser incluido a través de un código o mediante un texto libre escrito por el usuario.
                QUANTITY: Cantidad solicitada de dicho servicio.
                UOM: Unidad de Medida 
                GROSS_PRICE: Precio unitario.
                CURRENCY: Moneda
                MATL_GROUP: Grupo de Artículo.
                NET_VALUE: Valor total de la posición. --> Este dato no viene

             
             */

            if (result.ServiciosSuposiciones == null)
            {
                result.ServiciosSuposiciones = new List<SuposicionServicioSAP>();
            }
            foreach (var suposicionServicio in suposicionesServicios)
            {
                result.ServiciosSuposiciones.Add(new SuposicionServicioSAP
                {
                    NumeroSolicitud = suposicionServicio.PREQ_NO,
                    NumeroPosicion = suposicionServicio.DOC_ITEM,
                    NumeroEstructuracion = suposicionServicio.OUTLINE,
                    SumeroSubPosicion = suposicionServicio.SRV_LINE,
                    EstaBorrado = suposicionServicio.DEL_IND,
                    CodigoServicio = suposicionServicio.SERVICE,
                    DescripcionServicio = suposicionServicio.SHORT_TEXT,
                    Cantidad = suposicionServicio.QUANTITY,
                    CantidadString = SAPFormatter.FormatearCantidad(suposicionServicio.QUANTITY, suposicionServicio.UOM),
                    UnidadDeMedida = suposicionServicio.UOM,
                    PrecioUnitario = suposicionServicio.GROSS_PRICE,
                    PrecioUnitarioString = SAPFormatter.FormatearMonto(suposicionServicio.GROSS_PRICE, suposicionServicio.CURRENCY),
                    Moneda = suposicionServicio.CURRENCY,
                    GrupoDeArticulo = suposicionServicio.MATL_GROUP,
                    ValorTotalNeto = suposicionServicio.NET_PRICE,
                    ValorTotalNetoString = SAPFormatter.FormatearMonto(suposicionServicio.NET_PRICE, suposicionServicio.CURRENCY),
                });
            }

            return result;
        }
    }

    public class SuposicionServicioSAP
    {
        public string NumeroSolicitud { get; set; }
        public string NumeroPosicion { get; set; }
        public string NumeroEstructuracion { get; set; }
        public string SumeroSubPosicion { get; set; }
        public string EstaBorrado { get; set; }
        public string CodigoServicio { get; set; }
        public string DescripcionServicio { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadDeMedida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string Moneda { get; set; }
        public string GrupoDeArticulo { get; set; }
        public decimal ValorTotalNeto { get; set; }
        public string CantidadString { get; internal set; }
        public string PrecioUnitarioString { get; internal set; }
        public string ValorTotalNetoString { get; internal set; }
    }

    public class ImputacionSuposicionSAP
    {
        public string NumeroSolicitud { get; set; }
        public string NumeroPosicion { get; set; }
        public string NumeroEstructuracion { get; set; }
        public string NumeroSubPosicion { get; set; }
        public string ImputacionLineaServicio { get; set; }
        public string NumeroActualImputacion { get; set; }
        public string EstaBorrado { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PorcentajeReparticionImputacion { get; set; }
        public decimal ValorNetoPosicion { get; set; }
        public string CantidadString { get; internal set; }
        public string PorcentajeReparticionImputacionString { get; internal set; }
        public string ValorNetoPosicionString { get; internal set; }
    }

    public class PosicionSolpSAP
    {
        public string NumeroSolicitud { get; set; }
        public string NumeroPosicion { get; set; }
        public string TipoDocumento { get; set; }
        public string ImputacionActiva { get; set; }
        public string NumeroPaquete { get; set; }
        public string OrigenCreacion { get; set; }
        public string IndicadorDeLiberacion { get; set; }
        public string EstadoSolp { get; set; }
        public string GrupoCompras { get; set; }
        public string UsuarioCreado { get; set; }
        public string NombreSolicitante { get; set; }
        public string TextoPosicion { get; set; }
        public string Material { get; set; }
        public string CentroLogistico { get; set; }
        public string Almacen { get; set; }
        public string NumeroRequerimientoInterno { get; set; }
        public string GrupoArticulo { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public string FechaSolicitud { get; set; }
        public string FechaEntrega { get; set; }
        public DateTime FechaEntregaDate { get; set; }
        public string FechaEstimadaLiberacion { get; set; }
        public decimal DiasTratamientoEntrada { get; set; }
        public decimal PrecioSolp { get; set; }
        public decimal MonedaPrecio { get; set; }
        public string Tipo { get; set; }
        public string TipoImputacion { get; set; }
        public string ProveedorDeseado { get; set; }
        public string ProveedorFijo { get; set; }
        public string OrganizacionCompras { get; set; }
        public string NumeroContratoMarco { get; set; }
        public string PosicionContratoMarco { get; set; }
        public string NumeroPedido { get; set; }
        public string PosicionPedido { get; set; }
        public string FechaPedido { get; set; }
        public string EsPosicionConcluida { get; set; }
        public string Moneda { get; set; }
        public decimal CantidadDiasEntrega { get; set; }
        public string EstaBloqueada { get; set; }
        public string PrecioSolpString { get; internal set; }
        public string CantidadString { get; internal set; }
        public string MonedaPrecioString { get; internal set; }
        public string EstadoSolpSap { get; set; }
        public string EstadoPosicion { get; set; }
        public DateTime FechaEstimadaLiberacionDate { get; set; }
        public decimal Ordered { get;  set; }
        public string ProveedorFijoRazonSocial { get; internal set; }
        public string CodigoDeProveedor { get; internal set; }
    }

    public class DireccionSolpSAP
    {
        public string NumeroSolicitud { get; set; }
        public string NumeroPosicion { get; set; }
        public string NombreUbicacion { get; set; }
        public string CodigoPostal { get; set; }
        public string Ciudad { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Telefono { get; set; }
    }

    public class TipoImputacionSAP
    {
        public string NumeroSolicitud { get; set; }
        public string CentroDeBeneficio { get; set; }
        public string COArea { get; set; }
        public string IdOrden { get; set; }
        public string NumeroActivoFijo { get; set; }
        public string DivisionImputada { get; set; }
        public string CentroDeCosto { get; set; }
        public string CuentaContableImputada { get; set; }
        public decimal PrecioNetoImputado { get; set; }
        public decimal PorcentajeDistribucion { get; set; }
        public decimal CantidadesImputadas { get; set; }
        public string ImputacionActiva { get; set; }
        public string NumeroDeSerie { get; set; }
        public string NumeroPosicion { get; set; }
        public string CantidadesImputadasString { get; internal set; }
        public string PorcentajeDistribucionString { get; internal set; }
        public string PrecioNetoImputadoString { get; internal set; }
        public string NumeroOrdenDeCompra { get; internal set; }
    }

    public class ObtenerSolpSAPResponse
    {
        internal object Error;

        public IList<TipoImputacionSAP> TipoImputaciones { get; set; }
        public IList<DireccionSolpSAP> Direcciones { get; set; }
        public IList<PosicionSolpSAP> Posiciones { get; set; }
        public IList<ImputacionSuposicionSAP> ImputacionesSuposiciones { get; set; }
        public IList<SuposicionServicioSAP> ServiciosSuposiciones { get; set; }
        public List<ArchivoSolpDto> Archivos { get; internal set; }
        public string ObservacionesGeneracion { get; internal set; }
    }

    public class ObtenerSolpRequest
    {
        public bool ObtenerImputacion { get; set; }
        public string TipoDeImputacion { get; set; }
        public string OrigenCreacion { get; set; }
        public bool MostrarItemsBorrados { get; set; }
        public bool ObtenerDireccionDeEntrega { get; set; }
        public string FiltroTipoPosicion { get; set; }
        public string CentroLogistico { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string NumeroSolp { get; set; }
        public string IndicadorDeLiberacion { get; set; }
        public bool ObtenerServicios { get; set; }
        public List<string> CreadoPorUsuarios { get; set; }

    }

    public class ErrorObtenerSOLP
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string rTipo { get; set; }
    }

    public class ArchivoSolpDto
    {
        public string DocId { get; set; }
        public string Nombre { get; internal set; }
        public string Tipo { get; internal set; }
    }


}

