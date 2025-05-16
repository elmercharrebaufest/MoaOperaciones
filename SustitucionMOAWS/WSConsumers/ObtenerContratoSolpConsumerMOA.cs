using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOAWS.CredentialService;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAWS.ObtenerContratoSolpWebServiceMOA;
using SustitucionMOAWS.WS_GAQ_sin_PI_DIRECT_COMPRAS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SustitucionMOAWS.WSConsumers
{
    public class ObtenerContratoSolpConsumerMOA : IObtenerContratoSolpConsumerMOA
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        public ObtenerContratoSolpConsumerMOA()
        {
        }

        ContratoSolpWSMOAResponse IObtenerContratoSolpConsumerMOA.Request(string numeroContrato, string centro)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var agent = new Z_WS_MOAOP_COMPRAS_DIRECTClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    string IM_COMP_CODE = "MOA";
                    string IM_DETAIL = "X";
                    string IM_ITEM_NO = "00000";
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5800[] IM_MATERIAL   = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5800[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5880[] IM_NOM_VENDOR = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5880[] { };
                    string IM_PLANT = centro;
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5810[] IM_TEXT_POS = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5810[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5870[] IM_VENDOR   = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5870[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5890[] EX_HEADER   = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5890[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5900[] EX_ITEM     = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5900[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRETURN[] EX_RETURN  = new WS_GAQ_sin_PI_DIRECT_COMPRAS.BAPIRETURN[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5910[] EX_SUB_ITEM = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5910[] { };
                    WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7150[] IM_CONTRACT = new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7150[] { new WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES7150 {
                    LOW = numeroContrato,
                    HIGH = numeroContrato,
                    OPTION = "EQ",
                    SIGN = "I"
                    }};

                    var request = new Z_MMRFC_OBTENER_CONTRATO()
                    {
                        IM_COMP_CODE = IM_COMP_CODE,
                        IM_CONTRACT = IM_CONTRACT,
                        IM_DETAIL = IM_DETAIL,
                        IM_ITEM_NO = IM_ITEM_NO,
                        IM_MATERIAL = IM_MATERIAL,
                        IM_NOM_VENDOR = IM_NOM_VENDOR,
                        IM_PLANT = IM_PLANT,
                        IM_TEXT_POS = IM_TEXT_POS,
                        IM_VENDOR = IM_VENDOR
                    };
                    var response = agent.Z_MMRFC_OBTENER_CONTRATO(request);
                    return MapSinPI(response);
                }
                else
                {
                    SI_MMRFC_OBTENER_CONTRATOClient service;
                    var url = "http://gslopidevqa00.molinosagro.ad:50000/XISOAPAdapter/MessageServlet?senderParty=&amp;senderService=BC_MOA_Operaciones&amp;receiverParty=&amp;receiverService=&amp;interface=SI_MMRFC_OBTENER_CONTRATO&amp;interfaceNamespace=urn%3AOPERACIONES";
                    service = new SI_MMRFC_OBTENER_CONTRATOClient(SAPCredential.CrearSapLongBinding(), SAPCredential.DevolverEndpoint(url));
                    service.ClientCredentials.UserName.UserName = SAPCredential.getUserName();
                    service.ClientCredentials.UserName.Password = SAPCredential.getPassword();

                    string IM_COMP_CODE = "MOA";
                    string IM_DETAIL = "X";
                    string IM_ITEM_NO = "00000";
                    ObtenerContratoSolpWebServiceMOA.ZMPES5800[] IM_MATERIAL   = new ObtenerContratoSolpWebServiceMOA.ZMPES5800[] { };
                    ObtenerContratoSolpWebServiceMOA.ZMPES5880[] IM_NOM_VENDOR = new ObtenerContratoSolpWebServiceMOA.ZMPES5880[] { };
                    string IM_PLANT = centro;
                    ObtenerContratoSolpWebServiceMOA.ZMPES5810[] IM_TEXT_POS = new ObtenerContratoSolpWebServiceMOA.ZMPES5810[] { };
                    ObtenerContratoSolpWebServiceMOA.ZMPES5870[] IM_VENDOR   = new ObtenerContratoSolpWebServiceMOA.ZMPES5870[] { };
                    ObtenerContratoSolpWebServiceMOA.ZMPES5890[] EX_HEADER   = new ObtenerContratoSolpWebServiceMOA.ZMPES5890[] { };
                    ObtenerContratoSolpWebServiceMOA.ZMPES5900[] EX_ITEM     = new ObtenerContratoSolpWebServiceMOA.ZMPES5900[] { };
                    ObtenerContratoSolpWebServiceMOA.BAPIRETURN[] EX_RETURN  = new ObtenerContratoSolpWebServiceMOA.BAPIRETURN[] { };
                    ObtenerContratoSolpWebServiceMOA.ZMPES5910[] EX_SUB_ITEM = new ObtenerContratoSolpWebServiceMOA.ZMPES5910[] { };
                    ObtenerContratoSolpWebServiceMOA.ZMPES7150[] IM_CONTRACT = new ObtenerContratoSolpWebServiceMOA.ZMPES7150[] { new ObtenerContratoSolpWebServiceMOA.ZMPES7150 {
                    LOW = numeroContrato,
                    HIGH = numeroContrato,
                    OPTION = "EQ",
                    SIGN = "I"
                    }};

                    string resultado = service.SI_MMRFC_OBTENER_CONTRATO(IM_COMP_CODE, IM_CONTRACT, IM_DETAIL, IM_ITEM_NO, IM_MATERIAL, IM_NOM_VENDOR, IM_PLANT, IM_TEXT_POS, IM_VENDOR, out EX_HEADER, out EX_ITEM, out EX_RETURN, out EX_SUB_ITEM);

                    return Map(resultado, EX_HEADER, EX_ITEM, EX_RETURN, EX_SUB_ITEM);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected ContratoSolpWSMOAResponse Map(string resultado, ObtenerContratoSolpWebServiceMOA.ZMPES5890[] EX_HEADER, ObtenerContratoSolpWebServiceMOA.ZMPES5900[] EX_ITEM, ObtenerContratoSolpWebServiceMOA.BAPIRETURN[] EX_RETURN, ObtenerContratoSolpWebServiceMOA.ZMPES5910[] EX_SUB_ITEM)
        {
            ContratoSolpWSMOAResponse result = new ContratoSolpWSMOAResponse();
            result.ContratosSolp = new List<ContratoSolp> { };

            if (EX_HEADER.Length == 0 && EX_ITEM.Length == 0)
            {
                return result;
            }

            var numeroDocumentoCompras = "";

            foreach (var contratoCabecera in EX_HEADER)
            {
                if (numeroDocumentoCompras != contratoCabecera.NUMBER)
                {
                    numeroDocumentoCompras = contratoCabecera.NUMBER;

                    var contrato = new ContratoSolp()
                    {
                        //Resultado de Cabecera de Contratos Marco
                        NumeroDocumentoCompras = contratoCabecera.NUMBER, //NUMBER  EBELN Número del documento de compras
                        Sociedad = contratoCabecera.COMP_CODE, //COMP_CODE BUKRS   Sociedad
                        IndicadorDeBorrado = contratoCabecera.DELETE_IND_HDR, //DELETE_IND_HDR  ELOEK Indicador de borrado en el documento de compras
                        NumeroCuentaProveedor = contratoCabecera.VENDOR, //VENDOR  ELIFN Número de cuenta del proveedor
                        NombreProveedor = contratoCabecera.NAM_VENDOR,  //NAM_VENDOR LFA1-NAME1  Nombre del Proveedor
                        OrganizacionCompras = contratoCabecera.PURCH_ORG, //PURCH_ORG   EKORG Organización de compras
                        GrupoCompras = contratoCabecera.PUR_GROUP, //PUR_GROUP BKGRP   Grupo de compras
                        ClaveMoneda = contratoCabecera.CURRENCY, //CURRENCY    WAERS Clave de moneda
                        InicioPeriodoValidez = contratoCabecera.VPER_START, //VPER_START KDATB   In.período validez
                        FinPeriodoValidez = contratoCabecera.VPER_END//VPER_END KDATE   Fin período validez
                    };

                    contrato.Posiciones = EX_ITEM.Where(x => x.NUMBER == contrato.NumeroDocumentoCompras).Select(pos => new ContratoSolpPosicion
                    {
                        NumeroDocumentoCompras = pos.NUMBER, //NUMBER  EBELN   Número del documento de compras
                        NumeroPosicionDocumentoCompras = pos.ITEM_NO, //ITEM_NO EBELP   Número de posición del documento de compras
                        IndicadorDeBorrado = pos.DELETE_IND, //DELETE_IND  ELOEK   Indicador de borrado en el documento de compras
                        NumeroMaterial = pos.MATERIAL, //MATERIAL    MATNR18 Número de material (18 caracteres)
                        TextoMaterialOServicio = pos.SHORT_TEXT, //SHORT_TEXT  TXZ01   Texto de Material o Servicio
                        Centro = pos.PLANT, //PLANT   WERKS_D Centro
                        Almacen = pos.STGE_LOC, //STGE_LOC    LGORT_D Almacén
                        CantidadPrevista = pos.TARGET_QTY, //TARGET_QTY  KTMNG   Cantidad prevista
                        UnidadMedida = pos.PO_UNIT, //PO_UNIT BSTME   Unidad de medida de pedido
                        ImporteMonedaBapi = pos.NET_PRICE, //NET_PRICE   BAPICUREXT  Importe de moneda para BAPIs (con 9 decimales)
                        TipoPosicionDocumentoCompras = pos.ITEM_CAT, //ITEM_CAT    PSTYP   Tipo de posición del documento de compras
                        TipoImputacionCompras = pos.ACCTASSCAT, //ACCTASSCAT  KNTTP   Tipo de imputación
                        NumeroPaquete = pos.PCKG_NO, //PCKG_NO PACKNO  Nº paquete
                        GrupoArticuloMateriales = pos.MATKL, //MATKL   MATKL   Grupo de Articulo de Materiales
                        SubPosiciones = EX_SUB_ITEM.Length > 0 ? ObtenerSubPosiciones(pos, EX_SUB_ITEM) : new List<ContratoSolpSubposicion>()
                    }).ToList();

                    result.ContratosSolp.Add(contrato);
                }
            }
            result.error = resultado;

            return result;
        }

        private List<ContratoSolpSubposicion> ObtenerSubPosiciones(ObtenerContratoSolpWebServiceMOA.ZMPES5900 posicion, ObtenerContratoSolpWebServiceMOA.ZMPES5910[] subPosiciones)
        {
            return subPosiciones.Where(x => x.NUMBER == posicion.NUMBER && x.ITEM_NO == posicion.ITEM_NO).Select(subPos => new ContratoSolpSubposicion
            {

                //Resultado de Subosiciones del Contratos Marco           
                NumeroDocumentoCompras = subPos.NUMBER, //NUMBER  EBELN   Número del documento de compras
                NumeroPosicionDocumentoCompras = subPos.ITEM_NO, //ITEM_NO EBELP   Número de posición del documento de compras
                Subposicion = subPos.EXTROW, //EXTROW  EXTROW  Subposición
                NumeroServicio = subPos.SRVPOS, //SRVPOS  ASNUM   Número de servicio
                TextoBreve = subPos.KTEXT1, //KTEXT1  SH_TEXT1    Texto breve
                CantidadPositivoONegativo = subPos.MENGE, //MENGE   MENGEV  Cantidad con signo +/-
                UnidadMedidaBase = subPos.MEINS, //MEINS   MEINS   Unidad de medida base
                PrecioUnitario = subPos.TBTWR, //TBTWR   SBRTWR  Precio Unitario
                PrecioTotal = subPos.NETWR, //NETWR   SNETWR  Precio total
                GrupoArticulo = subPos.MATKL_SRV//MATKL_SRV   MATKL_SRV   Grupo de Articulo
            }).ToList();
        }


        private ContratoSolpWSMOAResponse MapSinPI(Z_MMRFC_OBTENER_CONTRATOResponse response)
        {
            ContratoSolpWSMOAResponse result = new ContratoSolpWSMOAResponse();
            result.ContratosSolp = new List<ContratoSolp> { };

            if (response.EX_HEADER.Length == 0 && response.EX_ITEM.Length == 0)
            {
                return result;
            }

            var numeroDocumentoCompras = "";

            foreach (var contratoCabecera in response.EX_HEADER)
            {
                if (numeroDocumentoCompras != contratoCabecera.NUMBER)
                {
                    numeroDocumentoCompras = contratoCabecera.NUMBER;

                    var contrato = new ContratoSolp()
                    {
                        //Resultado de Cabecera de Contratos Marco
                        NumeroDocumentoCompras = contratoCabecera.NUMBER, //NUMBER  EBELN Número del documento de compras
                        Sociedad = contratoCabecera.COMP_CODE, //COMP_CODE BUKRS   Sociedad
                        IndicadorDeBorrado = contratoCabecera.DELETE_IND_HDR, //DELETE_IND_HDR  ELOEK Indicador de borrado en el documento de compras
                        NumeroCuentaProveedor = contratoCabecera.VENDOR, //VENDOR  ELIFN Número de cuenta del proveedor
                        NombreProveedor = contratoCabecera.NAM_VENDOR,  //NAM_VENDOR LFA1-NAME1  Nombre del Proveedor
                        OrganizacionCompras = contratoCabecera.PURCH_ORG, //PURCH_ORG   EKORG Organización de compras
                        GrupoCompras = contratoCabecera.PUR_GROUP, //PUR_GROUP BKGRP   Grupo de compras
                        ClaveMoneda = contratoCabecera.CURRENCY, //CURRENCY    WAERS Clave de moneda
                        InicioPeriodoValidez = contratoCabecera.VPER_START, //VPER_START KDATB   In.período validez
                        FinPeriodoValidez = contratoCabecera.VPER_END//VPER_END KDATE   Fin período validez
                    };

                    contrato.Posiciones = response.EX_ITEM.Where(x => x.NUMBER == contrato.NumeroDocumentoCompras).Select(pos => new ContratoSolpPosicion
                    {
                        NumeroDocumentoCompras = pos.NUMBER, //NUMBER  EBELN   Número del documento de compras
                        NumeroPosicionDocumentoCompras = pos.ITEM_NO, //ITEM_NO EBELP   Número de posición del documento de compras
                        IndicadorDeBorrado = pos.DELETE_IND, //DELETE_IND  ELOEK   Indicador de borrado en el documento de compras
                        NumeroMaterial = pos.MATERIAL, //MATERIAL    MATNR18 Número de material (18 caracteres)
                        TextoMaterialOServicio = pos.SHORT_TEXT, //SHORT_TEXT  TXZ01   Texto de Material o Servicio
                        Centro = pos.PLANT, //PLANT   WERKS_D Centro
                        Almacen = pos.STGE_LOC, //STGE_LOC    LGORT_D Almacén
                        CantidadPrevista = pos.TARGET_QTY, //TARGET_QTY  KTMNG   Cantidad prevista
                        UnidadMedida = pos.PO_UNIT, //PO_UNIT BSTME   Unidad de medida de pedido
                        ImporteMonedaBapi = pos.NET_PRICE, //NET_PRICE   BAPICUREXT  Importe de moneda para BAPIs (con 9 decimales)
                        TipoPosicionDocumentoCompras = pos.ITEM_CAT, //ITEM_CAT    PSTYP   Tipo de posición del documento de compras
                        TipoImputacionCompras = pos.ACCTASSCAT, //ACCTASSCAT  KNTTP   Tipo de imputación
                        NumeroPaquete = pos.PCKG_NO, //PCKG_NO PACKNO  Nº paquete
                        GrupoArticuloMateriales = pos.MATKL, //MATKL   MATKL   Grupo de Articulo de Materiales
                        SubPosiciones = response.EX_SUB_ITEM.Length > 0 ? ObtenerSubPosicionesSinPI(pos, response.EX_SUB_ITEM) : new List<ContratoSolpSubposicion>()
                    }).ToList();

                    result.ContratosSolp.Add(contrato);
                }
            }
            //result.error = resultado;

            return result;
        }

        private List<ContratoSolpSubposicion> ObtenerSubPosicionesSinPI(WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5900 posicion, WS_GAQ_sin_PI_DIRECT_COMPRAS.ZMPES5910[] subPosiciones)
        {
            return subPosiciones.Where(x => x.NUMBER == posicion.NUMBER && x.ITEM_NO == posicion.ITEM_NO).Select(subPos => new ContratoSolpSubposicion
            {

                //Resultado de Subosiciones del Contratos Marco           
                NumeroDocumentoCompras = subPos.NUMBER, //NUMBER  EBELN   Número del documento de compras
                NumeroPosicionDocumentoCompras = subPos.ITEM_NO, //ITEM_NO EBELP   Número de posición del documento de compras
                Subposicion = subPos.EXTROW, //EXTROW  EXTROW  Subposición
                NumeroServicio = subPos.SRVPOS, //SRVPOS  ASNUM   Número de servicio
                TextoBreve = subPos.KTEXT1, //KTEXT1  SH_TEXT1    Texto breve
                CantidadPositivoONegativo = subPos.MENGE, //MENGE   MENGEV  Cantidad con signo +/-
                UnidadMedidaBase = subPos.MEINS, //MEINS   MEINS   Unidad de medida base
                PrecioUnitario = subPos.TBTWR, //TBTWR   SBRTWR  Precio Unitario
                PrecioTotal = subPos.NETWR, //NETWR   SNETWR  Precio total
                GrupoArticulo = subPos.MATKL_SRV//MATKL_SRV   MATKL_SRV   Grupo de Articulo
            }).ToList();
        }


    }
}


