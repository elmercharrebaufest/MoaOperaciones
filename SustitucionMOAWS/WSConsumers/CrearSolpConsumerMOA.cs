using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAFotmatter;
using SustitucionMOAModel.Entities;
using SustitucionMOAWS.CrearSolpWebServiceMOA;
using SustitucionMOAWS.CredentialService;

namespace SustitucionMOAWS.WSConsumers
{
    public class CrearSolpConsumerMOA : ICrearSolpConsumerMOA
    {
        private readonly SI_MMRFC_CREAR_SOLPEDClient service;

        public CrearSolpConsumerMOA()
        {
            service = new SI_MMRFC_CREAR_SOLPEDClient();
            service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
            service.ClientCredentials.UserName.Password = SAPCredential.getPassword();
        }

        public object Request(Solp solpActual)
        {
            var solpSAP = ConvertirSOLP(solpActual);

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


            return result;
        }

        public object Map()
        {
            return null;
        }




        public SolpSAPDto ConvertirSOLP(Solp solpActual)
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
            string outlineNumber = "";
            string numeroPaquete = "";
            string preqItem = "";
            string serialNumber = "";
            int numeroDireccion = 0;

            /* Algunas cuestiones con los números que se mandan:
             * DOC_ITEM, PREQ_ITEM, OUTLINE, SERIAL_NO, PCKG_NO, corresponden al número de la posicion pero formateados de distintas formas
             * SRV_LINE y SERIAL_NO_ITEM, son de la SUBPOSICION, pero también formatodo de distintas formas
             * 
             */

            solpSAP.IM_PR_TYPE = "NB";


