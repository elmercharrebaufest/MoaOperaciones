// Ignore Spelling: Solp Sustitucion Utils Solpe Posicion numeros

using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.sap;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.WSConsumers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class ComprasSapService : IComprasSapService
    {
        private readonly IObtenerCecoSolpConsumerMOA CentroDeCostoSolpConsumerMOA;
        private readonly ICrearSolpConsumerMOA crearSolpConsumerMOA;
        private readonly IModificarSolpConsumerMOA modificarSolpConsumerMOA;
        private readonly IObtenerCuentasSolpConsumerMOA cuentasSolpConsumerMOA;
        private readonly IObtenerOrdenSolpConsumerMOA ordenesSolpConsumerMOA;
        private readonly IObtenerServiciosSolpConsumerMOA serviciosSolpConsumerMOA;
        private readonly IObtenerSolpConsumerMOA obtenerSolpConsumerMOA;

        private readonly ICentroDireccionService centroDireccionService;
        private readonly ITablaSapService tablaSapService;
        private readonly IUnidadMedidaService unidadMedidaService;
        private readonly IUsuarioService usuarioService;

        public ComprasSapService(IObtenerCecoSolpConsumerMOA obtenerCentroDeCostoSolpConsumerMOA,
                                 ICrearSolpConsumerMOA crearSolpConsumerMOA,
                                 IModificarSolpConsumerMOA modificarSolpConsumerMOA,
                                 IObtenerCuentasSolpConsumerMOA obtenerCuentasSolpConsumerMOA,
                                 IObtenerOrdenSolpConsumerMOA obtenerOrdenSolpConsumerMOA,
                                 IObtenerServiciosSolpConsumerMOA obtenerServiciosSolpConsumerMOA,
                                 IObtenerSolpConsumerMOA obtenerSolpConsumerMOA,
                                 ICentroDireccionService centroDireccionService,
                                 ITablaSapService tablaSapService,
                                 IUnidadMedidaService unidadMedidaService,
                                 IUsuarioService usuarioService)
        {
            this.CentroDeCostoSolpConsumerMOA = obtenerCentroDeCostoSolpConsumerMOA;
            this.crearSolpConsumerMOA = crearSolpConsumerMOA;
            this.modificarSolpConsumerMOA = modificarSolpConsumerMOA;
            this.cuentasSolpConsumerMOA = obtenerCuentasSolpConsumerMOA;
            this.ordenesSolpConsumerMOA = obtenerOrdenSolpConsumerMOA;
            this.serviciosSolpConsumerMOA = obtenerServiciosSolpConsumerMOA;
            this.obtenerSolpConsumerMOA = obtenerSolpConsumerMOA;
            this.centroDireccionService = centroDireccionService;
            this.tablaSapService = tablaSapService;
            this.unidadMedidaService = unidadMedidaService;
            this.usuarioService = usuarioService;
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
                            PROFIT_CTR = getCodigoTablaSap(subPosicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio
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

        public CrearSolpConsumerMOAResponse CrearSolpSap(SolpSAPDto solpSap)
        {
            return crearSolpConsumerMOA.Request(solpSap);
        }

        public ModificarSolpConsumerMOAResponse ModificarSolpSap(SolpSAPDto solpSap)
        {
            return modificarSolpConsumerMOA.Request(solpSap);
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
    }
}
