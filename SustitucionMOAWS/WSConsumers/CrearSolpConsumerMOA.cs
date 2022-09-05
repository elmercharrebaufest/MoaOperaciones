using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearSolpConsumerMOA : ICrearSolpConsumerMOA
    {
        private readonly IRepositorio repositorio;
        private readonly SI_MMRFC_CREAR_SOLPEDClient service;
        private readonly string rutaArchivosXmls = ConfigurationManager.AppSettings["RutaArchivosCompras"];

        public CrearSolpConsumerMOA(IRepositorio repositorio)
        {      
            service = new SI_MMRFC_CREAR_SOLPEDClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
            this.repositorio = repositorio;
        }

        public CrearSolpConsumerMOAResponse Request(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas)
        {
            var solpSAP = ConvertirSOLP(solpActual, postEntitySubPosicionesEliminadas);

            var serxml = new System.Xml.Serialization.XmlSerializer(solpSAP.GetType());
            var ms = new MemoryStream();
            serxml.Serialize(ms, solpSAP);
            string xml = Encoding.UTF8.GetString(ms.ToArray());

            var fecha = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");

            var nombreArchivoLlamada = string.Concat(solpActual.Id, " - ", fecha, " - llamada crear.xml");
            var nombreArchivoRespuesta = string.Concat(solpActual.Id, " - ", fecha, " - respuesta crear.xml");

            var rutaArchivoLlamada = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoLlamada);
            var rutaArchivoRespuesta = Path.Combine(rutaArchivosXmls, "XMLS", nombreArchivoRespuesta);

            FileInfo fileCrear = new FileInfo(rutaArchivoLlamada);
            fileCrear.Directory.Create(); 
            File.WriteAllText(fileCrear.FullName, xml);

            var result = service.SI_MMRFC_CREAR_SOLPED(solpSAP.IM_PRACCOUNTList.ToArray(),
                                                       solpSAP.IM_PRACCOUNTXList.ToArray(),
                                                       solpSAP.IM_PRADDRDELIVERYList.ToArray(),
                                                       solpSAP.IM_PRHEADERTEXTList.ToArray(),
                                                       solpSAP.IM_PRITEMList.ToArray(),
                                                       solpSAP.IM_PRITEMTEXTList.ToArray(),
                                                       solpSAP.IM_PRITEMXList.ToArray(),
                                                       solpSAP.IM_PR_TYPE,
                                                       solpSAP.IM_SERVICEACCOUNTList.ToArray(),
                                                       solpSAP.IM_SERVICEACCOUNTXList.ToArray(),
                                                       solpSAP.IM_SERVICELINESList.ToArray(),
                                                       solpSAP.IM_SERVICELINESXList.ToArray(),
                                                       out string EX_PREQ_NO,
                                                       out BAPIRETURN[] EX_RETURN);


            var respuesta = new CrearSolpConsumerMOAResponse();

            respuesta.NumeroSolp = EX_PREQ_NO;
            respuesta.Resultado = result;
            respuesta.Errores = new List<CrearSolpConsumerMOAError>();

            foreach (var errorSAP in EX_RETURN)
            {
                var error = new CrearSolpConsumerMOAError
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


        public SolpSAPDto ConvertirSOLP(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas)
        {

            SolpSAPDto solpSAP = new SolpSAPDto();

            //SERVICELINES ZBAPI_SRV_SERVICE_LINE  Si Subposición
            //SERVICELINESX ZBAPI_SRV_SERVICE_LINEX Si Change Toolbar for Enjoy Purchase Req. - Subposición


            /*
            Nombre	Dominio / Tipo	Denominación
            DOC_ITEM	EBELP	Número de posición de la solicitud de pedido = PREQ_ITEM
            OUTLINE	OUTLINE_NO	Número de estructuración
            SRV_LINE	EXTROW	Número de línea
            DEL_IND	DEL	Indicador de borrado
            SERVICE	ASNUM	Número de servicio
            SHORT_TEXT	SH_TEXT1	Texto breve
            QUANTITY	MENGEV	Cantidad con signo +/-
            UOM	MEINS	Unidad de medida base
            UOM_ISO	MEINS_ISO	Unidad medida base en código ISO
            GROSS_PRICE	SBRTWR	Precio bruto Unitario
            CURRENCY	WAERS	Clave de moneda
            MATL_GROUP	MATKL_SRV	Grupo artículos
            */


            /*¨
                Nombre: ZBAPIMEREQITEMIMP Denominación:	Posición de SOLPED
                Nombre  Dominio / Tipo  Denominación
            */
            #region posiciones y servicios
            int numeroPosicion = 0;

            //•	el problema está en que siempre debes poner en el campo OUT_LINE= "000000001", sino debieras llenar otra tabla de SAP que no la estamos cargando. Para quitarle complejidad se saco dicha tabla.
            string outlineNumber = "000000001";
            string numeroPaquete = "";
            string preqItem = "";
            string serialNumber = "";
            string serviceAccountSerialNumber = "01";

            string docItem = "";

            string textId = "B03";
            string formatText = "*";


            /* Algunas cuestiones con los números que se mandan:
             * DOC_ITEM, PREQ_ITEM, OUTLINE, SERIAL_NO, PCKG_NO, corresponden al número de la posicion pero formateados de distintas formas
             * SRV_LINE y SERIAL_NO_ITEM, son de la SUBPOSICION, pero también formatodo de distintas formas
             * 
             */

            solpSAP.IM_PR_TYPE = solpActual.ClaseDocumento.CodigoSap;

            foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
            {
         
                bool eliminarPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;
                bool eliminarSubPosicion = posicion.Subposiciones.Where(item => !Convert.ToBoolean(item.Estado)).Count() == posicion.Subposiciones.Count;

                eliminarPosicion = eliminarPosicion ? true : !posicion.Estado;

                numeroPosicion++;

                preqItem = $"{numeroPosicion:00000}";
                docItem = preqItem;
                numeroPaquete = $"{numeroPosicion:0000000000}";
                serialNumber = $"{numeroPosicion:00}";

                var IM_PRITEM = new ZMPES5700();
                    //PREQ_ITEM BNFPO Número de posición de la solicitud de pedido
                    //PUR_GROUP EKGRP Grupo de compras
                    //CREATED_BY ERNAM Nombre del responsable que ha añadido el objeto
                    //PREQ_NAME AFNAM Nombre del solicitante
                    //SHORT_TEXT TXZ01 Texto breve
                    //MATERIAL MATNR18 Número de material(18 caracteres)
                    //PLANT EWERK   Centro
                    //STORE_LOC   LGORT_D Almacén
                    //TRACKINGNO BEDNR   Número de necesidad

                IM_PRITEM.PREQ_ITEM = preqItem;
                IM_PRITEM.PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString();
                IM_PRITEM.CREATED_BY = solpActual.UsuarioCreacion != null ? solpActual.UsuarioCreacion.UsuarioSap : repositorio.Obtener<Usuario>(solpActual.UsuarioCreacion_Id).UsuarioSap;
                IM_PRITEM.PREQ_NAME = posicion.Solicitante;
                IM_PRITEM.SHORT_TEXT = posicion.Tarea;


                //if (posicion.TipoPosicion.Codigo == "MATERIALES")
                //{
                //    if (posicion.MaterialSolp != null)
                //    {
                //        IM_PRITEM.MATERIAL = posicion.MaterialSolp.CodigoSap.ToString();
                //    }                
                //}

                //Esto es para el MVP2 ,porque los materiales no tienen sub posiciones


                //if (subposicion.serviciosolp != null)
                //    im_serviceline.service = subposicion.serviciosolp.codigo.tostring();
                //else
                //    im_serviceline.short_text = subposicion.tarea;



                IM_PRITEM.PLANT = posicion.Centro.CodigoSap.ToString();
                IM_PRITEM.STORE_LOC = posicion.Almacen.CodigoSap.ToString();
                IM_PRITEM.TRACKINGNO = posicion.NroNecesidad;


                //MATL_GROUP  MATKL Grupo de artículos
                //QUANTITY BAMNG   Cantidad solicitud de pedido
                //UNIT BAMEI   Unidad de medida de solicitud pedido
                //PREQ_UNIT_ISO BAMEI_ISO   Código ISO p.la unidad de medida en la solicitud de pedido
                //PREQ_DATE   BADAT Fecha de solicitud
                //DELIV_DATE EINDT   Fecha de entrega de posición
                //REL_DATE    FRGDT Fecha de liberación de la solicitud de pedido
                //GR_PR_TIME  WEBAZ Tiempo de tratamiento para la entrada de mercancía en días
                //PREQ_PRICE  BAPICUREXT Importe de moneda para BAPIs(con 9 decimales)
                //PRICE_UNIT EPEIN   Cantidad base
                //ITEM_CAT PSTYP   Tipo de posición del documento de compras
                //ACCTASSCAT  KNTTP Tipo de imputación

                IM_PRITEM.MATL_GROUP = "30015";
                IM_PRITEM.MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString();
        
              


                //QUANTITY = null,
                //UNIT = null,
                //PREQ_UNIT_ISO = null,
                IM_PRITEM.PREQ_DATE = SAPFormatter.PrepararFecha(DateTime.Now);
                IM_PRITEM.DELIV_DATE = SAPFormatter.PrepararFecha(posicion.FechaEntregaServicio??DateTime.Now);
                IM_PRITEM.REL_DATE = null; 
                


                switch(posicion.TipoPosicion.Codigo.ToLower())
                {
                    case "servicio":
                        IM_PRITEM.ITEM_CAT = "9";
                        break;

                    case "materiales":
                    default:
                        IM_PRITEM.ITEM_CAT = "0";
                        break;
                }

                if (posicion.TipoImputacion != null)
                {
                    switch (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower())
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


                //DES_VENDOR WLIEF   Proveedor deseado
                //FIXED_VEND FLIEF   Proveedor fijo
                //PURCH_ORG EKORG   Organización de compras
                //AGREEMENT   KONNR Número del contrato superior
                //AGMT_ITEM   KTPNR Número de posición del contrato superior
                //INFO_REC    INFNR Número del registro info de compras
                //CLOSED  EBAKZ Solicitud de pedido concluida
                //CURRENCY    WAERS Clave de moneda
                //CURRENCY_ISO BAPIISOCD   Código ISO para moneda
                //PLND_DELRY PLIFZ   Plazo de entrega previsto en días
                //PCKG_NO PACKNO  Nº paquete
                
                IM_PRITEM.AGREEMENT = null; //Contrato marco? No está en este MVP
                IM_PRITEM.AGMT_ITEM = null;//Contrato marco? No está en este MVP
                IM_PRITEM.CLOSED = null; //Contrato marco? No está en este MVP
                IM_PRITEM.CURRENCY = posicion.Moneda.CodigoSap;
                IM_PRITEM.PLND_DELRY = (decimal)posicion.PlazoEntrega;
                IM_PRITEM.PLND_DELRYSpecified = true;
                IM_PRITEM.PCKG_NO = numeroPaquete;               
                IM_PRITEM.VAL_TYPE = SAPFormatter.FormatearBooleano(eliminarPosicion);

               
                if (posicion.TipoPosicion.Codigo.ToLower() == "materiales")
                {
                    IM_PRITEM.PREQ_PRICE = (Decimal)posicion.PrecioBruto;
                    IM_PRITEM.PREQ_PRICESpecified = true;
                    //IM_PRITEM.PRICE_UNIT =
                    //IM_PRITEM.PRICE_UNITSpecified = true;
                    IM_PRITEM.UNIT = posicion.Unidad.CodigoSap.ToString();
                    IM_PRITEM.MATERIAL = posicion.MaterialSolp != null && posicion.TipoPosicion.Codigo == "MATERIALES" ? posicion.MaterialSolp.CodigoSap.ToString() : "";
                    IM_PRITEM.QUANTITY = (Decimal)posicion.Cantidad;
                    IM_PRITEM.QUANTITYSpecified = true;
                    //IM_PRITEM.ACCTASSCAT = "";

                }

                solpSAP.IM_PRITEMList.Add(IM_PRITEM);

                solpSAP.IM_PRITEMXList.Add(new ZMPES5660 {
                    PREQ_ITEM = preqItem,
                    PREQ_ITEMX = "X",
                    PUR_GROUP = "X",
                    PREQ_NAME = "X",
                    SHORT_TEXT = "X",
                    QUANTITY = ((decimal)IM_PRITEM.QUANTITY == 0) ? "" : "X",
                    PLANT = "X",
                    STORE_LOC = "X",
                    TRACKINGNO = "X",
                    MATL_GROUP = "X",
                    PREQ_DATE = "X",
                    DELIV_DATE = "X",
                    ITEM_CAT = "X",
                    ACCTASSCAT = (IM_PRITEM.ACCTASSCAT != null) ? "X" : "",
                    //PURCH_ORG = "X",
                    CURRENCY = "X",
                    PLND_DELRY = "X",
                    PCKG_NO = "X",
                    MATERIAL = (IM_PRITEM.MATL_GROUP != null) ?"X":"",
                    DELETE_IND = posicion.TipoPosicion.Codigo != "MATERIALES" ? SAPFormatter.FormatearBooleano(eliminarPosicion) : "",
                    CREATED_BY = "X",
                    PREQ_PRICE = ((decimal)IM_PRITEM.PREQ_PRICE == 0 ) ?"":"X",
                    UNIT = "X",
                    //PRICE_UNIT = "X"
                });


                /*
                DOC_ITEM	EBELP	Número de posición de la solicitud de pedido = PREQ_ITEM
                OUTLINE	OUTLINE_NO	Número de estructuración
                SRV_LINE	EXTROW	Número de línea
                DEL_IND	DEL	Indicador de borrado
                SERVICE	ASNUM	Número de servicio
                SHORT_TEXT	SH_TEXT1	Texto breve
                QUANTITY	MENGEV	Cantidad con signo +/-
                UOM	MEINS	Unidad de medida base
                UOM_ISO	MEINS_ISO	Unidad medida base en código ISO
                GROSS_PRICE	SBRTWR	Precio bruto Unitario
                CURRENCY	WAERS	Clave de moneda
                MATL_GROUP	MATKL_SRV	Grupo artículos
                 */



                if (posicion.TipoPosicion.Codigo == "MATERIALES") 
                {

                    if (!solpSAP.IM_PRACCOUNTList.Any(x =>
                            x.PREQ_ITEM == preqItem &&
                            x.SERIAL_NO == serialNumber &&
                            x.GL_ACCOUNT == getCodigoTablaSap(posicion.CuentaMayorSap) &&//"0000607034" && 
                            x.COSTCENTER == getCodigoTablaSap(posicion.TipoImputacionSap) &&
                            x.ORDERID == getCodigoTablaSap(posicion.TipoImputacionSap) &&
                            x.PROFIT_CTR == getCodigoTablaSap(posicion.TipoImputacionSap)
                            
                    )) 
                    {
                        solpSAP.IM_PRACCOUNTList.Add(new ZMPES5690
                        {
                            PREQ_ITEM = preqItem,
                            SERIAL_NO = serialNumber,
                            QUANTITY = posicion.Cantidad.Value,
                            GL_ACCOUNT = getCodigoTablaSap(posicion.CuentaMayorSap), //"0000607034",
                            COSTCENTER = getCodigoTablaSap(posicion.TipoImputacionSap),
                            ORDERID = getCodigoTablaSap(posicion.TipoImputacionSap),
                            PROFIT_CTR = getCodigoTablaSap(posicion.TipoImputacionSap)
                        }); 
                    }

                    solpSAP.IM_PRACCOUNTXList.Add(new ZMPES5680
                    {
                        PREQ_ITEM = preqItem,
                        SERIAL_NO = serialNumber,
                        PREQ_ITEMX = "X",
                        SERIAL_NOX = "X",
                        QUANTITY = "X",
                        GL_ACCOUNT = "X",
                        COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                        ORDERID = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                        PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : ""
                    });

                    var linesTextoSuministro = getLinesFromTextoSuministro(posicion.TextoSuministro);

                    linesTextoSuministro.ForEach(texto =>
                    {
                       solpSAP.IM_PRITEMTEXTList.Add(new BAPIMEREQITEMTEXT
                       {
                           PREQ_ITEM = preqItem,
                           TEXT_ID = textId,
                           TEXT_FORM = formatText,
                           TEXT_LINE = texto
                       });
                   });
                    
                }
                else
                {
                    //serialNumber = solpSAP.IM_PRACCOUNTList.FirstOrDefault(x =>
                    //    x.PREQ_ITEM == preqItem &&
                    //    x.SERIAL_NO == serialNumber &&
                    //    x.GL_ACCOUNT == getCodigoTablaSap(posicion.CuentaMayorSap) &&//"0000607034" && 
                    //    x.COSTCENTER == getCodigoTablaSap(posicion.TipoImputacionSap) &&
                    //    x.ORDERID == getCodigoTablaSap(posicion.TipoImputacionSap) &&
                    //    x.PROFIT_CTR == getCodigoTablaSap(posicion.TipoImputacionSap)
                    //).SERIAL_NO;

                    solpSAP.IM_SERVICEACCOUNTList.Add(new ZMPES5790
                    {
                        DOC_ITEM = docItem,                                   
                        SERIAL_NO = serviceAccountSerialNumber,
                        SERIAL_NO_ITEM = serialNumber,
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = docItem,                
                        SERIAL_NO = serviceAccountSerialNumber,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });

                }
            






                var numeroSubPosicion = 0;
                var numeroSerialNumberItem = 0;
                string serviceLineNumber = "";
                string serialNumberItem = "";

                foreach (var subPosicion in posicion.Subposiciones.OrderBy(x => x.Id))
                {
                    numeroSubPosicion++;
                    serviceLineNumber = $"{subPosicion.Numero:000000000}0";
                   

                    //serialNumberItem = serialNumber;

                    //SUBPOSICION
                    var IM_SERVICELINE = new ZMPES5780();

                    IM_SERVICELINE.DOC_ITEM = docItem;
                    IM_SERVICELINE.OUTLINE = outlineNumber; //Preguntar a Ulises
                    IM_SERVICELINE.SRV_LINE = serviceLineNumber;
                    IM_SERVICELINE.DEL_IND = SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado));
                    //IM_SERVICELINE.SERVICE = "000000000003005912";//
                    //IM_SERVICELINE.SERVICE = subPosicion.CodigoServicioSap.Codigo.ToString();

                    if (subPosicion.ServicioSolp != null)
                        IM_SERVICELINE.SERVICE = subPosicion.ServicioSolp.Codigo.ToString();
                    else 
                        IM_SERVICELINE.SHORT_TEXT = subPosicion.Tarea;

                    IM_SERVICELINE.QUANTITY = (decimal)subPosicion.Cantidad.Value;
                    IM_SERVICELINE.QUANTITYSpecified = true;
                    IM_SERVICELINE.UOM = subPosicion.Unidad.CodigoSap;
                    IM_SERVICELINE.GROSS_PRICE = (decimal)subPosicion.PrecioBruto.Value;
                    IM_SERVICELINE.GROSS_PRICESpecified = true;

                    IM_SERVICELINE.CURRENCY = posicion.Moneda.CodigoSap;
                    //IM_SERVICELINE.MATL_GROUP = subPosicion.CuentaMayorSap.CodigoSap;
                    //IM_SERVICELINE.MATL_GROUP = "30015"; // subPosicion.CuentaMayorSap.CodigoSap;

                    solpSAP.IM_SERVICELINESList.Add(IM_SERVICELINE);

                    solpSAP.IM_SERVICELINESXList.Add(new ZMPES5720
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber, //Preguntar a Ulises
                        SRV_LINE = serviceLineNumber,
                        DEL_IND = eliminarSubPosicion ? "" : SAPFormatter.FormatearBooleano(!Convert.ToBoolean(subPosicion.Estado)),
                        SERVICE = (subPosicion.ServicioSolp != null) ? "X" : "",
                        SHORT_TEXT = (subPosicion.ServicioSolp == null) ? "X" : "",
                        QUANTITY = "X",
                        UOM = "X",
                        GROSS_PRICE = "X",
                        CURRENCY = "X",
                        //MATL_GROUP = "X",
                    });

                    

                    /*
                       Nombre: ZBAPIMEREQACCOUNT		Denominación:	Imputación
                       Nombre	Dominio / Tipo	Denominación
                       PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                       SERIAL_NO	DZEKKN	Número actual de la imputación
                       QUANTITY	MENGE_D	Cantidad
                       GL_ACCOUNT	SAKNR	Número de la cuenta de mayor
                       BUS_AREA	GSBER	División
                       COSTCENTER	KOSTL	Centro de coste
                       ASSET_NO	ANLN1	Número principal de activo fijo
                       SUB_NUMBER	ANLN2	Subnúmero de activo fijo
                       ORDERID	AUFNR	Número de orden
                       CO_AREA	KOKRS	Sociedad CO
                       COSTOBJECT	KSTRG	Objeto de coste
                       PROFIT_CTR	PRCTR	Centro de beneficio
                   */




                    if (!solpSAP.IM_PRACCOUNTList.Any(x => 
                            x.PREQ_ITEM == preqItem &&
                            x.SERIAL_NO == serialNumber &&
                            x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) &&//"0000607034" && 
                            x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) &&
                            x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) &&
                            x.PROFIT_CTR == getCodigoTablaSap(subPosicion.TipoImputacionSap)
                        ))
                    {
                        

                        numeroSerialNumberItem++;

                        serialNumberItem = $"{numeroSerialNumberItem:00}";


                        solpSAP.IM_PRACCOUNTList.Add(new ZMPES5690
                        {
                            PREQ_ITEM = preqItem,
                            SERIAL_NO = serialNumberItem,
                            QUANTITY = subPosicion.Cantidad.Value,
                            GL_ACCOUNT = getCodigoTablaSap(subPosicion.CuentaMayorSap), //"0000607034",
                            COSTCENTER = getCodigoTablaSap(subPosicion.TipoImputacionSap),
                            ORDERID = getCodigoTablaSap(subPosicion.TipoImputacionSap),
                            PROFIT_CTR = getCodigoTablaSap(subPosicion.TipoImputacionSap)
                        });;

                        solpSAP.IM_PRACCOUNTXList.Add(new ZMPES5680
                        {
                            PREQ_ITEM = preqItem,
                            SERIAL_NO = serialNumberItem,
                            PREQ_ITEMX = "X",
                            SERIAL_NOX ="X",
                            QUANTITY = "X",
                            GL_ACCOUNT = "X",
                            COSTCENTER = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "centrodecosto") ? "X" : "",
                            ORDERID =  (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeot" || getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "ordendeinversion") ? "X" : "",
                            PROFIT_CTR = (getCodigoTablaGeneral(posicion.TipoImputacion).ToLower() == "siniestrobeneficio") ? "X" : ""
                        });
                    }
                    else
                    {
                        serialNumberItem = solpSAP.IM_PRACCOUNTList.FirstOrDefault(x =>
                            x.PREQ_ITEM == preqItem &&
                            x.SERIAL_NO == serialNumber &&
                            x.GL_ACCOUNT == getCodigoTablaSap(subPosicion.CuentaMayorSap) &&//"0000607034" && 
                            x.COSTCENTER == getCodigoTablaSap(subPosicion.TipoImputacionSap) &&
                            x.ORDERID == getCodigoTablaSap(subPosicion.TipoImputacionSap) &&
                            x.PROFIT_CTR == getCodigoTablaSap(subPosicion.TipoImputacionSap)
                        ).SERIAL_NO;
                    }

                    //IMPUTACION SUBPOSICION
                    solpSAP.IM_SERVICEACCOUNTList.Add(new ZMPES5790
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber,
                        SRV_LINE = serviceLineNumber,
                        SERIAL_NO = serviceAccountSerialNumber,
                        SERIAL_NO_ITEM = serialNumberItem,
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });

                    solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = docItem,
                        OUTLINE = outlineNumber, //Preguntar a Ulises
                        SRV_LINE = serviceLineNumber, //Preguntar a Ulises
                        SERIAL_NO = serviceAccountSerialNumber,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
                    });

                }

                /*
                 *  PREQ_NO	BANFN	Numero de SOLPED
                    PREQ_ITEM	BNFPO	Número de posición de la solicitud de pedido
                    ADDR_NO	AD_ADDRNUM	Nº dirección
                    NAME	AD_NAME1	Nombre 1
                    POSTL_COD1	AD_PSTCD1	Código postal de la población
                    CITY	AD_CITY1	Población
                    STREET	AD_STREET	Calle
                    STREET_NO	AD_STRNUM	Codificación de la calle para fichero de población y calle
                    TEL1_NUMBR	AD_TLNMBR1	Primer número teléfono: Prefijo + número
                    */

                CentroDireccion centroPorDefecto = repositorio.Obtener<CentroDireccion>(x => x.CodigoSap == posicion.Centro.CodigoSap);
                TablaSap centroPorDefecto2 = repositorio.Obtener<TablaSap>(x => x.Id == posicion.Centro_Id);


                if (centroPorDefecto2.Descripcion != posicion.NombreEntrega ||
                    centroPorDefecto.Cp != posicion.CpEntrega ||
                    centroPorDefecto2.Descripcion != posicion.Centro.Descripcion ||
                    centroPorDefecto.Direccion != posicion.CalleEntrega ||
                    centroPorDefecto.Numero != posicion.NumeroEntrega)
                {
                    solpSAP.IM_PRADDRDELIVERYList.Add(
                    new ZMPES5750
                    {
                        PREQ_NO = preqItem,
                        PREQ_ITEM = preqItem,
                        NAME = posicion.NombreEntrega,
                        POSTL_COD1 = posicion.CpEntrega,
                        CITY = posicion.Centro.Descripcion,
                        STREET = posicion.CalleEntrega,
                        TEL1_NUMBR = posicion.NumeroEntrega
                    });
                }          
            }

            #endregion

            return solpSAP;
        }
    }

    public class CrearSolpConsumerMOAResponse
    {
        public string NumeroSolp { get; set; }
        public List<CrearSolpConsumerMOAError> Errores { get; set; }
        public string Resultado { get; internal set; }
    }

    public class CrearSolpConsumerMOAError
    {
        public string Codigo { get; set; }
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }

    public class SolpSAPDto
    {
        public List<ZMPES5690> IM_PRACCOUNTList { get; set; } //OK
        public List<ZMPES5680> IM_PRACCOUNTXList { get; set; } //OK
        public List<ZMPES5750> IM_PRADDRDELIVERYList { get; set; } //OK
        public List<BAPIMEREQHEADTEXT> IM_PRHEADERTEXTList { get; set; } //OK
        public List<ZMPES5700> IM_PRITEMList { get; set; } //OK
        public List<BAPIMEREQITEMTEXT> IM_PRITEMTEXTList { get; set; } 
        public List<ZMPES5660> IM_PRITEMXList { get; set; } //OK?
        public string IM_PR_TYPE { get; set; } //OK
        public List<ZMPES5790> IM_SERVICEACCOUNTList { get; set; } //OK
        public List<BAPI_SRV_ACC_DATAX> IM_SERVICEACCOUNTXList { get; set; } //OK
        public List<ZMPES5780> IM_SERVICELINESList { get; set; } //OK
        public List<ZMPES5720> IM_SERVICELINESXList { get; set; } //OK?


        public SolpSAPDto ()
        {
            IM_PR_TYPE = "";
            IM_PRACCOUNTList = new List<ZMPES5690>();
            IM_PRACCOUNTXList = new List<ZMPES5680>();

            IM_PRADDRDELIVERYList = new List<ZMPES5750>();
            IM_PRHEADERTEXTList = new List<BAPIMEREQHEADTEXT>();

            IM_PRITEMList = new List<ZMPES5700>();
            IM_PRITEMTEXTList = new List<BAPIMEREQITEMTEXT>();

            IM_PRITEMXList = new List<ZMPES5660>();

            IM_SERVICEACCOUNTList = new List<ZMPES5790>();
            IM_SERVICEACCOUNTXList = new List<BAPI_SRV_ACC_DATAX>();
            IM_SERVICELINESList = new List<ZMPES5780>();
            IM_SERVICELINESXList = new List<ZMPES5720>();
        }
    }

    public interface ICrearSolpConsumerMOA
    {
        CrearSolpConsumerMOAResponse Request(Solp solpActual, SolpPosicion postEntitySubPosicionesEliminadas);

    }
}
