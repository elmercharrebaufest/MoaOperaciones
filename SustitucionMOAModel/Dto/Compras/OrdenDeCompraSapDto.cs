using System;
using System.Collections.Generic;
using SustitucionMOAModel.Entities;


namespace SustitucionMOAModel.Dto
{
    public class OrdenDeCompraSAPDto
    {
        public OrdenDeCompraSAPCabecera Cabecera { get; set; }
        public List<OrdenDeCompraSAPPosicion> Posiciones { get; set; } = new List<OrdenDeCompraSAPPosicion>();
        public List<OrdenDeCompraSAPCertificacion> Certificaciones { get; set; } = new List<OrdenDeCompraSAPCertificacion>();
        public ErrorOC Error { get; set; }
        public string Mensaje { get; set; }
    }

    public class OrdenDeCompraSAPCabecera
    {
        public string OrdenDeCompra { get; set; } //nro orden de compra
        public string CodigoProveedor { get; set; } //codigo de proveedor, con otra rfc buscar el vendedor
        public string RazonSocialProveedor { get; set; }
        public string CUITProveedor { get; set; }
        public string Moneda { get; set; }
        public decimal MontoTotal { get; set; }
        public string CreadoPor { get; set; }
        public string ClaseDocumento { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string FechaCreacionString { get; set; }
        public string TipoDocCompras { get; set; }
        public int? Usuario_Id { get; set; }
        public string TipoSolp { get; set; }
        public decimal MontoBruto { get; set; }
        public string EstadoLiberacionCodigo { get; set; }
        public int? UsuarioCompras_Id { get; set; }
        public string UsuarioComprasSAP { get; set; }
        public string OrganizacionDeComprasCodigo { get; set; }
        public string MailProveedor { get; set; }
        public decimal SaldoDisponible { get; set; }

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
        //POACCOUNT
        //POADDRDELIVERY
        public OrdenDeCompraSAPPosicionDireccionDeEntrega DireccionDeEntrega { get; set; }
        public string Indice { get; set; } //PO_ITEM
        public string IndiceSolp { get; set; } //PREQ_ITEM
        public int NumeroItemOC { get { return int.Parse(Indice); } }
        public string RegistroInfo { get; set; }
        public string NroSolp { get; set; }
        public DateTime? PlazoDeOferta { get; set; }
        public string TipoPosicion { get; set; }
        public string AcuerdoMarco { get; set; }
    }

    public class OrdenDeCompraSAPCertificacion
    {
        public string NroCertificacion { get; set; }
        public decimal Saldo { get; set; }
        public string Moneda { get; set; }
        public string MontoFormateado { get; set; }
        public List<Archivo> Archivo { get; set; } = new List<Archivo>();
    }

    public class OrdenDeCompraSAPPosicionDireccionDeEntrega
    {
        //POADDRDELIVERY
        public string RegionSap { get; set; }
        public int? Id { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }
        public string PaisSap { get; set; }
    }

    public class ErrorOC
    {
        public string Mensaje { get; set; }
        public string Tipo { get; set; }
    }
}