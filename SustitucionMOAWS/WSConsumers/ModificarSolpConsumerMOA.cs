using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.ModificarSolpWebServiceMOA;
using SustitucionMOARepositorio;
using SustitucionMOAModel.Dto;
using AutoMapper;

namespace SustitucionMOAWS.WSConsumers
{
    public class ModificarSolpConsumerMOA : IModificarSolpConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly SI_MMRFC_MODIFICAR_SOLPEDClient service;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];

        public ModificarSolpConsumerMOA(IRepositorio repositorio)
        {
            var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_MODIFICAR_SOLPED&amp;interfaceNamespace=urn%3AOPERACIONES";
            service = new SI_MMRFC_MODIFICAR_SOLPEDClient(SAPCredential.CrearSapBasicBinding(), SAPCredential.DevolverEndpoint(url));
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        public ModificarSolpConsumerMOAResponse Request(SolpSAPDto solpSAP)
        {
            var serxml = new System.Xml.Serialization.XmlSerializer(solpSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(solpSAP.Id, " - ", fecha, " - llamada modificar.xml");
            var nombreArchivoRespuesta = string.Concat(solpSAP.Id, " - ", fecha, " - respuesta modificar.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);
            var rutaArchivoRespuesta = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoRespuesta);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create();
            File.WriteAllText(fileCrear.FullName, xml);

            var result = service.SI_MMRFC_MODIFICAR_SOLPED(
                                                       solpSAP.NroSolp,
                                                       solpSAP.IM_PRACCOUNTList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5690
                                                       {
                                                           ASSET_NO = a.ASSET_NO,
                                                           BUS_AREA = a.BUS_AREA,
                                                           COSTCENTER = a.COSTCENTER,
                                                           COSTOBJECT = a.COSTOBJECT,
                                                           CO_AREA = a.CO_AREA,
                                                           GL_ACCOUNT = a.GL_ACCOUNT,
                                                           ORDERID = a.ORDERID,
                                                           PREQ_ITEM = a.PREQ_ITEM,
                                                           PROFIT_CTR = a.PROFIT_CTR,
                                                           SUB_NUMBER = a.SUB_NUMBER,
                                                           QUANTITY = a.QUANTITY,
                                                           QUANTITYSpecified = a.QUANTITYSpecified,
                                                           SERIAL_NO = a.SERIAL_NO
                                                       }).ToArray(),
                                                       solpSAP.IM_PRACCOUNTXList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5680
                                                       {
                                                           ASSET_NO = a.ASSET_NO,
                                                           BUS_AREA = a.BUS_AREA,
                                                           COSTCENTER = a.COSTCENTER,
                                                           COSTOBJECT = a.COSTOBJECT,
                                                           CO_AREA = a.CO_AREA,
                                                           GL_ACCOUNT = a.GL_ACCOUNT,
                                                           ORDERID = a.ORDERID,
                                                           PREQ_ITEM = a.PREQ_ITEM,
                                                           PROFIT_CTR = a.PROFIT_CTR,
                                                           QUANTITY = a.QUANTITY,
                                                           PREQ_ITEMX = a.PREQ_ITEMX,
                                                           SERIAL_NOX = a.SERIAL_NOX,
                                                           SUB_NUMBER = a.SUB_NUMBER,
                                                           SERIAL_NO = a.SERIAL_NO
                                                       }).ToArray(),
                                                       solpSAP.IM_PRADDRDELIVERYList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5750
                                                       {
                                                           ADDR_NO = a.ADDR_NO,
                                                           CITY = a.CITY,
                                                           HOUSE_NO = a.HOUSE_NO,
                                                           NAME = a.NAME,
                                                           POSTL_COD1 = a.POSTL_COD1,
                                                           PREQ_ITEM = a.PREQ_ITEM,
                                                           PREQ_NO = a.PREQ_NO,
                                                           STREET = a.STREET,
                                                           TEL1_NUMBR = a.TEL1_NUMBR
                                                       }).ToArray(),
                                                       solpSAP.IM_PRHEADERTEXTList.Select(a => new ModificarSolpWebServiceMOA.BAPIMEREQHEADTEXT
                                                       {
                                                           PREQ_ITEM = a.PREQ_ITEM,
                                                           PREQ_NO = a.PREQ_NO,
                                                           TEXT_FORM = a.TEXT_FORM,
                                                           TEXT_ID = a.TEXT_ID,
                                                           TEXT_LINE = a.TEXT_LINE
                                                       }).ToArray(),
                                                       solpSAP.IM_PRITEMList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5700
                                                       {
                                                           ACCTASSCAT = a.ACCTASSCAT,
                                                           PREQ_ITEM = a.PREQ_ITEM,
                                                           AGMT_ITEM = a.AGMT_ITEM,
                                                           AGREEMENT = a.AGREEMENT,
                                                           CLOSED = a.CLOSED,
                                                           CREATED_BY = a.CREATED_BY,
                                                           CURRENCY = a.CURRENCY,
                                                           CURRENCY_ISO = a.CURRENCY_ISO,
                                                           DELETE_IND = a.DELETE_IND,
                                                           DELIV_DATE = a.DELIV_DATE,
                                                           DES_VENDOR = a.DES_VENDOR,
                                                           FIXED_VEND = a.FIXED_VEND,
                                                           GR_PR_TIME = a.GR_PR_TIME,
                                                           GR_PR_TIMESpecified = a.GR_PR_TIMESpecified,
                                                           INFO_REC = a.INFO_REC,
                                                           ITEM_CAT = a.ITEM_CAT,
                                                           MATERIAL = a.MATERIAL,
                                                           MATL_GROUP = a.MATL_GROUP,
                                                           PCKG_NO = a.PCKG_NO,
                                                           PLANT = a.PLANT,
                                                           PLND_DELRY = a.PLND_DELRY,
                                                           PLND_DELRYSpecified = a.PLND_DELRYSpecified,
                                                           PREQ_DATE = a.PREQ_DATE,
                                                           PREQ_NAME = a.PREQ_NAME,
                                                           PREQ_PRICE = a.PREQ_PRICE,
                                                           PREQ_PRICESpecified = a.PREQ_PRICESpecified,
                                                           PREQ_UNIT_ISO = a.PREQ_UNIT_ISO,
                                                           PRICE_UNIT = a.PRICE_UNIT,
                                                           PRICE_UNITSpecified = a.PRICE_UNITSpecified,
                                                           PURCH_ORG = a.PURCH_ORG,
                                                           PUR_GROUP = a.PUR_GROUP,
                                                           QUANTITY = a.QUANTITY,
                                                           QUANTITYSpecified = a.QUANTITYSpecified,
                                                           REL_DATE = a.REL_DATE,
                                                           SHORT_TEXT = a.SHORT_TEXT,
                                                           STORE_LOC = a.STORE_LOC,
                                                           TRACKINGNO = a.TRACKINGNO,
                                                           UNIT = a.UNIT,
                                                           VAL_TYPE = a.VAL_TYPE


                                                       }).ToArray(),
                                                       solpSAP.IM_PRITEMTEXTList.Select(a => new ModificarSolpWebServiceMOA.BAPIMEREQITEMTEXT
                                                       {
                                                           PREQ_ITEM = a.PREQ_ITEM,
                                                           PREQ_NO = a.PREQ_NO,
                                                           TEXT_FORM = a.TEXT_FORM,
                                                           TEXT_ID = a.TEXT_ID,
                                                           TEXT_LINE = a.TEXT_LINE
                                                       }).ToArray(),
                                                       solpSAP.IM_PRITEMXList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5660
                                                       {
                                                           ACCTASSCAT = a.ACCTASSCAT,
                                                           PREQ_ITEM = a.PREQ_ITEM,
                                                           AGMT_ITEM = a.AGMT_ITEM,
                                                           AGREEMENT = a.AGREEMENT,
                                                           CLOSED = a.CLOSED,
                                                           CREATED_BY = a.CREATED_BY,
                                                           CURRENCY = a.CURRENCY,
                                                           CURRENCY_ISO = a.CURRENCY_ISO,
                                                           DELETE_IND = a.DELETE_IND,
                                                           DELIV_DATE = a.DELIV_DATE,
                                                           DES_VENDOR = a.DES_VENDOR,
                                                           FIXED_VEND = a.FIXED_VEND,
                                                           GR_PR_TIME = a.GR_PR_TIME,
                                                           INFO_REC = a.INFO_REC,
                                                           ITEM_CAT = a.ITEM_CAT,
                                                           MATERIAL = a.MATERIAL,
                                                           MATL_GROUP = a.MATL_GROUP,
                                                           PCKG_NO = a.PCKG_NO,
                                                           PLANT = a.PLANT,
                                                           PLND_DELRY = a.PLND_DELRY,
                                                           PREQ_DATE = a.PREQ_DATE,
                                                           PREQ_NAME = a.PREQ_NAME,
                                                           PREQ_PRICE = a.PREQ_PRICE,
                                                           PREQ_UNIT_ISO = a.PREQ_UNIT_ISO,
                                                           PRICE_UNIT = a.PRICE_UNIT,
                                                           PURCH_ORG = a.PURCH_ORG,
                                                           PUR_GROUP = a.PUR_GROUP,
                                                           QUANTITY = a.QUANTITY,
                                                           REL_DATE = a.REL_DATE,
                                                           SHORT_TEXT = a.SHORT_TEXT,
                                                           STORE_LOC = a.STORE_LOC,
                                                           TRACKINGNO = a.TRACKINGNO,
                                                           UNIT = a.UNIT,
                                                           VAL_TYPE = a.VAL_TYPE,
                                                           PREQ_ITEMX = a.PREQ_ITEMX
                                                       }).ToArray(),
                                                       solpSAP.IM_SERVICEACCOUNTList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5790
                                                       {
                                                           DEL_IND = a.DEL_IND,
                                                           DOC_ITEM = a.DOC_ITEM,
                                                           NET_VALUE = a.NET_VALUE,
                                                           NET_VALUESpecified = a.NET_VALUESpecified,
                                                           QUANTITY = a.QUANTITY,
                                                           OUTLINE = a.OUTLINE,
                                                           PERCENT = a.PERCENT,
                                                           PERCENTSpecified = a.PERCENTSpecified,
                                                           QUANTITYSpecified = a.QUANTITYSpecified,
                                                           SERIAL_NO = a.SERIAL_NO,
                                                           SERIAL_NO_ITEM = a.SERIAL_NO_ITEM,
                                                           SRV_LINE = a.SRV_LINE
                                                       }).ToArray(),
                                                       solpSAP.IM_SERVICEACCOUNTXList.Select(a => new ModificarSolpWebServiceMOA.BAPI_SRV_ACC_DATAX
                                                       {
                                                           DEL_IND = a.DEL_IND,
                                                           DOC_ITEM = a.DOC_ITEM,
                                                           NET_VALUE = a.NET_VALUE,
                                                           QUANTITY = a.QUANTITY,
                                                           OUTLINE = a.OUTLINE,
                                                           PERCENT = a.PERCENT,
                                                           SERIAL_NO = a.SERIAL_NO,
                                                           SERIAL_NO_ITEM = a.SERIAL_NO_ITEM,
                                                           SRV_LINE = a.SRV_LINE
                                                       }).ToArray(),
                                                       solpSAP.IM_SERVICELINESList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5780
                                                       {
                                                           SRV_LINE = a.SRV_LINE,
                                                           QUANTITYSpecified = a.QUANTITYSpecified,
                                                           OUTLINE = a.OUTLINE,
                                                           QUANTITY = a.QUANTITY,
                                                           CURRENCY = a.CURRENCY,
                                                           DEL_IND = a.DEL_IND,
                                                           DOC_ITEM = a.DOC_ITEM,
                                                           GROSS_PRICE = a.GROSS_PRICE,
                                                           GROSS_PRICESpecified = a.GROSS_PRICESpecified,
                                                           MATL_GROUP = a.MATL_GROUP,
                                                           NET_PRICE = a.NET_PRICE,
                                                           NET_PRICESpecified = a.NET_PRICESpecified,
                                                           SERVICE = a.SERVICE,
                                                           SHORT_TEXT = a.SHORT_TEXT,
                                                           UOM = a.UOM,
                                                           UOM_ISO = a.UOM_ISO
                                                       }).ToArray(),
                                                       solpSAP.IM_SERVICELINESXList.Select(a => new ModificarSolpWebServiceMOA.ZMPES5720
                                                       {
                                                           SRV_LINE = a.SRV_LINE,
                                                           OUTLINE = a.OUTLINE,
                                                           QUANTITY = a.QUANTITY,
                                                           CURRENCY = a.CURRENCY,
                                                           DEL_IND = a.DEL_IND,
                                                           DOC_ITEM = a.DOC_ITEM,
                                                           GROSS_PRICE = a.GROSS_PRICE,
                                                           MATL_GROUP = a.MATL_GROUP,
                                                           NET_PRICE = a.NET_PRICE,
                                                           SERVICE = a.SERVICE,
                                                           SHORT_TEXT = a.SHORT_TEXT,
                                                           UOM = a.UOM,
                                                           UOM_ISO = a.UOM_ISO
                                                       }).ToArray(),
                                                       out BAPIRETURN[] EX_RETURN);


            var respuesta = new ModificarSolpConsumerMOAResponse();

            respuesta.Resultado = result;
            respuesta.Errores = new List<ModificarSolpConsumerMOAError>();

            foreach (var errorSAP in EX_RETURN)
            {
                var error = new ModificarSolpConsumerMOAError
                {
                    Codigo = errorSAP.CODE,
                    Mensaje = errorSAP.MESSAGE,
                    Tipo = errorSAP.TYPE
                };

                respuesta.Errores.Add(error);
            }

            var jsonRespuesta = JsonConvert.SerializeObject(respuesta);

            FileInfo fileRespuesta = new FileInfo(rutaArchivoRespuesta);
            fileRespuesta.Directory.Create();
            File.WriteAllText(fileRespuesta.FullName, jsonRespuesta);

            return respuesta;
        }


        //public SolpSAPModificarDto ConvertirSOLPModificar(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas)
        //{
        //    SolpSAPModificarDto solpSAP = new SolpSAPModificarDto();

        //    #region posiciones y servicios
        //    int numeroPosicion = 0;

        //    //•	el problema está en que siempre debes poner en el campo OUT_LINE= "000000001", sino debieras llenar otra tabla de SAP que no la estamos cargando. Para quitarle complejidad se saco dicha tabla.
        //    string outlineNumber = "000000001";
        //    string numeroPaquete = "";
        //    string preqItem = "";
        //    string serialNumber = "";
        //    string serviceAccountSerialNumber = "01";

        //    string docItem = "";

        //    string textId = "B03";
        //    string formatText = "*";

        //    /* Algunas cuestiones con los números que se mandan:
        //     * DOC_ITEM, PREQ_ITEM, OUTLINE, SERIAL_NO, PCKG_NO, corresponden al número de la posicion pero formateados de distintas formas
        //     * SRV_LINE y SERIAL_NO_ITEM, son de la SUBPOSICION, pero también formatodo de distintas formas
        //     * 
        //     */

        //    solpSAP.IM_PR_TYPE = solpActual.ClaseDocumento.CodigoSap;

        //    foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
        //    {
        //        bool eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
        //        bool eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;

        //        eliminarPosicion = eliminarPosicion ? true : !posicion.Estado;
        //        numeroPosicion++;

        //        preqItem = $"{numeroPosicion:00000}";
        //        docItem = preqItem;
        //        numeroPaquete = $"{numeroPosicion:0000000000}";
        //        serialNumber = $"{numeroPosicion:00}";

        //        var IM_PRITEM = new ZMPES5700();

        //        //Nombre: ZBAPIMEREQITEMIMP Denominación:	Posición de SOLPED
        //        IM_PRITEM.PREQ_ITEM = preqItem; //PREQ_ITEM BNFPO Número de posición de la solicitud de pedido
        //        IM_PRITEM.PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(); //PUR_GROUP EKGRP Grupo de compras
        //        IM_PRITEM.CREATED_BY = solpActual.UsuarioCreacion == null ? "" : solpActual.UsuarioCreacion.UsuarioSap; //CREATED_BY ERNAM Nombre del responsable que ha añadido el objeto
        //        IM_PRITEM.PREQ_NAME = posicion.Solicitante; //PREQ_NAME AFNAM Nombre del solicitante
        //        IM_PRITEM.SHORT_TEXT = posicion.Tarea; //SHORT_TEXT TXZ01 Texto breve
        //        IM_PRITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : ""; //MATERIAL MATNR18 Número de material(18 caracteres)
        //        IM_PRITEM.PLANT = posicion.Centro.CodigoSap.ToString(); //PLANT EWERK   Centro
        //        IM_PRITEM.STORE_LOC = solpActual.TipoSolpSap == (int?)TipoSolpSap.Mantenimiento ? "" : posicion.Almacen.CodigoSap.ToString(); //STORE_LOC   LGORT_D Almacén
        //        IM_PRITEM.TRACKINGNO = posicion.NroNecesidad; //TRACKINGNO BEDNR   Número de necesidad
        //        IM_PRITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString(); //MATL_GROUP  MATKL Grupo de artículos

        //        if (posicion.TipoPosicion.Codigo == "MATERIALES") {

        //            IM_PRITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : ""; //MATERIAL MATNR18 Número de material(18 caracteres)
        //            IM_PRITEM.QUANTITY = (Decimal)posicion.Cantidad; //QUANTITY BAMNG   Cantidad solicitud de pedido
        //            IM_PRITEM.QUANTITYSpecified = true;
        //            IM_PRITEM.UNIT = posicion.Unidad.CodigoSap.ToString(); //UNIT BAMEI   Unidad de medida de solicitud pedido
        //            //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido
        //            IM_PRITEM.PREQ_PRICE = (Decimal)posicion.PrecioBruto; //PREQ_PRICE  BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
        //            IM_PRITEM.PREQ_PRICESpecified = true;
        //            //IM_PRITEM.PRICE_UNIT = null; //PRICE_UNIT EPEIN   Cantidad base  
        //            //IM_PRITEM.PRICE_UNITSpecified = true;
        //        }

        //        //IM_PRITEM.UNIT = null; //UNIT BAMEI   Unidad de medida de solicitud pedido
        //        //IM_PRITEM.PREQ_UNIT_ISO = null; //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido
        //        IM_PRITEM.PREQ_DATE = SAPFormatter.PrepararFecha(solpActual.FechaCreacion);  //PREQ_DATE   BADAT Fecha de solicitud
        //        IM_PRITEM.DELIV_DATE = SAPFormatter.PrepararFecha(posicion.FechaEntregaServicio ?? DateTime.Now); 
        //        IM_PRITEM.REL_DATE = null; //REL_DATE    FRGDT Fecha de liberación de la solicitud de pedido
        //                                   // IM_PRITEM.GR_PR_TIME = null; //GR_PR_TIME  WEBAZ Tiempo de tratamiento para la entrada de mercancía en días
        //                                   // IM_PRITEM.PREQ_PRICE = 0; //PREQ_PRICE  BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
        //                                   // IM_PRITEM.PRICE_UNIT = 0; //PRICE_UNIT EPEIN   Cantidad base

        //        //Estos datos se envian en el caso de que la posicion sea de materiales
        //        switch (posicion.TipoPosicion.Codigo.ToLower())
        //        //ITEM_CAT PSTYP   Tipo de posición del documento de compras
        //        {
        //            case "servicio":
        //                IM_PRITEM.ITEM_CAT = "9";
        //                break;

        //            case "materiales":
        //            default:
        //                IM_PRITEM.ITEM_CAT = "0";
        //                break;
        //        }

        //        //Indica el tipo de posicion de la solp
        //        if (posicion.TipoImputacion != null)
        //        //ACCTASSCAT  KNTTP Tipo de imputación
        //        {
        //            switch (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower())
        //            {
        //                case "centrodecosto":
        //                    IM_PRITEM.ACCTASSCAT = "K";
        //                    break;
        //                case "ordendeot":
        //                    IM_PRITEM.ACCTASSCAT = "F";
        //                    break;
        //                case "ordendeinversion":
        //                    IM_PRITEM.ACCTASSCAT = "F";
        //                    break;
        //                case "siniestrobeneficio":
        //                    IM_PRITEM.ACCTASSCAT = "Y";
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            IM_PRITEM.ACCTASSCAT = "";
        //        }

        //        //IM_PRITEM.DES_VENDOR = null; //DES_VENDOR WLIEF   Proveedor deseado
        //        //Contrato marco          
        //        //IM_PRITEM.FIXED_VEND = posicion.ProveedorFijo;; //FIXED_VEND FLIEF   Proveedor fijo
        //        //IM_PRITEM.PURCH_ORG = posicion.OrganizacionCompras; //PURCH_ORG EKORG   Organización de compras        
        //        //IM_PRITEM.AGREEMENT = posicion.NumeroContratoSuperior; //AGREEMENT   KONNR Número del contrato superior
        //        //IM_PRITEM.AGMT_ITEM = posicion.NumeroPosicionContratoSuperior; //AGMT_ITEM   KTPNR Número de posición del contrato superior
        //        //IM_PRITEM.INFO_REC = null; //INFO_REC    INFNR Número del registro info de compras
        //        IM_PRITEM.CLOSED = null; //Contrato marco? No está en este MVP //CLOSED  EBAKZ Solicitud de pedido concluida
        //        IM_PRITEM.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY    WAERS Clave de moneda
        //        IM_PRITEM.CURRENCY_ISO = null; //CURRENCY_ISO BAPIISOCD   Código ISO para moneda
        //        IM_PRITEM.PLND_DELRY = (decimal)posicion.PlazoEntrega; //PLND_DELRY PLIFZ   Plazo de entrega previsto en días
        //        IM_PRITEM.PLND_DELRYSpecified = true;
        //        IM_PRITEM.DELETE_IND = SAPFormatter.FormatearBooleano(eliminarPosicion); //Indica si esta borrada la posicion
        //        IM_PRITEM.PCKG_NO = numeroPaquete; //PCKG_NO PACKNO  Nº paquete

        //        //Agregamos todos los items a la estructura de posicion
        //        solpSAP.IM_PRITEMList.Add(IM_PRITEM);

        //        //Esta es una lista de campos que SAP nos pide que enviemos una "X" con los datos.
        //        solpSAP.IM_PRITEMXList.Add(new ZMPES5660
        //        {
        //            PREQ_ITEM = preqItem,
        //            PREQ_ITEMX = "X",
        //            PUR_GROUP = "X",                 
        //            CREATED_BY = "X",
        //            PREQ_NAME = "X",
        //            SHORT_TEXT = "X",
        //            MATERIAL = (IM_PRITEM.MATL_GROUP != null) ? "X" : "",
        //            PLANT = "X",
        //            STORE_LOC = "X",
        //            TRACKINGNO = "X",
        //            MATL_GROUP = "X",
        //            QUANTITY = ((decimal)IM_PRITEM.QUANTITY == 0) ? "" : "X",
        //            UNIT = "X",
        //            //PREQ_UNIT_ISO = "X",
        //            PREQ_DATE = "X",
        //            DELIV_DATE = "X",
        //            //REL_DATE = "X",
        //            //GR_PR_TIME = "X",
        //            PREQ_PRICE = ((decimal)IM_PRITEM.PREQ_PRICE == 0) ? "" : "X",
        //            //PRICE_UNIT = "X"
        //            ITEM_CAT = "X",
        //            ACCTASSCAT = "X",
        //            //DES_VENDOR = "X",
        //            //FIXED_VEND = "X",
        //            //PURCH_ORG = "X",
        //            //AGREEMENT = "X",
        //            //AGMT_ITEM = "X",
        //            //INFO_REC = "X",
        //            //CLOSED = "X",
        //            CURRENCY = "X",
        //            //CURRENCY_ISO = "X",
        //            PLND_DELRY = "X",
        //            PCKG_NO = "X",
        //            DELETE_IND = posicion.TipoPosicion.Codigo != "MATERIALES" ? SAPFormatter.FormatearBooleano(eliminarPosicion) : "",
        //        });

        //        //Estos datos de imputacion se envian solo para materiales por que en servicio va a nivel de subposicion
        //        if (posicion.TipoPosicion.Codigo == "MATERIALES")
        //        {
        //            if (!solpSAP.IM_PRACCOUNTList.Any(x =>
        //                    x.PREQ_ITEM == preqItem && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
        //                    x.SERIAL_NO == "01" && //SERIAL_NO	DZEKKN	Número actual de la imputación
        //                    x.GL_ACCOUNT == getCodigoTablaSap(posicion.CuentaMayorSap) &&//GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
        //                    x.COSTCENTER == getCodigoTablaSap(posicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
        //                    x.ORDERID == getCodigoTablaSap(posicion.TipoImputacionSap) && //ORDERID	AUFNR	Número de orden
        //                    x.PROFIT_CTR == getCodigoTablaSap(posicion.TipoImputacionSap) //PROFIT_CTR	PRCTR	Centro de beneficio

        //            ))
        //            {
        //                solpSAP.IM_PRACCOUNTList.Add(new ZMPES5690
        //                {
        //                    PREQ_ITEM = preqItem, //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
        //                    SERIAL_NO = "01", //SERIAL_NO	DZEKKN	Número actual de la imputación               
        //                    GL_ACCOUNT = getCodigoTablaSap(posicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
        //                    COSTCENTER = getCodigoTablaSap(posicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
        //                    ORDERID = getCodigoTablaSap(posicion.TipoImputacionSap), //ORDERID	AUFNR	Número de orden
        //                    PROFIT_CTR = getCodigoTablaSap(posicion.TipoImputacionSap), //PROFIT_CTR	PRCTR	Centro de beneficio
        //                    BUS_AREA = "GENE",
        //                    CO_AREA = "MOA"
        //                });


        //                solpSAP.IM_PRACCOUNTXList.Add(new ZMPES5680
        //                {
        //                    PREQ_ITEM = preqItem,
        //                    SERIAL_NO = "01",
        //                    PREQ_ITEMX = "X",
        //                    SERIAL_NOX = "X",
        //                    GL_ACCOUNT = "X",
        //                    COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
        //                    ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
        //                    PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : "",
        //                    BUS_AREA = "X",
        //                    CO_AREA = "X"
        //                });
        //            }
        //            //Este metodo lo usamos para enviar el texto de suministro. Solo se pueden enviar 132 caracteres por linea
        //            var linesTextoSuministro = getLinesFromTextoSuministro(posicion.TextoSuministro);

        //            linesTextoSuministro.ForEach(texto =>
        //            {
        //                solpSAP.IM_PRITEMTEXTList.Add(new BAPIMEREQITEMTEXT
        //                {
        //                    PREQ_ITEM = preqItem,
        //                    TEXT_ID = textId,
        //                    TEXT_FORM = formatText,
        //                    TEXT_LINE = texto
        //                });
        //            });

        //            solpSAP.IM_SERVICEACCOUNTList.Add(new ZMPES5790
        //            {
        //                DOC_ITEM = docItem,
        //                OUTLINE = outlineNumber,                    
        //                SERIAL_NO = "01",                     
        //                //Siempre mandar esto en 100. Lo autocalcula SAP
        //                PERCENT = 100
        //            });

        //            solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
        //            {
        //                DOC_ITEM = docItem,
        //                OUTLINE = outlineNumber, //Preguntar a Ulises          
        //                SERIAL_NO = "01",                     
        //                //Siempre mandar esto en 100. Lo autocalcula SAP
        //                PERCENT = "X"
        //            });

        //        }            

        //        //Desde aca empiezan las subposiciones
        //        var numeroSubPosicion = 0;
        //        var numeroSerialNumberItem = 0;
        //        string serviceLineNumber = "";
        //        string serialNumberItem = "";

        //        foreach (var subPosicion in posicion.Subposiciones.OrderBy(x => x.Id))
        //        {
        //            numeroSubPosicion++;
        //            serviceLineNumber = $"{subPosicion.Numero:000000000}0";


        //            //serialNumberItem = serialNumber;

        //            //SUBPOSICION
        //            var IM_SERVICELINE = new ZMPES5780();

        //            IM_SERVICELINE.DOC_ITEM = docItem; //DOC_ITEM EBELP   Número de posición de la solicitud de pedido = PREQ_ITEM
        //            IM_SERVICELINE.OUTLINE = outlineNumber; //OUTLINE OUTLINE_NO  Número de estructuración
        //            IM_SERVICELINE.SRV_LINE = serviceLineNumber; //SRV_LINE    EXTROW Número de línea
        //            IM_SERVICELINE.DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)); //DEL_IND DEL Indicador de borrado

        //            if (subPosicion.ServicioSolp != null)
        //                IM_SERVICELINE.SERVICE = subPosicion.ServicioSolp.Codigo.ToString(); //SERVICE ASNUM Número de servicio
        //            else
        //                IM_SERVICELINE.SHORT_TEXT = subPosicion.Tarea; //SHORT_TEXT SH_TEXT1    Texto breve

        //            IM_SERVICELINE.QUANTITY = (decimal)subPosicion.Cantidad.Value; //QUANTITY MENGEV  Cantidad con signo +/ -
        //            IM_SERVICELINE.QUANTITYSpecified = true;
        //            IM_SERVICELINE.UOM = subPosicion.Unidad.CodigoSap; //UOM MEINS Unidad de medida base
        //            //IM_SERVICELINE.UOM_ISO = null; //UOM_ISO MEINS_ISO   Unidad medida base en código ISO
        //            IM_SERVICELINE.GROSS_PRICE = (decimal)subPosicion.PrecioBruto.Value; //GROSS_PRICE SBRTWR Precio bruto Unitario
        //            IM_SERVICELINE.GROSS_PRICESpecified = true; 
        //            IM_SERVICELINE.CURRENCY = posicion.Moneda.CodigoSap; //CURRENCY WAERS   Clave de moneda

        //            solpSAP.IM_SERVICELINESList.Add(IM_SERVICELINE);

        //            solpSAP.IM_SERVICELINESXList.Add(new ZMPES5720
        //            {
        //                DOC_ITEM = docItem,
        //                OUTLINE = outlineNumber,
        //                SRV_LINE = serviceLineNumber,
        //                DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)),
        //                SERVICE = (subPosicion.ServicioSolp != null) ? "X" : "",
        //                SHORT_TEXT = (subPosicion.ServicioSolp == null) ? "X" : "",
        //                QUANTITY = "X",
        //                UOM = "X",
        //                //UOM_ISO = "X",
        //                GROSS_PRICE = "X",
        //                CURRENCY = "X"
        //            });

        //            if (!solpSAP.IM_PRACCOUNTList.Any(x =>
        //                    x.PREQ_ITEM == preqItem && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
        //                    x.SERIAL_NO == serialNumber && //SERIAL_NO    DZEKKN  Número actual de la imputación
        //                    x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) && //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor 
        //                    x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
        //                    x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) //ORDERID	AUFNR	Número de orden
        //                ))

        //            {
        //                numeroSerialNumberItem++;

        //                serialNumberItem = $"{numeroSerialNumberItem:00}";


        //                solpSAP.IM_PRACCOUNTList.Add(new ZMPES5690
        //                {
        //                    PREQ_ITEM = preqItem, //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
        //                    SERIAL_NO = serialNumberItem, //SERIAL_NO    DZEKKN  Número actual de la imputación
        //                    QUANTITY = subPosicion.Cantidad.Value, //QUANTITY	MENGE_D	Cantidad
        //                    GL_ACCOUNT = getCodigoTablaSap(subPosicion.CuentaMayorSap), //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
        //                    COSTCENTER = getCodigoTablaSap(subPosicion.TipoImputacionSap), //COSTCENTER	KOSTL	Centro de coste
        //                    ORDERID = getCodigoTablaSap(subPosicion.TipoImputacionSap) //ORDERID	AUFNR	Número de orden                          
        //                }); ;

        //                solpSAP.IM_PRACCOUNTXList.Add(new ZMPES5680
        //                {
        //                    PREQ_ITEM = preqItem,
        //                    SERIAL_NO = serialNumberItem,
        //                    PREQ_ITEMX = "X",
        //                    SERIAL_NOX = "X",
        //                    QUANTITY = "X",
        //                    GL_ACCOUNT = "X",
        //                    COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
        //                    ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : ""                        
        //                });
        //            }
        //            else
        //            {
        //                serialNumberItem = solpSAP.IM_PRACCOUNTList.FirstOrDefault(x =>
        //                    x.PREQ_ITEM == preqItem && //PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
        //                    x.SERIAL_NO == serialNumber && //SERIAL_NO    DZEKKN  Número actual de la imputación
        //                    x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) && //GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
        //                    x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) && //COSTCENTER	KOSTL	Centro de coste
        //                    x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) //ORDERID	AUFNR	Número de orden
        //                ).SERIAL_NO;
        //            }

        //            //IMPUTACION SUBPOSICION
        //            solpSAP.IM_SERVICEACCOUNTList.Add(new ZMPES5790
        //            {
        //                DOC_ITEM = docItem,
        //                OUTLINE = outlineNumber,
        //                SRV_LINE = serviceLineNumber,
        //                SERIAL_NO = serviceAccountSerialNumber,
        //                SERIAL_NO_ITEM = serialNumberItem,
        //                //Siempre mandar esto en 100. Lo autocalcula SAP
        //                PERCENT = 100
        //            });

        //            solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
        //            {
        //                DOC_ITEM = docItem,
        //                OUTLINE = outlineNumber, //Preguntar a Ulises
        //                SRV_LINE = serviceLineNumber, //Preguntar a Ulises
        //                SERIAL_NO = serviceAccountSerialNumber,
        //                SERIAL_NO_ITEM = "X",
        //                //Siempre mandar esto en 100. Lo autocalcula SAP
        //                PERCENT = "X"
        //            });

        //        }

        //        //Estos son los metodos con los que se nos fijamos si se edito algun campo de la direccion de entrega. Si no edito ninguno no 
        //        //hace falta enviar a SAP pero si se edito por lo menos uno tenemos que enviar todos los campos 
        //        CentroDireccion centroPorDefecto = repositorio.Obtener<CentroDireccion>(x => x.CodigoSap == posicion.Centro.CodigoSap);
        //        TablaSap centroPorDefecto2 = repositorio.Obtener<TablaSap>(x => x.Id == posicion.Centro_Id);


        //        if (centroPorDefecto2.Descripcion != posicion.NombreEntrega ||
        //            centroPorDefecto.Cp != posicion.CpEntrega ||
        //            centroPorDefecto2.Descripcion != posicion.Centro.Descripcion ||
        //            centroPorDefecto.Direccion != posicion.CalleEntrega ||
        //            centroPorDefecto.Numero != posicion.NumeroEntrega)
        //        {

        //            solpSAP.IM_PRADDRDELIVERYList.Add(
        //            new ZMPES5750
        //            {
        //                PREQ_NO = preqItem, //PREQ_NO BANFN   Numero de SOLPED
        //                PREQ_ITEM = preqItem, //PREQ_ITEM   BNFPO Número de posición de la solicitud de pedido
        //                NAME = posicion.NombreEntrega, //NAME    AD_NAME1 Nombre 1
        //                POSTL_COD1 = posicion.CpEntrega, //POSTL_COD1 AD_PSTCD1   Código postal de la población
        //                CITY = posicion.Centro.Descripcion, //CITY    AD_CITY1 Población
        //                STREET = posicion.CalleEntrega, //STREET AD_STREET   Calle
        //                TEL1_NUMBR = posicion.NumeroEntrega, //TEL1_NUMBR  AD_TLNMBR1 Primer número teléfono: Prefijo + número
        //            }
        //        );
        //        }
        //    }

        //    #endregion


        //    return solpSAP;
        //}

    }
    public class ModificarSolpConsumerMOAResponse
    {
        public string NumeroSolp { get; set; }
        public List<ModificarSolpConsumerMOAError> Errores { get; set; }
        public string Resultado { get; internal set; }
    }

    public class ModificarSolpConsumerMOAError
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }


    public interface IModificarSolpConsumerMOA
    {
        ModificarSolpConsumerMOAResponse Request(SolpSAPDto solpSAP);
    }

}
