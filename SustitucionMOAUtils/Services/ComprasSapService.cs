// Ignore Spelling: Solp Sustitucion Utils Solpe Posicion numeros Direccion

using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.sap;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio.Extensiones;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ModificarOCWebServiceMOA;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using static SustitucionMOAWS.WSConsumers.ModificarOrdenDeCompraConsumerMOA;

namespace SustitucionMOAUtils.Services
{
    public class ComprasSapService : IComprasSapService
    {
        private readonly IObtenerCecoSolpConsumerMOA CentroDeCostoSolpConsumerMOA;
        private readonly ICrearPedidoConsumerMOA crearPedidoConsumerMOA;
        private readonly ICrearSolpConsumerMOA crearSolpConsumerMOA;
        private readonly IModificarOrdenDeCompraConsumerMOA modificarOrdenDeCompraConsumerMOA;
        private readonly IModificarSolpConsumerMOA modificarSolpConsumerMOA;
        private readonly IObtenerCuentasSolpConsumerMOA cuentasSolpConsumerMOA;
        private readonly IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA;
        private readonly IObtenerOrdenSolpConsumerMOA ordenesSolpConsumerMOA;
        private readonly IObtenerServiciosSolpConsumerMOA serviciosSolpConsumerMOA;
        private readonly IObtenerSolpConsumerMOA obtenerSolpConsumerMOA;
        private readonly IObtenerFuenteAprovisionamientoConsumerMOA obtenerFuenteAprovisionamientoConsumerMOA;
        private readonly IObtenerContratoSolpConsumerMOA obtenerContratoSolpConsumerMOA;
        private readonly IListarSolpPendientesConsumerMOA listarSolpPendienteConsumeMOA;
        private readonly IReporteOrdenDeCompraConsumerMOA reporteOrdenDeCompraConsumerMOA;
        private readonly IObtenerPDFOrdenCompraConsumerMOA obtenerPDFOrdenCompraConsumerMOA;
        private readonly IObtenerAdjuntosSOLPEDConsumerMOA obtenerAdjuntosSOLPEDConsumerMOA;
        private readonly IObtenerOrdenesDeCompraParaSOLPConsumerMOA obtenerOrdenesDeCompraParaSOLPConsumerMOA;

        private readonly ICentroDireccionService centroDireccionService;
        private readonly ITablaSapService tablaSapService;
        private readonly IUnidadMedidaService unidadMedidaService;
        private readonly IUsuarioService usuarioService;
        private readonly ITipoCambioService tipoCambioService;

        public ComprasSapService(IObtenerCecoSolpConsumerMOA obtenerCentroDeCostoSolpConsumerMOA,
                                 ICrearPedidoConsumerMOA crearPedidoConsumerMOA,
                                 ICrearSolpConsumerMOA crearSolpConsumerMOA,
                                 IModificarOrdenDeCompraConsumerMOA modificarOrdenDeCompraConsumerMOA,
                                 IModificarSolpConsumerMOA modificarSolpConsumerMOA,
                                 IObtenerCuentasSolpConsumerMOA obtenerCuentasSolpConsumerMOA,
                                 IObtenerOrdenSolpConsumerMOA obtenerOrdenSolpConsumerMOA,
                                 IObtenerServiciosSolpConsumerMOA obtenerServiciosSolpConsumerMOA,
                                 IObtenerOrdenDeCompraConsumerMOA obtenerOrdenDeCompraConsumerMOA,
                                 IObtenerSolpConsumerMOA obtenerSolpConsumerMOA,
                                 IObtenerFuenteAprovisionamientoConsumerMOA obtenerFuenteAprovisionamientoConsumerMOA,
                                 IObtenerContratoSolpConsumerMOA obtenerContratoSolpConsumerMOA,
                                 IListarSolpPendientesConsumerMOA listarSolpPendientesConsumerMOA,
                                 IReporteOrdenDeCompraConsumerMOA reporteOrdenDeCompraConsumerMOA,
                                 IObtenerPDFOrdenCompraConsumerMOA obtenerPDFOrdenCompraConsumerMOA,
                                 IObtenerAdjuntosSOLPEDConsumerMOA obtenerAdjuntosSOLPEDConsumerMOA,
                                 IObtenerOrdenesDeCompraParaSOLPConsumerMOA obtenerOrdenesDeCompraParaSOLPConsumerMOA,
                                 ICentroDireccionService centroDireccionService,
                                 ITablaSapService tablaSapService,
                                 IUnidadMedidaService unidadMedidaService,
                                 IUsuarioService usuarioService,
                                 ITipoCambioService tipoCambioService)
        {
            this.CentroDeCostoSolpConsumerMOA = obtenerCentroDeCostoSolpConsumerMOA;
            this.crearPedidoConsumerMOA = crearPedidoConsumerMOA;
            this.crearSolpConsumerMOA = crearSolpConsumerMOA;
            this.modificarOrdenDeCompraConsumerMOA = modificarOrdenDeCompraConsumerMOA;
            this.modificarSolpConsumerMOA = modificarSolpConsumerMOA;
            this.cuentasSolpConsumerMOA = obtenerCuentasSolpConsumerMOA;
            this.obtenerOrdenDeCompraConsumerMOA = obtenerOrdenDeCompraConsumerMOA;
            this.ordenesSolpConsumerMOA = obtenerOrdenSolpConsumerMOA;
            this.serviciosSolpConsumerMOA = obtenerServiciosSolpConsumerMOA;
            this.obtenerSolpConsumerMOA = obtenerSolpConsumerMOA;
            this.obtenerFuenteAprovisionamientoConsumerMOA = obtenerFuenteAprovisionamientoConsumerMOA;
            this.obtenerContratoSolpConsumerMOA = obtenerContratoSolpConsumerMOA;
            this.listarSolpPendienteConsumeMOA = listarSolpPendientesConsumerMOA;
            this.reporteOrdenDeCompraConsumerMOA = reporteOrdenDeCompraConsumerMOA;
            this.centroDireccionService = centroDireccionService;
            this.tablaSapService = tablaSapService;
            this.unidadMedidaService = unidadMedidaService;
            this.usuarioService = usuarioService;
            this.tipoCambioService = tipoCambioService;
            this.obtenerPDFOrdenCompraConsumerMOA = obtenerPDFOrdenCompraConsumerMOA;
            this.obtenerAdjuntosSOLPEDConsumerMOA = obtenerAdjuntosSOLPEDConsumerMOA;
            this.obtenerOrdenesDeCompraParaSOLPConsumerMOA = obtenerOrdenesDeCompraParaSOLPConsumerMOA;
        }

