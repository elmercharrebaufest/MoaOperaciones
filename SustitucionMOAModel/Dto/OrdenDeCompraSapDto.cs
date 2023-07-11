using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SustitucionMOAModel.Dto
{
    public class OrdenDeCompraSAPDto
    {
        public OrdenDeCompraSAPCabecera Cabecera { get; set; }
        public List<OrdenDeCompraSAPPosicion> Posiciones { get; set; }
    }

    public class OrdenDeCompraSAPCabecera
    {
        public string OrdenDeCompra { get; set; } //nro orden de compra
        public string CodigoProveedor { get; set; } //codigo de proveedor, con otra rfc buscar el vendedor

        //        <COMP_CODE>MOA</COMP_CODE>// fijo
        //        <DOC_TYPE>ZPE1</DOC_TYPE>//tipo de documento
        //        <DELETE_IND/>// si esta borrado
        //        <STATUS>9</STATUS>// status
        //        <CREAT_DATE>2023-07-07</CREAT_DATE>// fecha de alta
        //        <CREATED_BY>RABELLATM</CREATED_BY>//quien lo creo
        //        <VENDOR>0057984261</VENDOR>
        //        <PURCH_ORG>2029</PURCH_ORG>// organizacion de compra
        //        <PUR_GROUP>001</PUR_GROUP>//grupo de compra
        //        <CURRENCY>ARP</CURRENCY>//moneda
        //        <EXCH_RATE>1.00000</EXCH_RATE>//tipo de cambio
    }

    public class OrdenDeCompraSAPPosicion
    {
        //POITEM
        //POACCOUNT
        //POADDRDELIVERY
        public OrdenDeCompraSAPPosicionDireccionDeEntrega DireccionDeEntrega { get; set; }
    }

    public class OrdenDeCompraSAPPosicionDireccionDeEntrega
    {
        //POADDRDELIVERY
    }
}