            foreach (var posicion in solpActual.Posiciones.OrderBy(x => x.Id))
            {
                numeroPosicion++;

                preqItem = $"{numeroPosicion:0000}0";
                numeroPaquete = $"{numeroPosicion:0000000000}";
                serialNumber = $"{numeroPosicion:00}";

                solpSAP.IM_PRITEMList.Add(new ZMPES5700
                {
                    //PREQ_ITEM BNFPO Número de posición de la solicitud de pedido
                    //PUR_GROUP EKGRP Grupo de compras
                    //CREATED_BY ERNAM Nombre del responsable que ha añadido el objeto
                    //PREQ_NAME AFNAM Nombre del solicitante
                    //SHORT_TEXT TXZ01 Texto breve
                    //MATERIAL MATNR18 Número de material(18 caracteres)
                    //PLANT EWERK   Centro
                    //STORE_LOC   LGORT_D Almacén
                    //TRACKINGNO BEDNR   Número de necesidad

                    PREQ_ITEM = preqItem,
                    PUR_GROUP = posicion.GrupoCompras.CodigoSap.ToString(),
                    CREATED_BY = solpActual.UsuarioCreacion.Mail,
                    PREQ_NAME = posicion.Solicitante,
                    SHORT_TEXT = posicion.TextoGenerico,
                    MATERIAL = null, //Esto es para el MVP2 ,porque los materiales no tienen sub posiciones
                    PLANT = posicion.Centro.CodigoSap.ToString(),
                    STORE_LOC = posicion.Almacen.CodigoSap.ToString(),
                    TRACKINGNO = posicion.NroNecesidad,


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

                    MATL_GROUP = posicion.GrupoArticulo.CodigoSap.ToString(),
                    //QUANTITY = null,
                    UNIT = null,
                    PREQ_UNIT_ISO = null,
                    PREQ_DATE = DateTime.Now.ToString(),
                    DELIV_DATE = posicion.FechaEntregaServicio.ToString(),
                    REL_DATE = null, //Calculan ellos ?
                   // GR_PR_TIME = null, 
                   // PREQ_PRICE = 0, //Calcular el precio de todas las subposiciones?
                   // PRICE_UNIT = 0,
                    ITEM_CAT = posicion.TipoPosicion.Codigo,
                    ACCTASSCAT = posicion.TipoImputacion.Codigo,

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

                    DES_VENDOR = null, //Preguntar a Ulises
                    FIXED_VEND = null, //Preguntar
                    PURCH_ORG = posicion.GrupoCompras.CodigoSap,
                    AGREEMENT = null, //Contrato marco? No está en este MVP
                    AGMT_ITEM = null, //Contrato marco? No está en este MVP
                    CLOSED = null, //Contrato marco? No está en este MVP
                    CURRENCY = posicion.Moneda.CodigoSap,
                    CURRENCY_ISO = posicion.Moneda.CodigoSap,
                    PLND_DELRY = (decimal)posicion.PlazoEntrega,
                    PCKG_NO = numeroPaquete
                });

                solpSAP.IM_PRITEMXList.Add(new ZMPES5660 {
                    PREQ_ITEM = preqItem,
                    PREQ_ITEMX = "X",
                    PUR_GROUP = "X",
                    CREATED_BY = "X",
                    PREQ_NAME = "X",
                    SHORT_TEXT = "X",
                    PLANT = "X",
                    STORE_LOC = "X",
                    TRACKINGNO = "X",
                    MATL_GROUP = "X",
                    PREQ_DATE = "X",
                    DELIV_DATE = "X",
                    ITEM_CAT = posicion.TipoPosicion.Codigo,
                    ACCTASSCAT = posicion.TipoImputacion.Codigo,
                    PURCH_ORG = "X",
                    CURRENCY = "X",
                    CURRENCY_ISO = "X",
                    PLND_DELRY = "X",
                    PCKG_NO = "X"

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

                var numeroSubPosicion = 0;
                string serviceLineNumber = "";
                string serialNumberItem = "";
                foreach (var subPosicion in posicion.Subposiciones.OrderBy(x => x.Id))
                {
                    numeroSubPosicion++;
                    serviceLineNumber = $"{numeroSubPosicion:000000000}0";
                   
                    serialNumberItem = $"{numeroSubPosicion:00}";

                    //SUBPOSICION
                    solpSAP.IM_SERVICELINESList.Add(new ZMPES5780
                    {
                        DOC_ITEM = numeroPosicion.ToString(),
                        OUTLINE = outlineNumber, //Preguntar a Ulises
                        SRV_LINE = serviceLineNumber,
                        //DEL_IND = SAPFormatter.FormatearBooleano(posicion.FechaBaja != null),
                        SERVICE = subPosicion.CodigoServicioSap.ToString(),
                        SHORT_TEXT = subPosicion.Tarea,
                        QUANTITY = subPosicion.Cantidad??0,
                        UOM = subPosicion.Unidad.CodigoSap,
                        UOM_ISO = subPosicion.Unidad.CodigoSap,
                        GROSS_PRICE = subPosicion.PrecioBruto??0,
                        CURRENCY = posicion.Moneda.CodigoSap,
                        MATL_GROUP = subPosicion.CuentaMayorSap.CodigoSap,
                    });

                    solpSAP.IM_SERVICELINESXList.Add(new ZMPES5720
                    {
                        DOC_ITEM = numeroPosicion.ToString(),
                        OUTLINE = outlineNumber, //Preguntar a Ulises
                        SRV_LINE = serviceLineNumber,
                        //DEL_IND = SAPFormatter.FormatearBooleano(posicion.FechaBaja != null),
                        SERVICE = "X",
                        SHORT_TEXT = "X",
                        QUANTITY = "X",
                        UOM = "X",
                        UOM_ISO = "X",
                        GROSS_PRICE = "X",
                        CURRENCY = "X",
                        MATL_GROUP = "X",
                    });


                    //IMPUTACION SUBPOSICION
                    solpSAP.IM_SERVICEACCOUNTList.Add(new ZMPES5790
                    {
                        DOC_ITEM = numeroPosicion.ToString(),
                        OUTLINE = outlineNumber, //Preguntar a Ulises
                        SRV_LINE = serviceLineNumber, //Preguntar a Ulises
                        SERIAL_NO = serialNumber,
                        SERIAL_NO_ITEM = serialNumberItem,
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = 100
                    });


                    solpSAP.IM_SERVICEACCOUNTXList.Add(new BAPI_SRV_ACC_DATAX
                    {
                        DOC_ITEM = numeroPosicion.ToString(),
                        OUTLINE = outlineNumber, //Preguntar a Ulises
                        SRV_LINE = serviceLineNumber, //Preguntar a Ulises
                        SERIAL_NO = serialNumber,
                        SERIAL_NO_ITEM = "X",
                        //Siempre mandar esto en 100. Lo autocalcula SAP
                        PERCENT = "X"
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

                    //Agrupar por subposición?
                    //Valido con Ulises

                    if (!solpSAP.IM_PRACCOUNTList.Any(x => 
                            x.PREQ_ITEM == preqItem &&
                            x.SERIAL_NO == serialNumber &&
                            x.GL_ACCOUNT == subPosicion.CuentaMayorSap.CodigoSap &&
                            x.COSTCENTER == posicion.Centro.CodigoSap
                        ))
                    {
                        solpSAP.IM_PRACCOUNTList.Add(new ZMPES5690
                        {
                            PREQ_ITEM = preqItem,
                            SERIAL_NO = serialNumber,
                            GL_ACCOUNT = subPosicion.CuentaMayorSap.CodigoSap,
                            COSTCENTER = posicion.Centro.CodigoSap,
                        });

                        solpSAP.IM_PRACCOUNTXList.Add(new ZMPES5680
                        {
                            PREQ_ITEM = preqItem,
                            SERIAL_NO = serialNumber,
                            GL_ACCOUNT = "X",
                            COSTCENTER = "X"
                        });
                    }
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


                numeroDireccion++;
                solpSAP.IM_PRADDRDELIVERYList.Add(
                    new ZMPES5750
                    {
                        PREQ_NO = solpActual.NroSolp,
                        PREQ_ITEM = preqItem,
                        ADDR_NO = numeroDireccion.ToString(),
                        NAME = posicion.NombreEntrega,
                        POSTL_COD1 = posicion.CpEntrega,
                        CITY = posicion.Centro.Descripcion,
                        STREET = posicion.CalleEntrega,
                        HOUSE_NO = "", // Validar con Ulises el tema del telefono/número
                        TEL1_NUMBR = posicion.NumeroEntrega, //Validar si es el numero entrega o de donde lo sacamos
                    }
                );
            }

            #endregion

            return solpSAP;
        }
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
    }

    public interface ICrearSolpConsumerMOA
    {
        object Request(Solp solpActual);

    }
}
