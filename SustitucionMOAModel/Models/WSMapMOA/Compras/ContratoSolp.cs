using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class ContratoSolp
    {
        //Resultado de Cabecera de Contratos Marco
        public string NumeroDocumentoCompras { get; set; } //NUMBER EBELN   Número del documento de compras
        public string Sociedad { get; set; } //COMP_CODE   BUKRS Sociedad
        public string IndicadorDeBorrado { get; set; }  //DELETE_IND_HDR ELOEK   Indicador de borrado en el documento de compras
        public string NumeroCuentaProveedor { get; set; } //VENDOR ELIFN   Número de cuenta del proveedor
        public string NombreProveedor { get; set; } //NAM_VENDOR  LFA1-NAME1 Nombre del Proveedor
        public string OrganizacionCompras { get; set; } //PURCH_ORG EKORG   Organización de compras
        public string GrupoCompras { get; set; } //PUR_GROUP   BKGRP Grupo de compras
        public string ClaveMoneda { get; set; } //CURRENCY WAERS   Clave de moneda
        public string InicioPeriodoValidez { get; set; } //VPER_START  KDATB In.período validez
        public string FinPeriodoValidez { get; set; } //VPER_END    KDATE Fin período validez
        public List<ContratoSolpPosicion> Posiciones { get; set; } = new List<ContratoSolpPosicion>();
    }

    public class ContratoSolpPosicion
    {
        //Resultado de Posiciones del Contratos Marco
        public string NumeroDocumentoCompras { get; set; } //NUMBER EBELN   Número del documento de compras
        public string NumeroPosicionDocumentoCompras { get; set; } //ITEM_NO EBELP Número de posición del documento de compras
        public string IndicadorDeBorrado { get; set; } //DELETE_IND ELOEK   Indicador de borrado en el documento de compras
        public string NumeroMaterial { get; set; } //MATERIAL MATNR18 Número de material(18 caracteres)
        public string TextoMaterialOServicio { get; set; } //SHORT_TEXT TXZ01   Texto de Material o Servicio
        public string Centro { get; set; } //PLANT   WERKS_D Centro
        public string Almacen { get; set; } //STGE_LOC LGORT_D Almacén
        public decimal CantidadPrevista { get; set; } //TARGET_QTY  KTMNG Cantidad prevista
        public string UnidadMedida { get; set; } //PO_UNIT BSTME Unidad de medida de pedido
        public decimal ImporteMonedaBapi { get; set; } //NET_PRICE BAPICUREXT  Importe de moneda para BAPIs(con 9 decimales)
        public string TipoPosicionDocumentoCompras { get; set; } //ITEM_CAT PSTYP   Tipo de posición del documento de compras
        public string TipoImputacionCompras { get; set; } //ACCTASSCAT  KNTTP Tipo de imputación
        public string NumeroPaquete { get; set; } //PCKG_NO PACKNO  Nº paquete
        public string GrupoArticuloMateriales { get; set; } //MATKL MATKL   Grupo de Articulo de Materiales
        public List<ContratoSolpSubposicion> SubPosiciones { get; set; } = new List<ContratoSolpSubposicion>();
    }

    public class ContratoSolpSubposicion
    {
        //Resultado de Subosiciones del Contratos Marco
        public string NumeroDocumentoCompras { get; set; } //NUMBER EBELN   Número del documento de compras
        public string NumeroPosicionDocumentoCompras { get; set; } //ITEM_NO EBELP Número de posición del documento de compras
        public string Subposicion { get; set; } //EXTROW EXTROW  Subposición
        public string NumeroServicio { get; set; } //SRVPOS  ASNUM Número de servicio
        public string TextoBreve { get; set; } //KTEXT1 SH_TEXT1    Texto breve
        public decimal CantidadPositivoONegativo { get; set; } //MENGE MENGEV  Cantidad con signo +/-
        public string UnidadMedidaBase { get; set; } //MEINS MEINS   Unidad de medida base
        public decimal PrecioUnitario { get; set; } //TBTWR SBRTWR  Precio Unitario
        public decimal PrecioTotal { get; set; } //NETWR SNETWR  Precio total
        public string GrupoArticulo { get; set; } //MATKL_SRV MATKL_SRV   Grupo de Articulo
    }

}