        public SolpSAPDto ConvertirSOLPSAP(Solp solpActual)
        {
            SolpSAPDto solpSAP = new SolpSAPDto();
            solpSAP.Id = solpActual.Id;
            if (!string.IsNullOrEmpty(solpActual.NroSolp))
            {
                solpSAP.NroSolp = solpActual.NroSolp;
            }
            #region posiciones y servicios

            //•	el problema está en que siempre debes poner en el campo OUT_LINE= "000000001", sino debieras llenar otra tabla de SAP que no la estamos cargando. Para quitarle complejidad se saco dicha tabla.
            const string OUTLINE_NUMBER = "000000001";
            const string SERVICE_ACCOUNT_SERIAL_NUMBER = "01";

            const string TEXT_ID = "B03";
            const string FORMAT_TEXT = "*";

            solpSAP.IM_PR_TYPE = solpActual.ClaseDocumento.CodigoSap;

            var unidadesCodigoSap = solpActual.Posiciones.SelectMany(p => new[] { p.Unidad?.CodigoSap }.Concat(p.Subposiciones.Select(sp => sp.Unidad.CodigoSap))).Distinct();

            var unidadesMedidaSap = unidadMedidaService.GetUnidadesMedidaSap(x => unidadesCodigoSap.Contains(x.Comercial),
                                                                             x => new { x.Comercial, x.UM })
                .ConvertAll(x => Tuple.Create(x.Comercial, x.UM));

            foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
            {
                bool eliminarPosicion = false;
                bool eliminarSubPosicion = false;
                if (posicion.TipoPosicion.Codigo.ToLower() == "servicios")
                {
                    eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                    eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                }

                eliminarPosicion = eliminarPosicion || !posicion.Estado;

                NumeroPosicion numeroPosicion = posicion.Indice;

                var IM_PRITEM = new ZMPES7090();

                //Nombre: ZBAPIMEREQITEMIMP Denominación: Posición de SOLPED
                IM_PRITEM.PREQ_ITEM = numeroPosicion.AsPreqItem(); //PREQ_ITEM BNFPO Número de posición de la solicitud de pedido
                IM_PRITEM.PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP EKGRP Grupo de compras
                IM_PRITEM.CREATED_BY = solpActual.UsuarioCreacion != null ? solpActual.UsuarioCreacion.UsuarioSap : usuarioService.GetUsuarioPorId(solpActual.UsuarioCreacion_Id ?? 0).UsuarioSap; //CREATED_BY ERNAM Nombre del responsable que ha añadido el objeto
                IM_PRITEM.PREQ_NAME = posicion.Solicitante; //PREQ_NAME AFNAM Nombre del solicitante
                IM_PRITEM.SHORT_TEXT = posicion.Tarea; //SHORT_TEXT TXZ01 Texto breve
                IM_PRITEM.PLANT = posicion.Centro.CodigoSap.ToString(); //PLANT EWERK   Centro
                IM_PRITEM.STORE_LOC = solpActual.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento ? "" : posicion.Almacen.CodigoSap.ToString(); //STORE_LOC   LGORT_D Almacén
                IM_PRITEM.TRACKINGNO = posicion.NroNecesidad; //TRACKINGNO BEDNR   Número de necesidad
                IM_PRITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString(); //MATL_GROUP  MATKL Grupo de artículos
                IM_PRITEM.PREQ_DATE = SAPFormatter.PrepararFecha(solpActual.FechaCreacion); //PREQ_DATE   BADAT Fecha de solicitud
                IM_PRITEM.DELIV_DATE = SAPFormatter.PrepararFecha(posicion.FechaEntregaServicio ?? DateTime.Now); //DELIV_DATE EINDT   Fecha de entrega de posición
                IM_PRITEM.REL_DATE = null;

                //Estos datos se envian si la posición es de materiales
                if (posicion.TipoPosicion.Codigo.ToLower() == "materiales")
                {
                    IM_PRITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : ""; //MATERIAL MATNR18 Número de material(18 caracteres)
                    IM_PRITEM.QUANTITY = (Decimal)posicion.Cantidad; //QUANTITY BAMNG   Cantidad solicitud de pedido
                    IM_PRITEM.QUANTITYSpecified = true;
                    IM_PRITEM.UNIT = unidadesMedidaSap?.Find(u => u.Item1 == posicion.Unidad.CodigoSap).Item2; //UNIT BAMEI - Cambia el código de la unidad solicitada por su equivalente 'UM' de la tabla UnidadMedidaSap
                    //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido
                    IM_PRITEM.PREQ_PRICE = (Decimal)posicion.PrecioBruto; //PREQ_PRICE  BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
                    IM_PRITEM.PREQ_PRICESpecified = true;
                    //IM_PRITEM.PRICE_UNIT = null; //PRICE_UNIT EPEIN Cantidad base  
                    //IM_PRITEM.PRICE_UNITSpecified = true;

                    //Estos datos de imputacion se envian solo para materiales por que en servicio van a nivel de subposicion
                    BAPIMEREQACCOUNT imputacionPosicionMateriales = solpSAP.IM_PRACCOUNTList.FirstOrDefault(x =>
                            x.PREQ_ITEM == numeroPosicion.AsPreqItem() && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            x.SERIAL_NO == SERVICE_ACCOUNT_SERIAL_NUMBER && //SERIAL_NO	DZEKKN	Número actual de la imputación
                            x.GL_ACCOUNT == getCodigoTablaSap(posicion.CuentaMayorSap) &&//GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            x.COSTCENTER == getCodigoTablaSap(posicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
                            x.ORDERID == getCodigoTablaSap(posicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
                            x.PROFIT_CTR == getCodigoTablaSap(posicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
                    );

                    if (imputacionPosicionMateriales is null)
                    {
                        imputacionPosicionMateriales = new BAPIMEREQACCOUNT
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(), //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER, //SERIAL_NO	DZEKKN	Número actual de la imputación
                            GL_ACCOUNT = getCodigoTablaSap(posicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            COSTCENTER = getCodigoTablaSap(posicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
                            ORDERID = getCodigoTablaSap(posicion.TipoImputacionSap), //ORDERID	AUFNR	Número de orden
                            PROFIT_CTR = getCodigoTablaSap(posicion.TipoImputacionSap), //PROFIT_CTR	PRCTR	Centro de beneficio
                            BUS_AREA = "GENE",
                            CO_AREA = "MOA"
                        };

                        solpSAP.IM_PRACCOUNTList.Add(imputacionPosicionMateriales);

                        solpSAP.IM_PRACCOUNTXList.Add(new BAPIMEREQACCOUNTX
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(),
                            SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                            PREQ_ITEMX = "X",
                            SERIAL_NOX = "X",
                            GL_ACCOUNT = "X",
                            COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                            ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                            PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : "",
                            BUS_AREA = "X",
                            CO_AREA = "X"
                        });
                    }
                    //Este metodo lo usamos para enviar el texto de suministro. Solo se pueden enviar 132 caracteres por linea
                    var linesTextoSuministro = getLinesFromTextoSuministro(posicion.TextoSuministro);

                    linesTextoSuministro.ForEach(texto =>
                    {
                        solpSAP.IM_PRITEMTEXTList.Add(new BAPIMEREQITEMTEXT
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(),
                            TEXT_ID = TEXT_ID,
                            TEXT_FORM = FORMAT_TEXT,
                            TEXT_LINE = texto
                        });
                    });

                    solpSAP.IM_SERVICEACCOUNTList.Add(new BAPI_SRV_ACC_DATA
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = numeroPosicion.AsSerialNumber(),
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });
                }

                //IM_PRITEM.UNIT = null; //UNIT BAMEI   Unidad de medida de solicitud pedido
                //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido

                //Indica el tipo de posicion de la solp
                switch (posicion.TipoPosicion.Codigo.ToLower())
                //ITEM_CAT PSTYP   Tipo de posición del documento de compras
                {
                    case "servicio":
                        IM_PRITEM.ITEM_CAT = "9";
                        break;

                    case "materiales":
                    default:
                        IM_PRITEM.ITEM_CAT = "0";
                        break;
                }

                //Indica el tipo de imputacion de la solp
                if (posicion.TipoImputacion != null)
                {
                    switch (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower())
                    //ACCTASSCAT  KNTTP Tipo de imputación
                    {
                        case "centrodecosto":
                            IM_PRITEM.ACCTASSCAT = "K";
                            break;
                        case "ordendeot":
                            IM_PRITEM.ACCTASSCAT = "F";
                            break;
                        case "ordendeinversion":
                            IM_PRITEM.ACCTASSCAT = "F";
                            break;
                        case "siniestrobeneficio":
                            IM_PRITEM.ACCTASSCAT = "Y";
                            break;
                    }
                }
                else
                {
                    IM_PRITEM.ACCTASSCAT = "";
                }

                //IM_PRITEM.DES_VENDOR = null; //DES_VENDOR WLIEF   Proveedor deseado
                //Contrato marco          
                IM_PRITEM.FIXED_VEND = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp != null ? posicion.ProveedorAdjudicado.ObtenerCodigoProveedor() : ""; //FIXED_VEND FLIEF   Proveedor fijo
                IM_PRITEM.DES_VENDOR = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp == null ? posicion.ProveedorAdjudicado.ObtenerCodigoProveedor() : "";
                IM_PRITEM.PURCH_ORG = posicion.OrganizacionDeComprasCodigo; //PURCH_ORG EKORG   Organización de compras
                IM_PRITEM.AGREEMENT = posicion.NumeroContratoSuperior; //AGREEMENT   KONNR Número del contrato superior
                IM_PRITEM.AGMT_ITEM = posicion.NumeroPosicionContratoSuperior; //AGMT_ITEM   KTPNR Número de posición del contrato superior
                IM_PRITEM.INFO_REC = posicion.RegistroInfoNro; //INFO_REC    INFNR Número del registro info de compras
                IM_PRITEM.CLOSED = null; //Contrato marco? No está en este MVP //CLOSED  EBAKZ Solicitud de pedido concluida
                IM_PRITEM.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY    WAERS Clave de moneda
                IM_PRITEM.CURRENCY_ISO = null; //CURRENCY_ISO BAPIISOCD   Código ISO para moneda
                                               //IM_PRITEM.INFO_REC = null; //INFO_REC    INFNR Número del registro info de compras                            
                IM_PRITEM.PLND_DELRY = (decimal)posicion.PlazoEntrega; //PLND_DELRY PLIFZ   Plazo de entrega previsto en días
                IM_PRITEM.PLND_DELRYSpecified = true;
                IM_PRITEM.PCKG_NO = numeroPosicion.AsNumeroPaquete(); //PCKG_NO PACKNO  Nº paquete

                //Indica si esta borrada la posicion 
                if (!string.IsNullOrEmpty(solpActual.NroSolp))
                {
                    IM_PRITEM.DELETE_IND = SAPFormatter.FormatearBooleano(eliminarPosicion);
                }

                //Agregamos todos los items a la estructura de posicion
                solpSAP.IM_PRITEMList.Add(IM_PRITEM);

                //Esta es una lista de campos que SAP nos pide que enviemos una "X" con los datos.
                solpSAP.IM_PRITEMXList.Add(new ZMPES8000
                {
                    PREQ_ITEM = numeroPosicion.AsPreqItem(),
                    PREQ_ITEMX = "X",
                    PUR_GROUP = "X",
                    CREATED_BY = "X",
                    PREQ_NAME = "X",
                    SHORT_TEXT = "X",
                    MATERIAL = (IM_PRITEM.MATL_GROUP != null) ? "X" : "",
                    PLANT = "X",
                    STORE_LOC = "X",
                    TRACKINGNO = "X",
                    MATL_GROUP = "X",
                    QUANTITY = (IM_PRITEM.QUANTITY == 0) ? "" : "X",
                    UNIT = "X",
                    //PREQ_UNIT_ISO = "X",
                    PREQ_DATE = "X",
                    DELIV_DATE = "X",
                    //REL_DATE = "X",
                    //GR_PR_TIME = "X",
                    PREQ_PRICE = (IM_PRITEM.PREQ_PRICE == 0) ? "" : "X",
                    //PRICE_UNIT = "X"
                    ITEM_CAT = "X",
                    ACCTASSCAT = "X",
                    DES_VENDOR = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp == null ? "X" : "",
                    FIXED_VEND = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp != null ? "X" : "",
                    PURCH_ORG = !string.IsNullOrEmpty(posicion.OrganizacionDeComprasCodigo) ? "X" : "",
                    AGREEMENT = !string.IsNullOrEmpty(posicion.NumeroContratoSuperior) ? "X" : "",
                    AGMT_ITEM = !string.IsNullOrEmpty(posicion.NumeroPosicionContratoSuperior) ? "X" : "",
                    INFO_REC = !string.IsNullOrEmpty(posicion.RegistroInfoNro) ? "X" : "",
                    //CLOSED = "X",
                    CURRENCY = "X",
                    //CURRENCY_ISO = "X",
                    PLND_DELRY = "X",
                    PCKG_NO = "X",
                    DELETE_IND = string.IsNullOrEmpty(solpActual.NroSolp) ? "" : "X",
                });


                //---Desde aca empiezan las subposiciones---

                var numeroSerialNumberItem = 0;

                foreach (var subPosicion in posicion.Subposiciones.OrderBy(x => x.Id))
                {
                    NumeroSubPosicion numeroSubPosicion = subPosicion.Numero;

                    //SUBPOSICION
                    var IM_SERVICELINE = new BAPI_SRV_SERVICE_LINE();

                    IM_SERVICELINE.DOC_ITEM = numeroPosicion.AsDocItem(); //DOC_ITEM EBELP   Número de posición de la solicitud de pedido = PREQ_ITEM
                    IM_SERVICELINE.OUTLINE = OUTLINE_NUMBER; //OUTLINE OUTLINE_NO  Número de estructuración
                    IM_SERVICELINE.SRV_LINE = numeroSubPosicion.AsServiceLineNumber(); //SRV_LINE    EXTROW Número de línea
                    IM_SERVICELINE.DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)); //DEL_IND DEL Indicador de borrado

                    if (subPosicion.ServicioSolp != null)
                        IM_SERVICELINE.SERVICE = subPosicion.ServicioSolp.Codigo.ToString(); //SERVICE ASNUM Número de servicio
                    else
                        IM_SERVICELINE.SHORT_TEXT = subPosicion.Tarea; //SHORT_TEXT SH_TEXT1 Texto breve

                    IM_SERVICELINE.QUANTITY = subPosicion.Cantidad.Value; //QUANTITY MENGEV  Cantidad con signo +/ -
                    IM_SERVICELINE.QUANTITYSpecified = true;
                    IM_SERVICELINE.UOM = unidadesMedidaSap?.Find(u => u.Item1 == subPosicion.Unidad.CodigoSap).Item2; //UOM MEINS - Cambia el código de la unidad solicitada por su equivalente 'UM' de la tabla UnidadMedidaSap
                    //IM_SERVICELINE.UOM_ISO = null; //UOM_ISO MEINS_ISO   Unidad medida base en código ISO
                    IM_SERVICELINE.GROSS_PRICE = subPosicion.PrecioBruto.Value; //GROSS_PRICE SBRTWR Precio bruto Unitario
                    IM_SERVICELINE.GROSS_PRICESpecified = true;
                    IM_SERVICELINE.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY WAERS   Clave de moneda

                    solpSAP.IM_SERVICELINESList.Add(IM_SERVICELINE);

                    solpSAP.IM_SERVICELINESXList.Add(new BAPI_SRV_SERVICE_LINEX
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SRV_LINE = numeroSubPosicion.AsServiceLineNumber(),
                        DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)),
                        SERVICE = "X",
                        SHORT_TEXT = (subPosicion.ServicioSolp == null) ? "X" : "",
                        QUANTITY = "X",
                        UOM = "X",
                        //UOM_ISO = "X",
                        GROSS_PRICE = "X",
                        CURRENCY = "X"
                    });

                    BAPIMEREQACCOUNT imputacionPosicion = solpSAP.IM_PRACCOUNTList.Find(x =>
                            x.PREQ_ITEM == numeroPosicion.AsPreqItem() && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) && //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
                            x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
                            x.PROFIT_CTR == getCodigoTablaSap(subPosicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
                        );

                    if (imputacionPosicion is null)
                    {
                        numeroSerialNumberItem++;

                        imputacionPosicion = new BAPIMEREQACCOUNT
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(), //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            SERIAL_NO = $"{numeroSerialNumberItem:00}",//indiceImputacion, //SERIAL_NO    DZEKKN  Número actual de la imputación
                            QUANTITY = subPosicion.Cantidad.Value, //QUANTITY	MENGE_D	Cantidad
                            GL_ACCOUNT = getCodigoTablaSap(subPosicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            COSTCENTER = getCodigoTablaSap(subPosicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
                            ORDERID = getCodigoTablaSap(subPosicion.TipoImputacionSap), //ORDERID	AUFNR	Número de orden
                            PROFIT_CTR = getCodigoTablaSap(subPosicion.TipoImputacionSap), //PROFIT_CTR	PRCTR	Centro de beneficio
                            BUS_AREA = "GENE"
                        };

                        solpSAP.IM_PRACCOUNTList.Add(imputacionPosicion);

                        solpSAP.IM_PRACCOUNTXList.Add(new BAPIMEREQACCOUNTX
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(),
                            SERIAL_NO = imputacionPosicion.SERIAL_NO,
                            PREQ_ITEMX = "X",
                            SERIAL_NOX = "X",
                            QUANTITY = "X",
                            GL_ACCOUNT = "X",
                            BUS_AREA = "X",
                            COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                            ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                            PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : ""
                        });
                    }

                    //IMPUTACION SUBPOSICION
                    solpSAP.IM_SERVICEACCOUNTList.Add(new BAPI_SRV_ACC_DATA
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SRV_LINE = numeroSubPosicion.AsServiceLineNumber(),
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = imputacionPosicion.SERIAL_NO,
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER, //Preguntar a Ulises
                        SRV_LINE = numeroSubPosicion.AsServiceLineNumber(), //Preguntar a Ulises
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });
                }

                //Estos son los metodos con los que nos fijamos si se editó algun campo de la direccion de entrega. Si no editó ninguno no 
                //hace falta enviar a SAP, pero si se editó por lo menos uno tenemos que enviar todos los campos 
                CentroDireccion centroPorDefecto = centroDireccionService.GetCentroDireccionByCodigoSap(posicion.Centro.CodigoSap);
                TablaSap centroPorDefecto2 = tablaSapService.GetById(posicion.Centro_Id);

                if (centroPorDefecto2.Descripcion != posicion.NombreEntrega ||
                    centroPorDefecto.Cp != posicion.CpEntrega ||
                    centroPorDefecto2.Descripcion != posicion.Centro.Descripcion ||
                    centroPorDefecto.Direccion != posicion.CalleEntrega ||
                    centroPorDefecto.Numero != posicion.NumeroEntrega)
                {
                    solpSAP.IM_PRADDRDELIVERYList.Add(
                    new ZMPES7110
                    {
                        PREQ_NO = numeroPosicion.AsPreqItem(), //PREQ_NO BANFN   Numero de SOLPED
                        PREQ_ITEM = numeroPosicion.AsPreqItem(), //PREQ_ITEM   BNFPO Número de posición de la solicitud de pedido
                        NAME = posicion.NombreEntrega, //NAME    AD_NAME1 Nombre 1
                        POSTL_COD1 = posicion.CpEntrega, //POSTL_COD1 AD_PSTCD1   Código postal de la población
                        CITY = posicion.Centro.Descripcion, //CITY    AD_CITY1 Población
                        STREET = posicion.CalleEntrega, //STREET AD_STREET   Calle
                        TEL1_NUMBR = posicion.NumeroEntrega, //TEL1_NUMBR  AD_TLNMBR1 Primer número teléfono: Prefijo + número
                    });
                }
            }

            //Este metodo lo usamos para enviar el texto de observaciones. Solo se pueden enviar 132 caracteres por linea

            var textosObservacion = "";

            //Enviar observaciones del paso 2 o las observaciones de las condiciones especiales segun prioridad
            if (!string.IsNullOrEmpty(solpActual.Pliego.ObservacionesGeneracion))
            {
                // Si ObservacionesGeneracion tiene algún valor, se envía este.
                textosObservacion = solpActual.Pliego.ObservacionesGeneracion;
            }
            else if (!string.IsNullOrEmpty(solpActual.Pliego.ObservacionesCotizacionCondEsp))
            {
                // Si ObservacionesGeneracion está vacío y ObservacionesCotizacionCondEsp tiene algún valor, se envía este.
                textosObservacion = solpActual.Pliego.ObservacionesCotizacionCondEsp;
            }

            var linesObservacion = getLinesFromTextoSuministro(textosObservacion);

            linesObservacion.ForEach(texto =>
            {
                solpSAP.IM_PRHEADERTEXTList.Add(new BAPIMEREQHEADTEXT
                {
                    PREQ_ITEM = "00000",
                    TEXT_ID = "B01",
                    TEXT_FORM = "*",
                    TEXT_LINE = texto,
                });
            });
            #endregion

            return solpSAP;
        }

        public SolpSAPSinPIDto ConvertirSOLPSAPSinPI(Solp solpActual)
        {
            SolpSAPSinPIDto solpSAP = new SolpSAPSinPIDto();
            solpSAP.Id = solpActual.Id;
            if (!string.IsNullOrEmpty(solpActual.NroSolp))
            {
                solpSAP.NroSolp = solpActual.NroSolp;
            }
            #region posiciones y servicios

            //•	el problema está en que siempre debes poner en el campo OUT_LINE= "000000001", sino debieras llenar otra tabla de SAP que no la estamos cargando. Para quitarle complejidad se saco dicha tabla.
            const string OUTLINE_NUMBER = "000000001";
            const string SERVICE_ACCOUNT_SERIAL_NUMBER = "01";

            const string TEXT_ID = "B03";
            const string FORMAT_TEXT = "*";

            solpSAP.IM_PR_TYPE = solpActual.ClaseDocumento.CodigoSap;

            var unidadesCodigoSap = solpActual.Posiciones.SelectMany(p => new[] { p.Unidad?.CodigoSap }.Concat(p.Subposiciones.Select(sp => sp.Unidad.CodigoSap))).Distinct();

            var unidadesMedidaSap = unidadMedidaService.GetUnidadesMedidaSap(x => unidadesCodigoSap.Contains(x.Comercial),
                                                                             x => new { x.Comercial, x.UM })
                .ConvertAll(x => Tuple.Create(x.Comercial, x.UM));

            foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
            {
                bool eliminarPosicion = false;
                bool eliminarSubPosicion = false;
                if (posicion.TipoPosicion.Codigo.ToLower() == "servicios")
                {
                    eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                    eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                }

                eliminarPosicion = eliminarPosicion || !posicion.Estado;

                NumeroPosicion numeroPosicion = posicion.Indice;

                var IM_PRITEM = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7090();

                //Nombre: ZBAPIMEREQITEMIMP Denominación: Posición de SOLPED
                IM_PRITEM.PREQ_ITEM = numeroPosicion.AsPreqItem(); //PREQ_ITEM BNFPO Número de posición de la solicitud de pedido
                IM_PRITEM.PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP EKGRP Grupo de compras
                IM_PRITEM.CREATED_BY = solpActual.UsuarioCreacion != null ? solpActual.UsuarioCreacion.UsuarioSap : usuarioService.GetUsuarioPorId(solpActual.UsuarioCreacion_Id ?? 0).UsuarioSap; //CREATED_BY ERNAM Nombre del responsable que ha añadido el objeto
                IM_PRITEM.PREQ_NAME = posicion.Solicitante; //PREQ_NAME AFNAM Nombre del solicitante
                IM_PRITEM.SHORT_TEXT = posicion.Tarea; //SHORT_TEXT TXZ01 Texto breve
                IM_PRITEM.PLANT = posicion.Centro.CodigoSap.ToString(); //PLANT EWERK   Centro
                IM_PRITEM.STORE_LOC = solpActual.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento ? "" : posicion.Almacen.CodigoSap.ToString(); //STORE_LOC   LGORT_D Almacén
                IM_PRITEM.TRACKINGNO = posicion.NroNecesidad; //TRACKINGNO BEDNR   Número de necesidad
                IM_PRITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString(); //MATL_GROUP  MATKL Grupo de artículos
                IM_PRITEM.PREQ_DATE = SAPFormatter.PrepararFecha(solpActual.FechaCreacion); //PREQ_DATE   BADAT Fecha de solicitud
                IM_PRITEM.DELIV_DATE = SAPFormatter.PrepararFecha(posicion.FechaEntregaServicio ?? DateTime.Now); //DELIV_DATE EINDT   Fecha de entrega de posición
                IM_PRITEM.REL_DATE = null;
                IM_PRITEM.DELIV_TIME = string.Empty;
                //Estos datos se envian si la posición es de materiales
                if (posicion.TipoPosicion.Codigo.ToLower() == "materiales")
                {
                    IM_PRITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : ""; //MATERIAL MATNR18 Número de material(18 caracteres)
                    IM_PRITEM.QUANTITY = (Decimal)posicion.Cantidad; //QUANTITY BAMNG   Cantidad solicitud de pedido
                    //IM_PRITEM.QUANTITYSpecified = true;
                    IM_PRITEM.UNIT = unidadesMedidaSap?.Find(u => u.Item1 == posicion.Unidad.CodigoSap).Item2; //UNIT BAMEI - Cambia el código de la unidad solicitada por su equivalente 'UM' de la tabla UnidadMedidaSap
                    //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido
                    IM_PRITEM.PREQ_PRICE = Math.Round((Decimal)posicion.PrecioBruto,4); //PREQ_PRICE  BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
                    //IM_PRITEM.PREQ_PRICESpecified = true;
                    //IM_PRITEM.PRICE_UNIT = null; //PRICE_UNIT EPEIN Cantidad base  
                    //IM_PRITEM.PRICE_UNITSpecified = true;

                    //Estos datos de imputacion se envian solo para materiales por que en servicio van a nivel de subposicion
                    SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT imputacionPosicionMateriales = solpSAP.IM_PRACCOUNTList.FirstOrDefault(x =>
                            x.PREQ_ITEM == numeroPosicion.AsPreqItem() && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            x.SERIAL_NO == SERVICE_ACCOUNT_SERIAL_NUMBER && //SERIAL_NO	DZEKKN	Número actual de la imputación
                            x.GL_ACCOUNT == getCodigoTablaSap(posicion.CuentaMayorSap) &&//GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            x.COSTCENTER == getCodigoTablaSap(posicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
                            x.ORDERID == getCodigoTablaSap(posicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
                            x.PROFIT_CTR == getCodigoTablaSap(posicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
                    );

                    if (imputacionPosicionMateriales is null)
                    {
                        imputacionPosicionMateriales = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(), //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER, //SERIAL_NO	DZEKKN	Número actual de la imputación
                            GL_ACCOUNT = getCodigoTablaSap(posicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            COSTCENTER = getCodigoTablaSap(posicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
                            ORDERID = getCodigoTablaSap(posicion.TipoImputacionSap), //ORDERID	AUFNR	Número de orden
                            PROFIT_CTR = getCodigoTablaSap(posicion.TipoImputacionSap), //PROFIT_CTR	PRCTR	Centro de beneficio
                            BUS_AREA = "GENE",
                            CO_AREA = "MOA"
                        };

                        solpSAP.IM_PRACCOUNTList.Add(imputacionPosicionMateriales);

                        solpSAP.IM_PRACCOUNTXList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNTX
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(),
                            SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                            PREQ_ITEMX = "X",
                            SERIAL_NOX = "X",
                            GL_ACCOUNT = "X",
                            COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                            ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                            PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : "",
                            BUS_AREA = "X",
                            CO_AREA = "X"
                        });
                    }
                    //Este metodo lo usamos para enviar el texto de suministro. Solo se pueden enviar 132 caracteres por linea
                    var linesTextoSuministro = getLinesFromTextoSuministro(posicion.TextoSuministro);

                    linesTextoSuministro.ForEach(texto =>
                    {
                        solpSAP.IM_PRITEMTEXTList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQITEMTEXT
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(),
                            TEXT_ID = TEXT_ID,
                            TEXT_FORM = FORMAT_TEXT,
                            TEXT_LINE = texto
                        });
                    });

                    solpSAP.IM_SERVICEACCOUNTList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATA
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = numeroPosicion.AsSerialNumber(),
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });
                }

                //IM_PRITEM.UNIT = null; //UNIT BAMEI   Unidad de medida de solicitud pedido
                //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido

                //Indica el tipo de posicion de la solp
                switch (posicion.TipoPosicion.Codigo.ToLower())
                //ITEM_CAT PSTYP   Tipo de posición del documento de compras
                {
                    case "servicio":
                        IM_PRITEM.ITEM_CAT = "9";
                        break;

                    case "materiales":
                    default:
                        IM_PRITEM.ITEM_CAT = "0";
                        break;
                }

                //Indica el tipo de imputacion de la solp
                if (posicion.TipoImputacion != null)
                {
                    switch (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower())
                    //ACCTASSCAT  KNTTP Tipo de imputación
                    {
                        case "centrodecosto":
                            IM_PRITEM.ACCTASSCAT = "K";
                            break;
                        case "ordendeot":
                            IM_PRITEM.ACCTASSCAT = "F";
                            break;
                        case "ordendeinversion":
                            IM_PRITEM.ACCTASSCAT = "F";
                            break;
                        case "siniestrobeneficio":
                            IM_PRITEM.ACCTASSCAT = "Y";
                            break;
                    }
                }
                else
                {
                    IM_PRITEM.ACCTASSCAT = "";
                }

                //IM_PRITEM.DES_VENDOR = null; //DES_VENDOR WLIEF   Proveedor deseado
                //Contrato marco          
                IM_PRITEM.FIXED_VEND = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp != null ? posicion.ProveedorAdjudicado.ObtenerCodigoProveedor() : ""; //FIXED_VEND FLIEF   Proveedor fijo
                IM_PRITEM.DES_VENDOR = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp == null ? posicion.ProveedorAdjudicado.ObtenerCodigoProveedor() : "";
                IM_PRITEM.PURCH_ORG = posicion.OrganizacionDeComprasCodigo; //PURCH_ORG EKORG   Organización de compras
                IM_PRITEM.AGREEMENT = posicion.NumeroContratoSuperior; //AGREEMENT   KONNR Número del contrato superior
                IM_PRITEM.AGMT_ITEM = posicion.NumeroPosicionContratoSuperior; //AGMT_ITEM   KTPNR Número de posición del contrato superior
                IM_PRITEM.INFO_REC = posicion.RegistroInfoNro; //INFO_REC    INFNR Número del registro info de compras
                IM_PRITEM.CLOSED = null; //Contrato marco? No está en este MVP //CLOSED  EBAKZ Solicitud de pedido concluida
                IM_PRITEM.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY    WAERS Clave de moneda
                IM_PRITEM.CURRENCY_ISO = null; //CURRENCY_ISO BAPIISOCD   Código ISO para moneda
                                               //IM_PRITEM.INFO_REC = null; //INFO_REC    INFNR Número del registro info de compras                            
                IM_PRITEM.PLND_DELRY = (decimal)posicion.PlazoEntrega; //PLND_DELRY PLIFZ   Plazo de entrega previsto en días
                //IM_PRITEM.PLND_DELRYSpecified = true;
                IM_PRITEM.PCKG_NO = numeroPosicion.AsNumeroPaquete(); //PCKG_NO PACKNO  Nº paquete

                //Indica si esta borrada la posicion 
                if (!string.IsNullOrEmpty(solpActual.NroSolp))
                {
                    IM_PRITEM.DELETE_IND = SAPFormatter.FormatearBooleano(eliminarPosicion);
                }

                //Agregamos todos los items a la estructura de posicion
                solpSAP.IM_PRITEMList.Add(IM_PRITEM);

                //Esta es una lista de campos que SAP nos pide que enviemos una "X" con los datos.
                solpSAP.IM_PRITEMXList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES8000
                {
                    PREQ_ITEM = numeroPosicion.AsPreqItem(),
                    PREQ_ITEMX = "X",
                    PUR_GROUP = "X",
                    CREATED_BY = "X",
                    PREQ_NAME = "X",
                    SHORT_TEXT = "X",
                    MATERIAL = (IM_PRITEM.MATL_GROUP != null) ? "X" : "",
                    PLANT = "X",
                    STORE_LOC = "X",
                    TRACKINGNO = "X",
                    MATL_GROUP = "X",
                    QUANTITY = (IM_PRITEM.QUANTITY == 0) ? "" : "X",
                    UNIT = "X",
                    //PREQ_UNIT_ISO = "X",
                    PREQ_DATE = "X",
                    DELIV_DATE = "X",
                    //REL_DATE = "X",
                    //GR_PR_TIME = "X",
                    PREQ_PRICE = (IM_PRITEM.PREQ_PRICE == 0) ? "" : "X",
                    //PRICE_UNIT = "X"
                    ITEM_CAT = "X",
                    ACCTASSCAT = "X",
                    DES_VENDOR = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp == null ? "X" : "",
                    FIXED_VEND = posicion.ProveedorAdjudicado_Id != null && posicion.MaterialSolp != null ? "X" : "",
                    PURCH_ORG = !string.IsNullOrEmpty(posicion.OrganizacionDeComprasCodigo) ? "X" : "",
                    AGREEMENT = !string.IsNullOrEmpty(posicion.NumeroContratoSuperior) ? "X" : "",
                    AGMT_ITEM = !string.IsNullOrEmpty(posicion.NumeroPosicionContratoSuperior) ? "X" : "",
                    INFO_REC = !string.IsNullOrEmpty(posicion.RegistroInfoNro) ? "X" : "",
                    //CLOSED = "X",
                    CURRENCY = "X",
                    //CURRENCY_ISO = "X",
                    PLND_DELRY = "X",
                    PCKG_NO = "X",
                    DELETE_IND = string.IsNullOrEmpty(solpActual.NroSolp) ? "" : "X",
                });


                //---Desde aca empiezan las subposiciones---

                var numeroSerialNumberItem = 0;

                foreach (var subPosicion in posicion.Subposiciones.OrderBy(x => x.Id))
                {
                    NumeroSubPosicion numeroSubPosicion = subPosicion.Numero;

                    //SUBPOSICION
                    var IM_SERVICELINE = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINE();

                    IM_SERVICELINE.DOC_ITEM = numeroPosicion.AsDocItem(); //DOC_ITEM EBELP   Número de posición de la solicitud de pedido = PREQ_ITEM
                    IM_SERVICELINE.OUTLINE = OUTLINE_NUMBER; //OUTLINE OUTLINE_NO  Número de estructuración
                    IM_SERVICELINE.SRV_LINE = numeroSubPosicion.AsServiceLineNumber(); //SRV_LINE    EXTROW Número de línea
                    IM_SERVICELINE.DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)); //DEL_IND DEL Indicador de borrado

                    if (subPosicion.ServicioSolp != null)
                        IM_SERVICELINE.SERVICE = subPosicion.ServicioSolp.Codigo.ToString(); //SERVICE ASNUM Número de servicio
                    else
                        IM_SERVICELINE.SHORT_TEXT = subPosicion.Tarea; //SHORT_TEXT SH_TEXT1 Texto breve

                    IM_SERVICELINE.QUANTITY = subPosicion.Cantidad.Value; //QUANTITY MENGEV  Cantidad con signo +/ -
                    //IM_SERVICELINE.QUANTITYSpecified = true;
                    IM_SERVICELINE.UOM = unidadesMedidaSap?.Find(u => u.Item1 == subPosicion.Unidad.CodigoSap).Item2; //UOM MEINS - Cambia el código de la unidad solicitada por su equivalente 'UM' de la tabla UnidadMedidaSap
                    //IM_SERVICELINE.UOM_ISO = null; //UOM_ISO MEINS_ISO   Unidad medida base en código ISO
                    IM_SERVICELINE.GROSS_PRICE = Math.Round(subPosicion.PrecioBruto.Value,4); //GROSS_PRICE SBRTWR Precio bruto Unitario
                    //IM_SERVICELINE.GROSS_PRICESpecified = true;
                    IM_SERVICELINE.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY WAERS   Clave de moneda
                    IM_SERVICELINE.HR_START_TIME = string.Empty;
                    IM_SERVICELINE.HR_END_TIME = string.Empty;
                    solpSAP.IM_SERVICELINESList.Add(IM_SERVICELINE);

                    solpSAP.IM_SERVICELINESXList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_SERVICE_LINEX
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SRV_LINE = numeroSubPosicion.AsServiceLineNumber(),
                        DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)),
                        SERVICE = "X",
                        SHORT_TEXT = (subPosicion.ServicioSolp == null) ? "X" : "",
                        QUANTITY = "X",
                        UOM = "X",
                        //UOM_ISO = "X",
                        GROSS_PRICE = "X",
                        CURRENCY = "X"
                    });

                    SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT imputacionPosicion = solpSAP.IM_PRACCOUNTList.Find(x =>
                            x.PREQ_ITEM == numeroPosicion.AsPreqItem() && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) && //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
                            x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
                            x.PROFIT_CTR == getCodigoTablaSap(subPosicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
                        );

                    if (imputacionPosicion is null)
                    {
                        numeroSerialNumberItem++;

                        imputacionPosicion = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNT
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(), //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                            SERIAL_NO = $"{numeroSerialNumberItem:00}",//indiceImputacion, //SERIAL_NO    DZEKKN  Número actual de la imputación
                            QUANTITY = subPosicion.Cantidad.Value, //QUANTITY	MENGE_D	Cantidad
                            GL_ACCOUNT = getCodigoTablaSap(subPosicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                            COSTCENTER = getCodigoTablaSap(subPosicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
                            ORDERID = getCodigoTablaSap(subPosicion.TipoImputacionSap), //ORDERID	AUFNR	Número de orden
                            PROFIT_CTR = getCodigoTablaSap(subPosicion.TipoImputacionSap), //PROFIT_CTR	PRCTR	Centro de beneficio
                            BUS_AREA = "GENE"
                        };

                        solpSAP.IM_PRACCOUNTList.Add(imputacionPosicion);

                        solpSAP.IM_PRACCOUNTXList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQACCOUNTX
                        {
                            PREQ_ITEM = numeroPosicion.AsPreqItem(),
                            SERIAL_NO = imputacionPosicion.SERIAL_NO,
                            PREQ_ITEMX = "X",
                            SERIAL_NOX = "X",
                            QUANTITY = "X",
                            GL_ACCOUNT = "X",
                            BUS_AREA = "X",
                            COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                            ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                            PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : ""
                        });
                    }

                    //IMPUTACION SUBPOSICION
                    solpSAP.IM_SERVICEACCOUNTList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATA
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER,
                        SRV_LINE = numeroSubPosicion.AsServiceLineNumber(),
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = imputacionPosicion.SERIAL_NO,
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = numeroPosicion.AsDocItem(),
                        OUTLINE = OUTLINE_NUMBER, //Preguntar a Ulises
                        SRV_LINE = numeroSubPosicion.AsServiceLineNumber(), //Preguntar a Ulises
                        SERIAL_NO = SERVICE_ACCOUNT_SERIAL_NUMBER,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });
                }

                //Estos son los metodos con los que nos fijamos si se editó algun campo de la direccion de entrega. Si no editó ninguno no 
                //hace falta enviar a SAP, pero si se editó por lo menos uno tenemos que enviar todos los campos 
                CentroDireccion centroPorDefecto = centroDireccionService.GetCentroDireccionByCodigoSap(posicion.Centro.CodigoSap);
                TablaSap centroPorDefecto2 = tablaSapService.GetById(posicion.Centro_Id);

                if (centroPorDefecto2.Descripcion != posicion.NombreEntrega ||
                    centroPorDefecto.Cp != posicion.CpEntrega ||
                    centroPorDefecto2.Descripcion != posicion.Centro.Descripcion ||
                    centroPorDefecto.Direccion != posicion.CalleEntrega ||
                    centroPorDefecto.Numero != posicion.NumeroEntrega)
                {
                    solpSAP.IM_PRADDRDELIVERYList.Add(
                    new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7110
                    {
                        PREQ_NO = numeroPosicion.AsPreqItem(), //PREQ_NO BANFN   Numero de SOLPED
                        PREQ_ITEM = numeroPosicion.AsPreqItem(), //PREQ_ITEM   BNFPO Número de posición de la solicitud de pedido
                        NAME = posicion.NombreEntrega, //NAME    AD_NAME1 Nombre 1
                        POSTL_COD1 = posicion.CpEntrega, //POSTL_COD1 AD_PSTCD1   Código postal de la población
                        CITY = posicion.Centro.Descripcion, //CITY    AD_CITY1 Población
                        STREET = posicion.CalleEntrega, //STREET AD_STREET   Calle
                        TEL1_NUMBR = posicion.NumeroEntrega, //TEL1_NUMBR  AD_TLNMBR1 Primer número teléfono: Prefijo + número
                    });
                }
            }

            //Este metodo lo usamos para enviar el texto de observaciones. Solo se pueden enviar 132 caracteres por linea

            var textosObservacion = "";

            //Enviar observaciones del paso 2 o las observaciones de las condiciones especiales segun prioridad
            if (!string.IsNullOrEmpty(solpActual.Pliego.ObservacionesGeneracion))
            {
                // Si ObservacionesGeneracion tiene algún valor, se envía este.
                textosObservacion = solpActual.Pliego.ObservacionesGeneracion;
            }
            else if (!string.IsNullOrEmpty(solpActual.Pliego.ObservacionesCotizacionCondEsp))
            {
                // Si ObservacionesGeneracion está vacío y ObservacionesCotizacionCondEsp tiene algún valor, se envía este.
                textosObservacion = solpActual.Pliego.ObservacionesCotizacionCondEsp;
            }

            var linesObservacion = getLinesFromTextoSuministro(textosObservacion);

            linesObservacion.ForEach(texto =>
            {
                solpSAP.IM_PRHEADERTEXTList.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEREQHEADTEXT
                {
                    PREQ_ITEM = "00000",
                    TEXT_ID = "B01",
                    TEXT_FORM = "*",
                    TEXT_LINE = texto,
                });
            });
            #endregion

            return solpSAP;
        }


        public RespuestaCrearOrdenDeCompra CrearOrdenDeCompra(Adjudicacion AdjudicacionEntity, bool creadoAutomatico = false)
        {
            var respuesta = new RespuestaCrearOrdenDeCompra();
            respuesta.Errores = new List<string>();
            CrearPedidoConsumerMOAResponse resultadoCrearPedido;
            if (string.IsNullOrEmpty(AdjudicacionEntity.Usuario.OrganizacionDeCompra))
            {
                respuesta.Errores.Add("El usuario creador no tiene una organización de compra registrada en su perfil. Comunicarse con sistemas para agregarla.");
                return respuesta;
            }

            if (AdjudicacionEntity.Posiciones.FirstOrDefault().Posicion.Solp.Adicional == true)
            {
                resultadoCrearPedido = modificarOrdenDeCompraConsumerMOA.Request(AdjudicacionEntity);
            }
            else
            {
                resultadoCrearPedido = crearPedidoConsumerMOA.Request(AdjudicacionEntity, creadoAutomatico);
            }

            respuesta.NumeroPedido = resultadoCrearPedido.NumeroPedido;
            // respuesta.NumeroSolp = AdjudicacionEntity.Solp.NroSolp;
            foreach (var error in resultadoCrearPedido.Errores.Where(x => x.Tipo == "E"))
            {
                var mensaje = error.Mensaje.Trim();
                respuesta.Errores.Add(mensaje);
            }

            if (respuesta.Errores.Count == 0)
            {
                respuesta.Mensaje = "OK";
            }

            return respuesta;
        }

        public CrearSolpConsumerMOAResponse CrearSolpSap(SolpSAPDto solpSap)
        {
            return crearSolpConsumerMOA.Request(solpSap);
        }

        public CrearSolpConsumerMOAResponse CrearSolpSapSinPI(SolpSAPSinPIDto solpSAPSinPI)
        {
            return crearSolpConsumerMOA.RequestSinPI(solpSAPSinPI);
        }

        public ResultadoGenerico EditarOrdenDeCompra(AdjudicacionDto adjudicacionDto)
        {

            ResultadoGenerico resultadoEditarOC = new ResultadoGenerico();

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                resultadoEditarOC = ModificarOrdenDeCompraSinPI(adjudicacionDto);
            }
            else
            {
                resultadoEditarOC = ModificarOrdenDeCompra(adjudicacionDto);
            }

            //var adjudicacion = ConvertirAjudicacionDtoEnAdjudicacionSAP(adjudicacionDto);
            //ResultadoGenerico resultadoEditarOC = new ResultadoGenerico();
            ////adjudicacion = TestCompletarAdjudicacion(adjudicacion);
            ////Consulta de OC en SAP
            //var ocSap = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraRFC(adjudicacion.NumeroOrdenDeCompra);
            ////Convertir OC de SAP a ModificarPedidoSAP
            //ModificarPedidoSAP modificarPedidoSAP = new ModificarPedidoSAP
            //{
            //    POACCOUNT = ocSap.POACCOUNT.Select(a => new BAPIMEPOACCOUNT
            //    {
            //        PO_ITEM = a.PO_ITEM
            //    }).ToList(),
            //    POACCOUNTX = ocSap.POACCOUNT.Select(a => new BAPIMEPOACCOUNTX
            //    {
            //        PO_ITEM = a.PO_ITEM
            //    }).ToList(),
            //    POADDRDELIVERY = ocSap.POADDRDELIVERY.Select(a => new BAPIMEPOADDRDELIVERY
            //    {
            //        PO_ITEM = a.PO_ITEM,
            //        POSTL_COD1 = a.POSTL_COD1,
            //        CITY = a.CITY,
            //        ADDR_NO = "",
            //        NAME = a.NAME,
            //        TEL1_NUMBR = "",
            //        STREET = a.STREET,
            //        STREET_NO = "",
            //        REGION = a.REGION,
            //        COUNTRY = a.COUNTRY
            //    }).ToList(),
            //    POCOND = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new BAPIMEPOCOND
            //    {
            //        ITM_NUMBER = x.ITM_NUMBER,  //el número de ítem al que corresponda la condición
            //        COND_ST_NO = x.COND_ST_NO,
            //        COND_TYPE = x.COND_TYPE,
            //        COND_VALUE = x.COND_VALUE, //el importe de la condición
            //        COND_VALUESpecified = true,
            //        CURRENCY = x.CURRENCY,
            //        CHANGE_ID = "U",
            //        //COND_COUNT = x.COND_COUNT,

            //    }).ToList(),
            //    POCONDX = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new BAPIMEPOCONDX
            //    {
            //        ITM_NUMBER = x.ITM_NUMBER,
            //        ITM_NUMBERX = "X",
            //        COND_ST_NO = "001",
            //        COND_ST_NOX = "X",
            //        COND_TYPE = "X",
            //        COND_VALUE = "X",
            //        CURRENCY = "X",
            //        CHANGE_ID = "X",
            //        CONDITION_NOX = "X",
            //    }).ToList(),

            //    POHEADER = new BAPIMEPOHEADER(),
            //    POHEADERX = new BAPIMEPOHEADERX(),
            //    POITEM = ocSap.POITEM.Select(x => new BAPIMEPOITEM
            //    {
            //        PO_ITEM = x.PO_ITEM,
            //        PCKG_NO = x.PCKG_NO
            //    }).ToList(),
            //    POITEMX = ocSap.POITEM.Select(x => new BAPIMEPOITEMX
            //    {
            //        PO_ITEM = x.PO_ITEM,
            //    }).ToList(),
            //    POSCHEDULE = ocSap.POSCHEDULE.Select(x => new BAPIMEPOSCHEDULE { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = x.DELIVERY_DATE }).ToList(),
            //    POSCHEDULEX = ocSap.POSCHEDULE.Select(x => new BAPIMEPOSCHEDULX { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = "X" }).ToList(),
            //    POSERVICES = ConvertirLista<BAPIESLLC>(ocSap.POSERVICES.ToList()),
            //    //POSRVACCESSVALUES = ocSap.POSRVACCESSVALUES.Select(x => new BAPIESKLC
            //    //{
            //    //    PCKG_NO = x.PCKG_NO,
            //    //    LINE_NO = x.LINE_NO,
            //    //    PERCENTAGE = x.PERCENTAGE,
            //    //    SERNO_LINE = x.SERNO_LINE,
            //    //    SERIAL_NO = x.SERIAL_NO,
            //    //    QUANTITY = x.QUANTITY,
            //    //    NET_VALUE = x.NET_VALUE,
            //    //    NET_VALUESpecified = true,
            //    //    PERCENTAGESpecified = true,
            //    //    QUANTITYSpecified = true,
            //    //}).ToList(),
            //    POTEXTHEADER = new List<BAPIMEPOTEXTHEADER>(),
            //    PURCHASEORDER = adjudicacion.NumeroOrdenDeCompra,

            //};
            //foreach (var bAPIESLLC in modificarPedidoSAP.POSERVICES)
            //{
            //    bAPIESLLC.GR_PRICESpecified = true;
            //    bAPIESLLC.QUANTITYSpecified = true;
            //    bAPIESLLC.NET_VALUESpecified = true;
            //    bAPIESLLC.PRICE_UNITSpecified = true;
            //}

            ////Racional de compras
            //var listaVaciaTexto = new string[] { "" };
            //var textosDiccionario = new Dictionary<string, string[]>() {
            //    {"F01", !string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ?  adjudicacion.TextoDeCabecera.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            //    {"F05", !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega)? adjudicacion.CondicionesDeEntrega.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            //    {"F07", !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ? adjudicacion.CondicionesDePago.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            //    {"F08", !string.IsNullOrEmpty(adjudicacion.Garantias) ? adjudicacion.Garantias.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            //};
            //foreach (var grupos in textosDiccionario)
            //{
            //    bool todosVacios = grupos.Value.All(string.IsNullOrEmpty);
            //    if (!todosVacios)
            //    {
            //        foreach (var texto in grupos.Value)
            //        {
            //            modificarPedidoSAP.POTEXTHEADER.Add(new BAPIMEPOTEXTHEADER
            //            {
            //                TEXT_ID = grupos.Key,
            //                PO_NUMBER = "",
            //                PO_ITEM = "0",
            //                TEXT_FORM = "*",
            //                TEXT_LINE = texto
            //            });
            //        }
            //    }

            //}

            ////Condición de Pago  - no funcionan para las  ZPE1 y ZDIR por que sap no lo permite
            //List<string> condicionesNoEditables = new List<string>() { "ZPE1", "ZDIR" };
            //if (!condicionesNoEditables.Contains(ocSap.POHEADER.DOC_TYPE))
            //{
            //    modificarPedidoSAP.POHEADER.PMNTTRMS = adjudicacion.CondicionDePagoCodigo;
            //    modificarPedidoSAP.POHEADER.DSCNT1_TO = adjudicacion.PagoEn1;
            //    modificarPedidoSAP.POHEADER.DSCNT1_TOSpecified = true;
            //    modificarPedidoSAP.POHEADER.DSCNT2_TO = adjudicacion.PagoEn2;
            //    modificarPedidoSAP.POHEADER.DSCNT2_TOSpecified = true;
            //    modificarPedidoSAP.POHEADER.DSCNT3_TO = adjudicacion.PagoEn3;
            //    modificarPedidoSAP.POHEADER.DSCNT3_TOSpecified = true;
            //    modificarPedidoSAP.POHEADER.DSCT_PCT1 = adjudicacion.PagoEn1Porcentaje;
            //    modificarPedidoSAP.POHEADER.DSCT_PCT1Specified = true;
            //    modificarPedidoSAP.POHEADER.DSCT_PCT2 = adjudicacion.PagoEn2Porcentaje;
            //    modificarPedidoSAP.POHEADER.DSCT_PCT2Specified = true;
            //    modificarPedidoSAP.POHEADERX.PMNTTRMS = "X";
            //    modificarPedidoSAP.POHEADERX.DSCNT1_TO = "X";
            //    modificarPedidoSAP.POHEADERX.DSCNT2_TO = "X";
            //    modificarPedidoSAP.POHEADERX.DSCNT3_TO = "X";
            //    modificarPedidoSAP.POHEADERX.DSCT_PCT1 = "X";
            //    modificarPedidoSAP.POHEADERX.DSCT_PCT2 = "X";
            //}
            //else
            //{
            //    if (HayModificacionCondicionesDePago(ocSap.POHEADER, adjudicacion))// TODO: validar si edito alguna condicion de pago.
            //    {
            //        resultadoEditarOC.Errores.Add(new ErrorMessage("Condición de Pago no se puede editar para las clase de documento  ZPE1 y ZDIR."));
            //        return resultadoEditarOC;
            //    }
            //}


            ////Condición de Importacion
            //if (!string.IsNullOrEmpty(adjudicacion.CondicionDeImportacionComplemento) && adjudicacion.CondicionDeImportacionComplemento.Length > 28)
            //{
            //    resultadoEditarOC.Errores.Add(new ErrorMessage("Condicion de importacion muy largo, 28 caracteres maximo."));
            //    return resultadoEditarOC;
            //}
            //modificarPedidoSAP.POHEADER.INCOTERMS1 = adjudicacion.CondicionDeImportacionCodigo;
            //modificarPedidoSAP.POHEADER.INCOTERMS2 = adjudicacion.CondicionDeImportacionComplemento;
            //modificarPedidoSAP.POHEADERX.INCOTERMS1 = "X";
            //modificarPedidoSAP.POHEADERX.INCOTERMS2 = "X";

            ////Datos utiles
            //bool esMateriales = ocSap.POITEM[0].ITEM_CAT == "0";
            //bool modificoMoneda = ocSap.POHEADER.CURRENCY != adjudicacion.MonedaCodigo;

            ////Modificar moneda
            //if (modificoMoneda)
            //{
            //    modificarPedidoSAP.POHEADER.CURRENCY = adjudicacion.MonedaCodigo;
            //    modificarPedidoSAP.POHEADERX.CURRENCY = "X";
            //}

            //foreach (var posAdj in adjudicacion.Posiciones)
            //{
            //    string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');

            //    //Región
            //    var direccioSap = modificarPedidoSAP.POADDRDELIVERY.Single(a => a.PO_ITEM == PO_ITEM);
            //    direccioSap.REGION = posAdj.RegionCodigo;
            //    direccioSap.COUNTRY = posAdj.PaisCodigo;

            //    //Fechas de entrega(por posición)
            //    var fechaEntregaSap = modificarPedidoSAP.POSCHEDULE.Single(a => a.PO_ITEM == PO_ITEM);
            //    var fechaEntregaSapX = modificarPedidoSAP.POSCHEDULEX.Single(a => a.PO_ITEM == PO_ITEM);
            //    fechaEntregaSap.DELIVERY_DATE = posAdj.FechaEntrega.ToString("dd.MM.yyyy");
            //    fechaEntregaSapX.DELIVERY_DATE = "X";

            //    //Posición
            //    var posicionSap = modificarPedidoSAP.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
            //    var posicionSapX = modificarPedidoSAP.POITEMX.Single(a => a.PO_ITEM == PO_ITEM);
            //    //Impitación
            //    var imputacionSap = new BAPIMEPOACCOUNT();
            //    var imputacionSapX = new BAPIMEPOACCOUNTX();
            //    if (modificarPedidoSAP.POACCOUNT.Count > 0)
            //    {
            //        imputacionSap = modificarPedidoSAP.POACCOUNT.FirstOrDefault(a => a.PO_ITEM == PO_ITEM);
            //        imputacionSapX = modificarPedidoSAP.POACCOUNTX.FirstOrDefault(a => a.PO_ITEM == PO_ITEM);
            //    }
            //    //Condición
            //    var condicionSap = modificarPedidoSAP.POCOND.Single(a => a.ITM_NUMBER == "0" + PO_ITEM);

            //    //Eliminar posición
            //    posicionSap.DELETE_IND = posAdj.Eliminado ? "X" : "";
            //    posicionSapX.DELETE_IND = "X";

            //    //Tilde entrega final
            //    posicionSap.NO_MORE_GR = posAdj.EntregaFinal ? "X" : "";
            //    posicionSapX.NO_MORE_GR = "X";

            //    if (esMateriales)
            //    {
            //        bool modificoImporte = ModificoImporte(ocSap, adjudicacion);

            //        //Modificar cantidad
            //        posicionSap.QUANTITY = posAdj.Cantidad;
            //        posicionSap.QUANTITYSpecified = true;
            //        posicionSapX.QUANTITY = "X";
            //        if (modificarPedidoSAP.POACCOUNT.Count > 0)
            //        {
            //            imputacionSap.QUANTITY = posAdj.Cantidad;
            //            imputacionSapX.QUANTITY = "X";
            //        }

            //        //Modificar importe
            //        if (modificoImporte)
            //        {
            //            posicionSap.NET_PRICE = posAdj.PrecioUnidadCodigo;
            //            posicionSap.NET_PRICESpecified = true;
            //            posicionSapX.NET_PRICE = "X";
            //            condicionSap.COND_VALUE = posAdj.PrecioUnidadCodigo;
            //            condicionSap.COND_VALUESpecified = true;
            //            condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
            //            //"ZP01" no deja cambiar importes por eso se cambia a "ZP00"
            //            condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP00" : condicionSap.COND_TYPE;
            //        }
            //    }
            //    else
            //    {

            //        if (!posAdj.Eliminado)
            //        {
            //            foreach (var subPosAdj in posAdj.SubPosiciones)
            //            {
            //                //Subposición
            //                string LINE_NO = subPosAdj.Indice.ToString().PadLeft(10, '0');
            //                var subPosicionSap = modificarPedidoSAP.POSERVICES.First(a => a.LINE_NO == LINE_NO);
            //                //var imputacionSubPos = modificarPedidoSAP.POSRVACCESSVALUES.First(a => a.LINE_NO == LINE_NO);//

            //                //Eliminar subposición 
            //                subPosicionSap.DELETE_IND = subPosAdj.Eliminado ? "X" : "";
            //                if (subPosAdj.Eliminado) continue;
            //                //Cantidad
            //                subPosicionSap.QUANTITY = subPosAdj.Cantidad;
            //                //imputacionSubPos.QUANTITY = subPosAdj.Cantidad;
            //                //Importe 1/2                            
            //                subPosicionSap.GR_PRICE = subPosAdj.PrecioUnitario;
            //                subPosicionSap.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;
            //                //imputacionSubPos.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;

            //            }

            //            //Importe 2/2                        
            //            var totalPosicion = posAdj.SubPosiciones.Where(a => !a.Eliminado).Sum(a => a.Cantidad * a.PrecioUnitario);
            //            condicionSap.COND_VALUE = totalPosicion;
            //            condicionSap.COND_VALUESpecified = true;
            //            condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
            //            condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP01" : condicionSap.COND_TYPE;

            //            posicionSap.NET_PRICE = totalPosicion;
            //            posicionSap.NET_PRICESpecified = true;
            //            posicionSapX.NET_PRICE = "X";
            //            //imputacionSap.NET_VALUE = totalPosicion;
            //            //imputacionSap.NET_VALUESpecified = true;
            //            //imputacionSapX.NET_VALUE = "X";

            //        }

            //    }
            //}

            //if (!esMateriales) // para servicios guille nos dijo que no lo enviemos pero para materiales si lo necesitamos enviar.
            //{
            //    modificarPedidoSAP.POACCOUNT = new List<BAPIMEPOACCOUNT>();
            //    modificarPedidoSAP.POACCOUNTX = new List<BAPIMEPOACCOUNTX>();
            //}

            //if (modificoMoneda)
            //{
            //    modificarPedidoSAP.POITEM = new List<BAPIMEPOITEM>();
            //    modificarPedidoSAP.POITEMX = new List<BAPIMEPOITEMX>();

            //    modificarPedidoSAP.POACCOUNT = new List<BAPIMEPOACCOUNT>();
            //    modificarPedidoSAP.POACCOUNTX = new List<BAPIMEPOACCOUNTX>();

            //    modificarPedidoSAP.POSERVICES = new List<BAPIESLLC>();

            //    modificarPedidoSAP.POCOND = new List<BAPIMEPOCOND>();
            //    modificarPedidoSAP.POCONDX = new List<BAPIMEPOCONDX>();

            //    modificarPedidoSAP.POADDRDELIVERY = new List<BAPIMEPOADDRDELIVERY>();
            //    modificarPedidoSAP.POSCHEDULE = new List<BAPIMEPOSCHEDULE>();
            //    modificarPedidoSAP.POSCHEDULEX = new List<BAPIMEPOSCHEDULX>();

            //}


            //var resultadoSAP = modificarOrdenDeCompraConsumerMOA.EditarPedidoRequest(modificarPedidoSAP);
            //resultadoSAP.Where(a => a.MESSAGE == "No se han modificado datos").ToList().ForEach(a => a.TYPE = "E");

            //foreach (var item in resultadoSAP.Where(x => x.TYPE == "E"))
            //{
            //    resultadoEditarOC.Error(item.TYPE, item.MESSAGE);
            //}

            return resultadoEditarOC;
        }

        public ModificarSolpConsumerMOAResponse ModificarSolpSap(SolpSAPDto solpSap)
        {
            return modificarSolpConsumerMOA.Request(solpSap);
        }

        public ModificarSolpConsumerMOAResponse ModificarSolpSapSinPI(SolpSAPSinPIDto solpSap)
        {
            return modificarSolpConsumerMOA.RequestSinPI(solpSap);
        }

        public OrdenDeCompraSAPDto ObtenerOrdenDeCompra(string nroOC)
        {
            return obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompra(nroOC);
        }

        public AdjudicacionDto ObtenerOrdenDeCompraAdjudicacion(string nroOc)
        {
            return obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraAdjudicacion(nroOc);
        }

        //Fuente de aprovisionamiento es donde consultamos cuando ponemos un numero de material y asociamos un contrato
        public List<FuenteAprovisionamientoDto> ListarFuenteAprovisionamiento(string fechaEntregaPosicion, string numeroMaterial, string centro)
        {
            var result = obtenerFuenteAprovisionamientoConsumerMOA.request(fechaEntregaPosicion, numeroMaterial, centro);
            return result.ContratosAprovisionamiento.ConvertAll(item => new FuenteAprovisionamientoDto
            {
                ProveedorFijo = item.ProveedorFijo,
                NombreProveedor = item.NombreProveedor,

                CentroAprovisionamiento = item.CentroAprovisionamiento,
                NumeroContratoSuperior = item.NumeroContratoSuperior,
                NumeroPosicionContratoSuperior = item.NumeroPosicionContratoSuperior,
                NumeroRegistroInfoCompras = item.NumeroRegistroInfoCompras,
                TipoDocumentoCompras = item.TipoDocumentoCompras,
                OrganizacionCompras = item.OrganizacionCompras,
                UnidadMedida = item.UnidadMedida,
                TipoPosicionDocumento = item.TipoPosicionDocumento,
                NumeroMaterial = item.NumeroMaterial,
                TipoPosicionDocumentoCompras = item.TipoPosicionDocumentoCompras
            });
        }

        //Obtener contrato es lo que consultamos cuando vamos a crear una posicion desde contrato marco
        public List<ContratoSolp> ObtenerContratoMarco(string numeroContrato, string centro)
        {
            var result = obtenerContratoSolpConsumerMOA.Request(numeroContrato, centro);
            return result.ContratosSolp;
        }

        public AdjudicacionDto ObtenerAdjudicacion(string nroOC)
        {
            AdjudicacionDto ordenDeCompraSAP = ObtenerOrdenDeCompraAdjudicacion(nroOC);

            var monedaPesos = tablaSapService.Obtener(a => a.Tabla == "Moneda" && a.CodigoSap == "ARP");
            if (ordenDeCompraSAP.Moneda_Id != monedaPesos.Id)
            {
                var monedaAdjudicacion = tablaSapService.Obtener(a => a.Tabla == "Moneda" && a.Id == ordenDeCompraSAP.Moneda_Id);
                var tipoCambio = tipoCambioService.ObtenerTipoCambio(monedaAdjudicacion.Id, monedaPesos.Id, ordenDeCompraSAP.FechaCreacion);
                ordenDeCompraSAP.PrecioFinal = ordenDeCompraSAP.PrecioFinal * tipoCambio.TipoCambio;
            }


            if (ordenDeCompraSAP.TipoPosicionCodigo == "MATERIALES")
            {
                var todasLasUM = ObtenerTablaSap(TablasSap.Unidad);
                var unidadesDeMedidaSAP = unidadMedidaService.ObtenerUnidadesDesdeServicioSap(ordenDeCompraSAP.AdjudicacionPosiciones.Where(x => x.MaterialComprasCodigo != null).Select(x => x.MaterialComprasCodigo).ToList());
                foreach (var posicion in ordenDeCompraSAP.AdjudicacionPosiciones)
                {
                    posicion.UnidadMedida = new TablaSapDto
                    {
                        Codigo = posicion.UnidadCodigo,
                        Id = posicion.UnidadId
                    };
                    if (!string.IsNullOrEmpty(posicion.MaterialComprasCodigo))
                    {
                        var unidadesPorMaterialSAP = unidadesDeMedidaSAP.Where(x => x.CodigoMaterial == posicion.MaterialComprasCodigo).Select(x => x.UnidadDeMedida).ToList();
                        posicion.UnidadesDeMedida = todasLasUM.Where(x => unidadesPorMaterialSAP.Contains(x.Codigo)).ToList();
                    }
                    else
                    {
                        posicion.UnidadesDeMedida = todasLasUM.Where(x => x.Id == posicion.UnidadId).ToList();
                    }
                }
            }

            return ordenDeCompraSAP;
        }

        public ObtenerSolpSAPResponse ObtenerSolpSap(ObtenerSolpRequest obtenerSolpRequest)
        {
            return obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(obtenerSolpRequest);
        }

        public List<TablaSapDto> ObtenerCentrosDeCostoSap()
        {
            CecoWSMOAResponse resultSap = (CecoWSMOAResponse)CentroDeCostoSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Cecos.ConvertAll(c => new TablaSapDto()
            {
                Tabla = TablasSap.CecoSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.CostCenter, out codigoNum) ? codigoNum.ToString() : c.CostCenter,
                Codigo = c.CostCenter
            });
        }

        public List<TablaSapDto> ObtenerCuentasSap()
        {
            CuentaWSMOAResponse resultSap = (CuentaWSMOAResponse)cuentasSolpConsumerMOA.request();
            var codigoNum = 0;

            return resultSap.Cuentas.ConvertAll(c => new TablaSapDto()
            {
                Tabla = TablasSap.CuentasSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.Codigo, out codigoNum) ? codigoNum.ToString() : c.Codigo,
                Codigo = c.Codigo
            });
        }

        public List<TablaSapDto> ObtenerOrdenesSap(string idOrder = "")
        {
            OrdenWSMOAResponse resultSap = (OrdenWSMOAResponse)ordenesSolpConsumerMOA.request(idOrder);
            var codigoNum = 0;

            return resultSap.Ordenes.ConvertAll(c => new TablaSapDto()
            {
                Tabla = TablasSap.OrdenSolpSap,
                Descripcion = c.Descripcion,
                CodigoSap = int.TryParse(c.Codigo, out codigoNum) ? codigoNum.ToString() : c.Codigo,
                Codigo = c.Codigo
            });
        }

        public List<OrdenDeCompraSAPDto> ObtenerReporteOrdenDeCompra(string nroOC, string fechaDesde, string fechasHasta, string codigoProveedor)
        {
            var result = reporteOrdenDeCompraConsumerMOA.Request(nroOC, fechaDesde, codigoProveedor);

            var fechaHastaDate = string.IsNullOrEmpty(fechasHasta) ? DateTime.Now : DateTime.Parse(fechasHasta);

            result = result.OrderByDescending(x => x.Cabecera.FechaCreacion).ToList();

            result = result
              .Where(x => x.Cabecera == null || (x.Cabecera.FechaCreacion <= fechaHastaDate))
              .ToList();

            return result;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(string numeroSolp)
        {
            return ObtenerPosiciones(numeroSolp).Where(PosicionPendienteSap);
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<string> numerosSolp)
        {
            ConcurrentQueue<PosicionSolpSAP> result = new ConcurrentQueue<PosicionSolpSAP>();
            numerosSolp
                .Distinct()
                .AsParallel()
                .ForAll(numeroSolp =>
                    ObtenerPosicionesPendientesAdjudicar(numeroSolp)
                    .AsParallel()
                    .ForAll(posicionPendiente =>
                        result.Enqueue(posicionPendiente)
                    )
                );
            return result;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosicionesPendientesAdjudicar(IEnumerable<PosicionSolpSAP> posicionesSap)
            => posicionesSap.Where(PosicionPendienteSap);

        public bool PosicionPendienteSap(PosicionSolpSAP position)
        {
            if (position is null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            return position.Ordered < position.Cantidad;
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosiciones(string numeroSolp)
        {
            ObtenerSolpRequest request = new ObtenerSolpRequest()
            {
                NumeroSolp = numeroSolp,
                FechaDesde = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                FechaHasta = DateTime.Today.AddDays(1),
            };

            try
            {
                ObtenerSolpSAPResponse response = obtenerSolpConsumerMOA.RequestSolpWithNroAndDates(request);

                return response.Posiciones;
            }
            catch (Exception ex)
            {
                Logger.Log.ExternalAPIError(ex);
                throw;
            }
        }

        public IEnumerable<PosicionSolpSAP> ObtenerPosiciones(IEnumerable<string> numerosSolp)
        {
            ConcurrentQueue<PosicionSolpSAP> result = new ConcurrentQueue<PosicionSolpSAP>();
            numerosSolp
                .Distinct()
                .AsParallel()
                .ForAll(numeroSolp =>
                    ObtenerPosiciones(numeroSolp)
                    .AsParallel()
                    .ForAll(posicionPendiente =>
                        result.Enqueue(posicionPendiente)
                    )
                );
            return result;
        }

        public List<TablaSapDto> ObtenerServiciosSap()
        {
            List<Servicio> servicios = ObtenerServiciosSapRaw();
            var codigoNum = 0;

            return servicios.ConvertAll(s => new TablaSapDto()
            {
                Tabla = TablasSap.CodigoServicioSap,
                Descripcion = s.Descripcion,
                CodigoSap = int.TryParse(s.Codigo, out codigoNum) ? codigoNum.ToString() : s.Codigo,
                Codigo = s.Codigo
            });
        }

        public List<Servicio> ObtenerServiciosSapRaw()
        {
            ServicioWSMOAResponse resultSap = (ServicioWSMOAResponse)serviciosSolpConsumerMOA.request();
            return resultSap.Servicios;
        }

        public List<TablaSapDto> ObtenerTablaSap(string tabla)
        {
            var tablaSap = tablaSapService.Listar(x => x.Tabla == tabla).ConvertAll(x => new TablaSapDto(x));
            if (tabla == TablasSap.EstadoSolpSap)
            {
                tablaSap.Add(new TablaSapDto { Id = -1, Descripcion = "Borrado en SAP" });
            }
            return tablaSap;
        }

        public List<PosicionPendienteDto> ListarSolpPendientes()
        {
            return listarSolpPendienteConsumeMOA.ListarSolpPendientes();
        }

        public IEnumerable<string> ListarNumeroSolpPendientes()
        {
            return ListarSolpPendientes().Select(solp => solp.NroSolp);
        }

        public byte[] ObtenerPDFOrdenCompra(string nroOc)
        {
            return obtenerPDFOrdenCompraConsumerMOA.Request(nroOc);
        }

        public byte[] TraerArchivosDeSAP(string docId)
        {
            return obtenerAdjuntosSOLPEDConsumerMOA.ObtenerAdjuntosSolpConsumer(docId, "");
        }

        public List<OrdenDeCompraSAPDto> ObtenerOrdenesCompraSapParaSolpPosicion(List<SolpPosicionDto> solpPosiciones)
        {
            var ordenesDeCompraSap = new List<OrdenDeCompraSAPDto>();
            foreach (var pos in solpPosiciones)
            {
                var ordenesSapResp = obtenerOrdenesDeCompraParaSOLPConsumerMOA.Request(pos.NroSolp, pos.Indice.ToString());
                ordenesDeCompraSap.AddRange(ordenesSapResp);
            }

            return ordenesDeCompraSap.Distinct().ToList();
        }

        private AdjudicacionEditarDto ConvertirAjudicacionDtoEnAdjudicacionSAP(AdjudicacionDto adjudicacionDto)
        {
            return new AdjudicacionEditarDto
            {
                NumeroOrdenDeCompra = adjudicacionDto.NumeroOrdenDeCompra,
                TextoDeCabecera = adjudicacionDto.TextoDeCabecera,
                CondicionesDeEntrega = adjudicacionDto.CondicionesDeEntrega,
                CondicionesDePago = adjudicacionDto.CondicionesDePago,
                Garantias = adjudicacionDto.Garantias,
                CondicionDePagoCodigo = adjudicacionDto.CondicionDePago.Codigo,
                PagoEn1 = adjudicacionDto.PagoEn1,
                PagoEn2 = adjudicacionDto.PagoEn2,
                PagoEn3 = adjudicacionDto.PagoEn3,
                PagoEn1Porcentaje = adjudicacionDto.PagoEn1Porcentaje,
                PagoEn2Porcentaje = adjudicacionDto.PagoEn2Porcentaje,
                CondicionDeImportacionCodigo = adjudicacionDto.CondicionDeImportacion.Codigo,
                CondicionDeImportacionComplemento = adjudicacionDto.CondicionDeImportacionDescripcion,
                MonedaCodigo = !string.IsNullOrEmpty(adjudicacionDto.MonedaCodigo) ? adjudicacionDto.MonedaCodigo : adjudicacionDto.AdjudicacionPosiciones.FirstOrDefault().MonedaCodigo,
                Posiciones = adjudicacionDto.AdjudicacionPosiciones.Select(posicionDto => new AdjudicacionPosicionEditarDto
                {
                    SubPosiciones = posicionDto.SubposicionesCompras?.Select(subPosicionDto => new AdjudicacionSubPosicionEditarDto
                    {
                        Indice = subPosicionDto.Numero,
                        Cantidad = subPosicionDto.Cantidad ?? 0,
                        Eliminado = subPosicionDto.Eliminado,
                        PrecioUnitario = subPosicionDto.PrecioBruto ?? 0
                    }).ToList(),
                    Indice = posicionDto.Indice ?? 0,
                    PrecioUnidadCodigo = posicionDto.PrecioUnidad ?? 0,
                    Eliminado = posicionDto.Eliminado,
                    RegionCodigo = posicionDto.RegionCodigo,
                    PaisCodigo = posicionDto.PaisSap,
                    FechaEntrega = posicionDto.FechaEntregaServicio.Value,
                    EntregaFinal = posicionDto.EntregaFinal,
                    Cantidad = posicionDto.Cantidad
                }).ToList()
            };
        }

        private List<TDestino> ConvertirLista<TDestino>(IEnumerable<object> listaOrigen)
           where TDestino : class, new()
        {
            // Utiliza reflexión para copiar propiedades automáticamente
            return listaOrigen.Select(item => ConvertirObjeto<TDestino>(item)).ToList();
        }

        private TDestino ConvertirObjeto<TDestino>(object objetoOrigen)
            where TDestino : class, new()
        {
            // Crea una instancia del tipo de destino
            var objetoDestino = Activator.CreateInstance<TDestino>();

            // Obtiene las propiedades de ambos tipos
            PropertyInfo[] propiedadesOrigen = objetoOrigen.GetType().GetProperties();
            PropertyInfo[] propiedadesDestino = typeof(TDestino).GetProperties();

            // Copia los valores de las propiedades automáticamente
            foreach (var propiedadOrigen in propiedadesOrigen)
            {
                PropertyInfo propiedadDestino = propiedadesDestino.FirstOrDefault(p => p.Name == propiedadOrigen.Name);

                if (propiedadDestino != null && propiedadDestino.PropertyType == propiedadOrigen.PropertyType)
                {
                    // Copia el valor de la propiedad del objeto de origen al objeto de destino
                    propiedadDestino.SetValue(objetoDestino, propiedadOrigen.GetValue(objetoOrigen));
                }
            }

            return objetoDestino;
        }

        private string getCodigoTablaSap(TablaSap imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }

        private string getCodigoTablaGeneral(TablaGeneral imputacion)
        {
            var result = "";

            if (imputacion != null)
            {
                result = imputacion.Codigo;
            }
            return result;
        }

        private List<string> getLinesFromTextoSuministro(string texto)
        {
            const int maxLengthPerLine = 132;
            var result = new List<string>();

            if (!string.IsNullOrEmpty(texto))
            {
                var line = string.Empty;

                while (texto.Length > maxLengthPerLine)
                {
                    line = texto.Substring(0, maxLengthPerLine - 1);
                    result.Add(line);
                    texto = texto.Substring(maxLengthPerLine - 1, texto.Length - (maxLengthPerLine - 1));
                }

                if (!string.IsNullOrEmpty(texto))
                {
                    result.Add(texto);
                }
            }
            return result;
        }

        private bool HayModificacionCondicionesDePago(SustitucionMOAWS.ObtenerOrdenDeCompraWebServiceMOA.BAPIMEPOHEADER original, AdjudicacionEditarDto adjudicacion)
        {
            return
                original.PMNTTRMS != adjudicacion.CondicionDePagoCodigo ||
                original.DSCNT1_TO != adjudicacion.PagoEn1 ||
                original.DSCNT2_TO != adjudicacion.PagoEn2 ||
                original.DSCNT3_TO != adjudicacion.PagoEn3 ||
                original.DSCT_PCT1 != adjudicacion.PagoEn1Porcentaje ||
                original.DSCT_PCT2 != adjudicacion.PagoEn2Porcentaje;
        }
        private bool HayModificacionCondicionesDePagoSinPI(SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER original, AdjudicacionEditarDto adjudicacion)
        {
            return
                original.PMNTTRMS != adjudicacion.CondicionDePagoCodigo ||
                original.DSCNT1_TO != adjudicacion.PagoEn1 ||
                original.DSCNT2_TO != adjudicacion.PagoEn2 ||
                original.DSCNT3_TO != adjudicacion.PagoEn3 ||
                original.DSCT_PCT1 != adjudicacion.PagoEn1Porcentaje ||
                original.DSCT_PCT2 != adjudicacion.PagoEn2Porcentaje;
        }
        private bool ModificoImporte(ResultBAPI_PO_GETDETAIL1 ocSap, AdjudicacionEditarDto adjudicacion)
        {
            foreach (var posAdj in adjudicacion.Posiciones.Where(a => !a.Eliminado))
            {
                string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');
                var posicionSap = ocSap.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
                var modifico = posAdj.PrecioUnidadCodigo != posicionSap.NET_PRICE;
                if (modifico) return true;

            }
            return false;
        }
        private bool ModificoImporteSinPI(ResultBAPI_PO_GETDETAIL1SinPI ocSap, AdjudicacionEditarDto adjudicacion)
        {
            foreach (var posAdj in adjudicacion.Posiciones.Where(a => !a.Eliminado))
            {
                string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');
                var posicionSap = ocSap.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
                var modifico = posAdj.PrecioUnidadCodigo != posicionSap.NET_PRICE;
                if (modifico) return true;

            }
            return false;
        }

        private ResultadoGenerico ModificarOrdenDeCompraSinPI(AdjudicacionDto adjudicacionDto) {
            var adjudicacion = ConvertirAjudicacionDtoEnAdjudicacionSAP(adjudicacionDto);
            var ocSap = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraRFCSinPI(adjudicacion.NumeroOrdenDeCompra);
            ResultadoGenerico resultadoEditarOC = new ResultadoGenerico();

            ModificarPedidoSAPSinPI modificarPedidoSAP = new ModificarPedidoSAPSinPI();

            var listPOACCOUNT = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT>();
            var listPOADDRDELIVERY = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY>();
            var listPOCOND = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND>();
            var listPOCONDX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX>();

            var listPOITEM      = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM>();
            var listPOITEMX     = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX   >();
            var listPOSCHEDULE  = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE>();
            var listPOSCHEDULEX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX>();
            var listPOSERVICES  = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX>();


            foreach (var item in ocSap.POACCOUNT)
            {
                listPOACCOUNT.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT()
                {
                    PO_ITEM = item.PO_ITEM
                });
            }
            modificarPedidoSAP.POACCOUNT = listPOACCOUNT.ToList();

            //POACCOUNTX = ocSap.POACCOUNT.Select(a => new BAPIMEPOACCOUNTX
            //{
            //    PO_ITEM = a.PO_ITEM
            //}).ToList(),
            
            foreach (var item in ocSap.POADDRDELIVERY)
            {
                listPOADDRDELIVERY.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY()
                {
                    PO_ITEM = item.PO_ITEM,
                    POSTL_COD1 = item.POSTL_COD1,
                    CITY = item.CITY,
                    ADDR_NO = "",
                    NAME = item.NAME,
                    TEL1_NUMBR = "",
                    STREET = item.STREET,
                    STREET_NO = "",
                    REGION = item.REGION,
                    COUNTRY = item.COUNTRY
                });
            }
            modificarPedidoSAP.POADDRDELIVERY = listPOADDRDELIVERY.ToList();


            //POADDRDELIVERY = ocSap.POADDRDELIVERY.Select(a => new WS_GAQ_sin_PI_DIRECT_2012.BAPIMEPOADDRDELIVERY
            //    {
            //        PO_ITEM = a.PO_ITEM,
            //        POSTL_COD1 = a.POSTL_COD1,
            //        CITY = a.CITY,
            //        ADDR_NO = "",
            //        NAME = a.NAME,
            //        TEL1_NUMBR = "",
            //        STREET = a.STREET,
            //        STREET_NO = "",
            //        REGION = a.REGION,
            //        COUNTRY = a.COUNTRY
            //    }).ToList(),
            foreach (var item in ocSap.POCOND)
            {
                listPOCOND.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND()
                {
                    ITM_NUMBER = item.ITM_NUMBER,  //el número de ítem al que corresponda la condición
                    COND_ST_NO = item.COND_ST_NO,
                    COND_TYPE = item.COND_TYPE,
                    COND_VALUE = item.COND_VALUE, //el importe de la condición
                    //COND_VALUESpecified = true,
                    CURRENCY = item.CURRENCY,
                    CHANGE_ID = "U",
                    //COND_COUNT = x.COND_COUNT,
                });
            }
            modificarPedidoSAP.POCOND = listPOCOND.ToList();

            //POCOND = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND
            //{
            //    ITM_NUMBER = x.ITM_NUMBER,  //el número de ítem al que corresponda la condición
            //    COND_ST_NO = x.COND_ST_NO,
            //    COND_TYPE = x.COND_TYPE,
            //    COND_VALUE = x.COND_VALUE, //el importe de la condición
            //    COND_VALUESpecified = true,
            //    CURRENCY = x.CURRENCY,
            //    CHANGE_ID = "U",
            //    //COND_COUNT = x.COND_COUNT,

            //}).ToList()
            foreach (var item in ocSap.POCOND)
            {
                listPOCONDX.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX()
                {
                    ITM_NUMBER = item.ITM_NUMBER,
                    ITM_NUMBERX = "X",
                    COND_ST_NO = "001",
                    COND_ST_NOX = "X",
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                    CONDITION_NOX = "X",
                });
            }
            modificarPedidoSAP.POCONDX = listPOCONDX.ToList();

            //POCONDX = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX
            //{
            //    ITM_NUMBER = x.ITM_NUMBER,
            //    ITM_NUMBERX = "X",
            //    COND_ST_NO = "001",
            //    COND_ST_NOX = "X",
            //    COND_TYPE = "X",
            //    COND_VALUE = "X",
            //    CURRENCY = "X",
            //    CHANGE_ID = "X",
            //    CONDITION_NOX = "X",
            //}).ToList(),

            //POHEADER = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER(),
            //POHEADERX = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADERX(),
            modificarPedidoSAP.POHEADER = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER();
            modificarPedidoSAP.POHEADERX = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADERX();

            /*
            var listPOITEM = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM>();
            var listPOITEMX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX>();
            var listPOSCHEDULE = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE>();
            var listPOSCHEDULEX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX>();
            var listPOSERVICES = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX>();
            */
            foreach (var item in ocSap.POITEM)
            {
                listPOITEM.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM()
                {
                    PO_ITEM = item.PO_ITEM,
                    PCKG_NO = item.PCKG_NO
                });
            }
            modificarPedidoSAP.POITEM = listPOITEM.ToList();
            //POITEM = ocSap.POITEM.Select(x => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM
            //{
            //    PO_ITEM = x.PO_ITEM,
            //    PCKG_NO = x.PCKG_NO
            //}).ToList(),

            foreach (var item in ocSap.POITEM)
            {
                listPOITEMX.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX()
                {
                    PO_ITEM = item.PO_ITEM
                });
            }
            modificarPedidoSAP.POITEMX = listPOITEMX.ToList();

            //POITEMX = ocSap.POITEM.Select(x => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX
            //{
            //    PO_ITEM = x.PO_ITEM,
            //}).ToList(),

            foreach (var item in ocSap.POSCHEDULE)
            {
                listPOSCHEDULE.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE()
                {
                    PO_ITEM = item.PO_ITEM,
                    SCHED_LINE = item.SCHED_LINE,
                    DELIVERY_DATE = item.DELIVERY_DATE
                });
            }
            modificarPedidoSAP.POSCHEDULE = listPOSCHEDULE.ToList();

            foreach (var item in ocSap.POSCHEDULE)
            {
                listPOSCHEDULEX.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX()
                {
                    PO_ITEM = item.PO_ITEM,
                    SCHED_LINE = item.SCHED_LINE,
                    DELIVERY_DATE = "X"
                });
            }
            modificarPedidoSAP.POSCHEDULEX = listPOSCHEDULEX.ToList();
            modificarPedidoSAP.POSERVICES = ConvertirLista<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC>(ocSap.POSERVICES.ToList());


            //POSCHEDULE = ocSap.POSCHEDULE.Select(x => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = x.DELIVERY_DATE }).ToList(),
            //POSCHEDULEX = ocSap.POSCHEDULE.Select(x => new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = "X" }).ToList(),
            //POSERVICES = ConvertirLista<BAPIESLLC>(ocSap.POSERVICES.ToList()),

            //POSRVACCESSVALUES = ocSap.POSRVACCESSVALUES.Select(x => new BAPIESKLC
            //{
            //    PCKG_NO = x.PCKG_NO,
            //    LINE_NO = x.LINE_NO,
            //    PERCENTAGE = x.PERCENTAGE,
            //    SERNO_LINE = x.SERNO_LINE,
            //    SERIAL_NO = x.SERIAL_NO,
            //    QUANTITY = x.QUANTITY,
            //    NET_VALUE = x.NET_VALUE,
            //    NET_VALUESpecified = true,
            //    PERCENTAGESpecified = true,
            //    QUANTITYSpecified = true,
            //}).ToList(),

            modificarPedidoSAP.POTEXTHEADER = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER>();
            modificarPedidoSAP.PURCHASEORDER = adjudicacion.NumeroOrdenDeCompra;

            
            //foreach (var bAPIESLLC in modificarPedidoSAP.POSERVICES)
            //{
            //    bAPIESLLC.GR_PRICESpecified = true;
            //    bAPIESLLC.QUANTITYSpecified = true;
            //    bAPIESLLC.NET_VALUESpecified = true;
            //    bAPIESLLC.PRICE_UNITSpecified = true;
            //}

            //Racional de compras
            var listaVaciaTexto = new string[] { "" };
            var textosDiccionario = new Dictionary<string, string[]>() {
                {"F01", !string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ?  adjudicacion.TextoDeCabecera.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F05", !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega)? adjudicacion.CondicionesDeEntrega.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F07", !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ? adjudicacion.CondicionesDePago.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F08", !string.IsNullOrEmpty(adjudicacion.Garantias) ? adjudicacion.Garantias.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            };
            foreach (var grupos in textosDiccionario)
            {
                bool todosVacios = grupos.Value.All(string.IsNullOrEmpty);
                if (!todosVacios)
                {
                    foreach (var texto in grupos.Value)
                    {
                        modificarPedidoSAP.POTEXTHEADER.Add(new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOTEXTHEADER
                        {
                            TEXT_ID = grupos.Key,
                            PO_NUMBER = "",
                            PO_ITEM = "0",
                            TEXT_FORM = "*",
                            TEXT_LINE = texto
                        });
                    }
                }

            }

            //Condición de Pago  - no funcionan para las  ZPE1 y ZDIR por que sap no lo permite
            List<string> condicionesNoEditables = new List<string>() { "ZPE1", "ZDIR" };
            if (!condicionesNoEditables.Contains(ocSap.POHEADER.DOC_TYPE))
            {
                modificarPedidoSAP.POHEADER.PMNTTRMS = adjudicacion.CondicionDePagoCodigo;
                modificarPedidoSAP.POHEADER.DSCNT1_TO = adjudicacion.PagoEn1;
                //modificarPedidoSAP.POHEADER.DSCNT1_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCNT2_TO = adjudicacion.PagoEn2;
                //modificarPedidoSAP.POHEADER.DSCNT2_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCNT3_TO = adjudicacion.PagoEn3;
                //modificarPedidoSAP.POHEADER.DSCNT3_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCT_PCT1 = adjudicacion.PagoEn1Porcentaje;
                //modificarPedidoSAP.POHEADER.DSCT_PCT1Specified = true;
                modificarPedidoSAP.POHEADER.DSCT_PCT2 = adjudicacion.PagoEn2Porcentaje;
                //modificarPedidoSAP.POHEADER.DSCT_PCT2Specified = true;
                modificarPedidoSAP.POHEADERX.PMNTTRMS = "X";
                modificarPedidoSAP.POHEADERX.DSCNT1_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCNT2_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCNT3_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCT_PCT1 = "X";
                modificarPedidoSAP.POHEADERX.DSCT_PCT2 = "X";
            }
            else
            {
                var header = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOHEADER();

                header.PMNTTRMS  = ocSap.POHEADER.PMNTTRMS;
                header.DSCNT1_TO = ocSap.POHEADER.DSCNT1_TO;
                header.DSCNT2_TO = ocSap.POHEADER.DSCNT2_TO;
                header.DSCNT3_TO = ocSap.POHEADER.DSCNT3_TO;
                header.DSCT_PCT1 = ocSap.POHEADER.DSCT_PCT1;
                header.DSCT_PCT2 = ocSap.POHEADER.DSCT_PCT2;

                if (HayModificacionCondicionesDePagoSinPI(header, adjudicacion))// TODO: validar si edito alguna condicion de pago.
                {
                    resultadoEditarOC.Errores.Add(new ErrorMessage("Condición de Pago no se puede editar para las clase de documento  ZPE1 y ZDIR."));
                    return resultadoEditarOC;
                }
            }


            //Condición de Importacion
            if (!string.IsNullOrEmpty(adjudicacion.CondicionDeImportacionComplemento) && adjudicacion.CondicionDeImportacionComplemento.Length > 28)
            {
                resultadoEditarOC.Errores.Add(new ErrorMessage("Condicion de importacion muy largo, 28 caracteres maximo."));
                return resultadoEditarOC;
            }
            modificarPedidoSAP.POHEADER.INCOTERMS1 = adjudicacion.CondicionDeImportacionCodigo;
            modificarPedidoSAP.POHEADER.INCOTERMS2 = adjudicacion.CondicionDeImportacionComplemento;
            modificarPedidoSAP.POHEADERX.INCOTERMS1 = "X";
            modificarPedidoSAP.POHEADERX.INCOTERMS2 = "X";

            //Datos utiles
            bool esMateriales = ocSap.POITEM[0].ITEM_CAT == "0";
            bool modificoMoneda = ocSap.POHEADER.CURRENCY != adjudicacion.MonedaCodigo;

            //Modificar moneda
            if (modificoMoneda)
            {
                modificarPedidoSAP.POHEADER.CURRENCY = adjudicacion.MonedaCodigo;
                modificarPedidoSAP.POHEADERX.CURRENCY = "X";
            }

            foreach (var posAdj in adjudicacion.Posiciones)
            {
                string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');

                //Región
                var direccioSap = modificarPedidoSAP.POADDRDELIVERY.Single(a => a.PO_ITEM == PO_ITEM);
                direccioSap.REGION = posAdj.RegionCodigo;
                direccioSap.COUNTRY = posAdj.PaisCodigo;

                //Fechas de entrega(por posición)
                var fechaEntregaSap = modificarPedidoSAP.POSCHEDULE.Single(a => a.PO_ITEM == PO_ITEM);
                var fechaEntregaSapX = modificarPedidoSAP.POSCHEDULEX.Single(a => a.PO_ITEM == PO_ITEM);
                fechaEntregaSap.DELIVERY_DATE = posAdj.FechaEntrega.ToString("dd.MM.yyyy");
                fechaEntregaSapX.DELIVERY_DATE = "X";

                //Posición
                var posicionSap = modificarPedidoSAP.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
                var posicionSapX = modificarPedidoSAP.POITEMX.Single(a => a.PO_ITEM == PO_ITEM);
                //Impitación
                var imputacionSap = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT();
                var imputacionSapX = new SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX();
                if (modificarPedidoSAP.POACCOUNT.Count > 0)
                {
                    imputacionSap = modificarPedidoSAP.POACCOUNT.FirstOrDefault(a => a.PO_ITEM == PO_ITEM);
                    imputacionSapX = modificarPedidoSAP.POACCOUNTX.FirstOrDefault(a => a.PO_ITEM == PO_ITEM);
                }
                //Condición
                var condicionSap = modificarPedidoSAP.POCOND.Single(a => a.ITM_NUMBER == "0" + PO_ITEM);

                //Eliminar posición
                posicionSap.DELETE_IND = posAdj.Eliminado ? "X" : "";
                posicionSapX.DELETE_IND = "X";

                //Tilde entrega final
                posicionSap.NO_MORE_GR = posAdj.EntregaFinal ? "X" : "";
                posicionSapX.NO_MORE_GR = "X";

                if (esMateriales)
                {
                    bool modificoImporte = ModificoImporteSinPI(ocSap, adjudicacion);

                    //Modificar cantidad
                    posicionSap.QUANTITY = posAdj.Cantidad;
                    //posicionSap.QUANTITYSpecified = true;
                    posicionSapX.QUANTITY = "X";
                    if (modificarPedidoSAP.POACCOUNT.Count > 0)
                    {
                        imputacionSap.QUANTITY = posAdj.Cantidad;
                        imputacionSapX.QUANTITY = "X";
                    }

                    //Modificar importe
                    if (modificoImporte)
                    {
                        posicionSap.NET_PRICE = posAdj.PrecioUnidadCodigo;
                        //posicionSap.NET_PRICESpecified = true;
                        posicionSapX.NET_PRICE = "X";
                        condicionSap.COND_VALUE = posAdj.PrecioUnidadCodigo;
                        //condicionSap.COND_VALUESpecified = true;
                        condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
                        //"ZP01" no deja cambiar importes por eso se cambia a "ZP00"
                        condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP00" : condicionSap.COND_TYPE;
                    }
                }
                else
                {

                    if (!posAdj.Eliminado)
                    {
                        foreach (var subPosAdj in posAdj.SubPosiciones)
                        {
                            //Subposición
                            string LINE_NO = subPosAdj.Indice.ToString().PadLeft(10, '0');
                            var subPosicionSap = modificarPedidoSAP.POSERVICES.First(a => a.LINE_NO == LINE_NO);
                            //var imputacionSubPos = modificarPedidoSAP.POSRVACCESSVALUES.First(a => a.LINE_NO == LINE_NO);//

                            //Eliminar subposición 
                            subPosicionSap.DELETE_IND = subPosAdj.Eliminado ? "X" : "";
                            if (subPosAdj.Eliminado) continue;
                            //Cantidad
                            subPosicionSap.QUANTITY = subPosAdj.Cantidad;
                            //imputacionSubPos.QUANTITY = subPosAdj.Cantidad;
                            //Importe 1/2                            
                            subPosicionSap.GR_PRICE = subPosAdj.PrecioUnitario;
                            subPosicionSap.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;
                            //imputacionSubPos.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;

                        }

                        //Importe 2/2                        
                        var totalPosicion = posAdj.SubPosiciones.Where(a => !a.Eliminado).Sum(a => a.Cantidad * a.PrecioUnitario);
                        condicionSap.COND_VALUE = totalPosicion;
                        //condicionSap.COND_VALUESpecified = true;
                        condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
                        condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP01" : condicionSap.COND_TYPE;

                        posicionSap.NET_PRICE = totalPosicion;
                        //posicionSap.NET_PRICESpecified = true;
                        posicionSapX.NET_PRICE = "X";
                        //imputacionSap.NET_VALUE = totalPosicion;
                        //imputacionSap.NET_VALUESpecified = true;
                        //imputacionSapX.NET_VALUE = "X";

                    }

                }
            }

            if (!esMateriales) // para servicios guille nos dijo que no lo enviemos pero para materiales si lo necesitamos enviar.
            {
                modificarPedidoSAP.POACCOUNT = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT>();
                modificarPedidoSAP.POACCOUNTX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX>();
            }

            if (modificoMoneda)
            {
                modificarPedidoSAP.POITEM = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEM>();
                modificarPedidoSAP.POITEMX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOITEMX>();

                modificarPedidoSAP.POACCOUNT = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNT>();
                modificarPedidoSAP.POACCOUNTX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOACCOUNTX>();

                modificarPedidoSAP.POSERVICES = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIESLLC>();

                modificarPedidoSAP.POCOND = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCOND>();
                modificarPedidoSAP.POCONDX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOCONDX>();

                modificarPedidoSAP.POADDRDELIVERY = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOADDRDELIVERY>();
                modificarPedidoSAP.POSCHEDULE = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULE>();
                modificarPedidoSAP.POSCHEDULEX = new List<SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIMEPOSCHEDULX>();

            }

            var resultadoSAP = modificarOrdenDeCompraConsumerMOA.EditarPedidoRequestSinPI(modificarPedidoSAP);
            resultadoSAP.Where(a => a.MESSAGE == "No se han modificado datos").ToList().ForEach(a => a.TYPE = "E");

            foreach (var item in resultadoSAP.Where(x => x.TYPE == "E"))
            {
                resultadoEditarOC.Error(item.TYPE, item.MESSAGE);
            }
            

            return resultadoEditarOC;
        }
        private ResultadoGenerico ModificarOrdenDeCompra(AdjudicacionDto adjudicacionDto) {

            var adjudicacion = ConvertirAjudicacionDtoEnAdjudicacionSAP(adjudicacionDto);
            var ocSap = obtenerOrdenDeCompraConsumerMOA.ObtenerOrdenDeCompraRFC(adjudicacion.NumeroOrdenDeCompra);
            ResultadoGenerico resultadoEditarOC = new ResultadoGenerico();

            ModificarPedidoSAP modificarPedidoSAP = new ModificarPedidoSAP
            {
                POACCOUNT = ocSap.POACCOUNT.Select(a => new BAPIMEPOACCOUNT
                {
                    PO_ITEM = a.PO_ITEM
                }).ToList(),
                POACCOUNTX = ocSap.POACCOUNT.Select(a => new BAPIMEPOACCOUNTX
                {
                    PO_ITEM = a.PO_ITEM
                }).ToList(),
                POADDRDELIVERY = ocSap.POADDRDELIVERY.Select(a => new BAPIMEPOADDRDELIVERY
                {
                    PO_ITEM = a.PO_ITEM,
                    POSTL_COD1 = a.POSTL_COD1,
                    CITY = a.CITY,
                    ADDR_NO = "",
                    NAME = a.NAME,
                    TEL1_NUMBR = "",
                    STREET = a.STREET,
                    STREET_NO = "",
                    REGION = a.REGION,
                    COUNTRY = a.COUNTRY
                }).ToList(),
                POCOND = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new BAPIMEPOCOND
                {
                    ITM_NUMBER = x.ITM_NUMBER,  //el número de ítem al que corresponda la condición
                    COND_ST_NO = x.COND_ST_NO,
                    COND_TYPE = x.COND_TYPE,
                    COND_VALUE = x.COND_VALUE, //el importe de la condición
                    COND_VALUESpecified = true,
                    CURRENCY = x.CURRENCY,
                    CHANGE_ID = "U",
                    //COND_COUNT = x.COND_COUNT,

                }).ToList(),
                POCONDX = ocSap.POCOND.Where(a => a.COND_TYPE != "SKTO").Select(x => new BAPIMEPOCONDX
                {
                    ITM_NUMBER = x.ITM_NUMBER,
                    ITM_NUMBERX = "X",
                    COND_ST_NO = "001",
                    COND_ST_NOX = "X",
                    COND_TYPE = "X",
                    COND_VALUE = "X",
                    CURRENCY = "X",
                    CHANGE_ID = "X",
                    CONDITION_NOX = "X",
                }).ToList(),

                POHEADER = new BAPIMEPOHEADER(),
                POHEADERX = new BAPIMEPOHEADERX(),
                POITEM = ocSap.POITEM.Select(x => new BAPIMEPOITEM
                {
                    PO_ITEM = x.PO_ITEM,
                    PCKG_NO = x.PCKG_NO
                }).ToList(),
                POITEMX = ocSap.POITEM.Select(x => new BAPIMEPOITEMX
                {
                    PO_ITEM = x.PO_ITEM,
                }).ToList(),
                POSCHEDULE = ocSap.POSCHEDULE.Select(x => new BAPIMEPOSCHEDULE { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = x.DELIVERY_DATE }).ToList(),
                POSCHEDULEX = ocSap.POSCHEDULE.Select(x => new BAPIMEPOSCHEDULX { PO_ITEM = x.PO_ITEM, SCHED_LINE = x.SCHED_LINE, DELIVERY_DATE = "X" }).ToList(),
                POSERVICES = ConvertirLista<BAPIESLLC>(ocSap.POSERVICES.ToList()),
                //POSRVACCESSVALUES = ocSap.POSRVACCESSVALUES.Select(x => new BAPIESKLC
                //{
                //    PCKG_NO = x.PCKG_NO,
                //    LINE_NO = x.LINE_NO,
                //    PERCENTAGE = x.PERCENTAGE,
                //    SERNO_LINE = x.SERNO_LINE,
                //    SERIAL_NO = x.SERIAL_NO,
                //    QUANTITY = x.QUANTITY,
                //    NET_VALUE = x.NET_VALUE,
                //    NET_VALUESpecified = true,
                //    PERCENTAGESpecified = true,
                //    QUANTITYSpecified = true,
                //}).ToList(),
                POTEXTHEADER = new List<BAPIMEPOTEXTHEADER>(),
                PURCHASEORDER = adjudicacion.NumeroOrdenDeCompra,

            };
            foreach (var bAPIESLLC in modificarPedidoSAP.POSERVICES)
            {
                bAPIESLLC.GR_PRICESpecified = true;
                bAPIESLLC.QUANTITYSpecified = true;
                bAPIESLLC.NET_VALUESpecified = true;
                bAPIESLLC.PRICE_UNITSpecified = true;
            }

            //Racional de compras
            var listaVaciaTexto = new string[] { "" };
            var textosDiccionario = new Dictionary<string, string[]>() {
                {"F01", !string.IsNullOrEmpty(adjudicacion.TextoDeCabecera) ?  adjudicacion.TextoDeCabecera.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F05", !string.IsNullOrEmpty(adjudicacion.CondicionesDeEntrega)? adjudicacion.CondicionesDeEntrega.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F07", !string.IsNullOrEmpty(adjudicacion.CondicionesDePago) ? adjudicacion.CondicionesDePago.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
                {"F08", !string.IsNullOrEmpty(adjudicacion.Garantias) ? adjudicacion.Garantias.SplitParagraph(131).Where(x => x != null).ToArray() : listaVaciaTexto},
            };
            foreach (var grupos in textosDiccionario)
            {
                bool todosVacios = grupos.Value.All(string.IsNullOrEmpty);
                if (!todosVacios)
                {
                    foreach (var texto in grupos.Value)
                    {
                        modificarPedidoSAP.POTEXTHEADER.Add(new BAPIMEPOTEXTHEADER
                        {
                            TEXT_ID = grupos.Key,
                            PO_NUMBER = "",
                            PO_ITEM = "0",
                            TEXT_FORM = "*",
                            TEXT_LINE = texto
                        });
                    }
                }

            }

            //Condición de Pago  - no funcionan para las  ZPE1 y ZDIR por que sap no lo permite
            List<string> condicionesNoEditables = new List<string>() { "ZPE1", "ZDIR" };
            if (!condicionesNoEditables.Contains(ocSap.POHEADER.DOC_TYPE))
            {
                modificarPedidoSAP.POHEADER.PMNTTRMS = adjudicacion.CondicionDePagoCodigo;
                modificarPedidoSAP.POHEADER.DSCNT1_TO = adjudicacion.PagoEn1;
                modificarPedidoSAP.POHEADER.DSCNT1_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCNT2_TO = adjudicacion.PagoEn2;
                modificarPedidoSAP.POHEADER.DSCNT2_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCNT3_TO = adjudicacion.PagoEn3;
                modificarPedidoSAP.POHEADER.DSCNT3_TOSpecified = true;
                modificarPedidoSAP.POHEADER.DSCT_PCT1 = adjudicacion.PagoEn1Porcentaje;
                modificarPedidoSAP.POHEADER.DSCT_PCT1Specified = true;
                modificarPedidoSAP.POHEADER.DSCT_PCT2 = adjudicacion.PagoEn2Porcentaje;
                modificarPedidoSAP.POHEADER.DSCT_PCT2Specified = true;
                modificarPedidoSAP.POHEADERX.PMNTTRMS = "X";
                modificarPedidoSAP.POHEADERX.DSCNT1_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCNT2_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCNT3_TO = "X";
                modificarPedidoSAP.POHEADERX.DSCT_PCT1 = "X";
                modificarPedidoSAP.POHEADERX.DSCT_PCT2 = "X";
            }
            else
            {
                if (HayModificacionCondicionesDePago(ocSap.POHEADER, adjudicacion))// TODO: validar si edito alguna condicion de pago.
                {
                    resultadoEditarOC.Errores.Add(new ErrorMessage("Condición de Pago no se puede editar para las clase de documento  ZPE1 y ZDIR."));
                    return resultadoEditarOC;
                }
            }


            //Condición de Importacion
            if (!string.IsNullOrEmpty(adjudicacion.CondicionDeImportacionComplemento) && adjudicacion.CondicionDeImportacionComplemento.Length > 28)
            {
                resultadoEditarOC.Errores.Add(new ErrorMessage("Condicion de importacion muy largo, 28 caracteres maximo."));
                return resultadoEditarOC;
            }
            modificarPedidoSAP.POHEADER.INCOTERMS1 = adjudicacion.CondicionDeImportacionCodigo;
            modificarPedidoSAP.POHEADER.INCOTERMS2 = adjudicacion.CondicionDeImportacionComplemento;
            modificarPedidoSAP.POHEADERX.INCOTERMS1 = "X";
            modificarPedidoSAP.POHEADERX.INCOTERMS2 = "X";

            //Datos utiles
            bool esMateriales = ocSap.POITEM[0].ITEM_CAT == "0";
            bool modificoMoneda = ocSap.POHEADER.CURRENCY != adjudicacion.MonedaCodigo;

            //Modificar moneda
            if (modificoMoneda)
            {
                modificarPedidoSAP.POHEADER.CURRENCY = adjudicacion.MonedaCodigo;
                modificarPedidoSAP.POHEADERX.CURRENCY = "X";
            }

            foreach (var posAdj in adjudicacion.Posiciones)
            {
                string PO_ITEM = posAdj.Indice.ToString().PadLeft(5, '0');

                //Región
                var direccioSap = modificarPedidoSAP.POADDRDELIVERY.Single(a => a.PO_ITEM == PO_ITEM);
                direccioSap.REGION = posAdj.RegionCodigo;
                direccioSap.COUNTRY = posAdj.PaisCodigo;

                //Fechas de entrega(por posición)
                var fechaEntregaSap = modificarPedidoSAP.POSCHEDULE.Single(a => a.PO_ITEM == PO_ITEM);
                var fechaEntregaSapX = modificarPedidoSAP.POSCHEDULEX.Single(a => a.PO_ITEM == PO_ITEM);
                fechaEntregaSap.DELIVERY_DATE = posAdj.FechaEntrega.ToString("dd.MM.yyyy");
                fechaEntregaSapX.DELIVERY_DATE = "X";

                //Posición
                var posicionSap = modificarPedidoSAP.POITEM.Single(a => a.PO_ITEM == PO_ITEM);
                var posicionSapX = modificarPedidoSAP.POITEMX.Single(a => a.PO_ITEM == PO_ITEM);
                //Impitación
                var imputacionSap = new BAPIMEPOACCOUNT();
                var imputacionSapX = new BAPIMEPOACCOUNTX();
                if (modificarPedidoSAP.POACCOUNT.Count > 0)
                {
                    imputacionSap = modificarPedidoSAP.POACCOUNT.FirstOrDefault(a => a.PO_ITEM == PO_ITEM);
                    imputacionSapX = modificarPedidoSAP.POACCOUNTX.FirstOrDefault(a => a.PO_ITEM == PO_ITEM);
                }
                //Condición
                var condicionSap = modificarPedidoSAP.POCOND.Single(a => a.ITM_NUMBER == "0" + PO_ITEM);

                //Eliminar posición
                posicionSap.DELETE_IND = posAdj.Eliminado ? "X" : "";
                posicionSapX.DELETE_IND = "X";

                //Tilde entrega final
                posicionSap.NO_MORE_GR = posAdj.EntregaFinal ? "X" : "";
                posicionSapX.NO_MORE_GR = "X";

                if (esMateriales)
                {
                    bool modificoImporte = ModificoImporte(ocSap, adjudicacion);

                    //Modificar cantidad
                    posicionSap.QUANTITY = posAdj.Cantidad;
                    posicionSap.QUANTITYSpecified = true;
                    posicionSapX.QUANTITY = "X";
                    if (modificarPedidoSAP.POACCOUNT.Count > 0)
                    {
                        imputacionSap.QUANTITY = posAdj.Cantidad;
                        imputacionSapX.QUANTITY = "X";
                    }

                    //Modificar importe
                    if (modificoImporte)
                    {
                        posicionSap.NET_PRICE = posAdj.PrecioUnidadCodigo;
                        posicionSap.NET_PRICESpecified = true;
                        posicionSapX.NET_PRICE = "X";
                        condicionSap.COND_VALUE = posAdj.PrecioUnidadCodigo;
                        condicionSap.COND_VALUESpecified = true;
                        condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
                        //"ZP01" no deja cambiar importes por eso se cambia a "ZP00"
                        condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP00" : condicionSap.COND_TYPE;
                    }
                }
                else
                {

                    if (!posAdj.Eliminado)
                    {
                        foreach (var subPosAdj in posAdj.SubPosiciones)
                        {
                            //Subposición
                            string LINE_NO = subPosAdj.Indice.ToString().PadLeft(10, '0');
                            var subPosicionSap = modificarPedidoSAP.POSERVICES.First(a => a.LINE_NO == LINE_NO);
                            //var imputacionSubPos = modificarPedidoSAP.POSRVACCESSVALUES.First(a => a.LINE_NO == LINE_NO);//

                            //Eliminar subposición 
                            subPosicionSap.DELETE_IND = subPosAdj.Eliminado ? "X" : "";
                            if (subPosAdj.Eliminado) continue;
                            //Cantidad
                            subPosicionSap.QUANTITY = subPosAdj.Cantidad;
                            //imputacionSubPos.QUANTITY = subPosAdj.Cantidad;
                            //Importe 1/2                            
                            subPosicionSap.GR_PRICE = subPosAdj.PrecioUnitario;
                            subPosicionSap.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;
                            //imputacionSubPos.NET_VALUE = subPosAdj.PrecioUnitario * subPosAdj.Cantidad;

                        }

                        //Importe 2/2                        
                        var totalPosicion = posAdj.SubPosiciones.Where(a => !a.Eliminado).Sum(a => a.Cantidad * a.PrecioUnitario);
                        condicionSap.COND_VALUE = totalPosicion;
                        condicionSap.COND_VALUESpecified = true;
                        condicionSap.CURRENCY = adjudicacion.MonedaCodigo;
                        condicionSap.COND_TYPE = condicionSap.COND_TYPE == "ZP01" ? "ZP01" : condicionSap.COND_TYPE;

                        posicionSap.NET_PRICE = totalPosicion;
                        posicionSap.NET_PRICESpecified = true;
                        posicionSapX.NET_PRICE = "X";
                        //imputacionSap.NET_VALUE = totalPosicion;
                        //imputacionSap.NET_VALUESpecified = true;
                        //imputacionSapX.NET_VALUE = "X";

                    }

                }
            }

            if (!esMateriales) // para servicios guille nos dijo que no lo enviemos pero para materiales si lo necesitamos enviar.
            {
                modificarPedidoSAP.POACCOUNT = new List<BAPIMEPOACCOUNT>();
                modificarPedidoSAP.POACCOUNTX = new List<BAPIMEPOACCOUNTX>();
            }

            if (modificoMoneda)
            {
                modificarPedidoSAP.POITEM = new List<BAPIMEPOITEM>();
                modificarPedidoSAP.POITEMX = new List<BAPIMEPOITEMX>();

                modificarPedidoSAP.POACCOUNT = new List<BAPIMEPOACCOUNT>();
                modificarPedidoSAP.POACCOUNTX = new List<BAPIMEPOACCOUNTX>();

                modificarPedidoSAP.POSERVICES = new List<BAPIESLLC>();

                modificarPedidoSAP.POCOND = new List<BAPIMEPOCOND>();
                modificarPedidoSAP.POCONDX = new List<BAPIMEPOCONDX>();

                modificarPedidoSAP.POADDRDELIVERY = new List<BAPIMEPOADDRDELIVERY>();
                modificarPedidoSAP.POSCHEDULE = new List<BAPIMEPOSCHEDULE>();
                modificarPedidoSAP.POSCHEDULEX = new List<BAPIMEPOSCHEDULX>();

            }

            var resultadoSAP = modificarOrdenDeCompraConsumerMOA.EditarPedidoRequest(modificarPedidoSAP);
            resultadoSAP.Where(a => a.MESSAGE == "No se han modificado datos").ToList().ForEach(a => a.TYPE = "E");

            foreach (var item in resultadoSAP.Where(x => x.TYPE == "E"))
            {
                resultadoEditarOC.Error(item.TYPE, item.MESSAGE);
            }

            return resultadoEditarOC;

        }
    }
}
