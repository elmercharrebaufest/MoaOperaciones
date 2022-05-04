using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos
{
    public class ComprobanteNGLista
    {
        public string FechaDocumento { get; set; } //BLDAT: corresponde a la fecha de documento del documento // Fecha comprobante
        public string DescripcionTipoDocumento { get; set; } //LTEXT: corresponde a la descripción del tipo de documento
        public string NumeroLegalDocumento { get; set; } //XBLNR: corresponde al número legal del documento
        public string TotalMasMoneda { get; set; }
        public string OrdenDeCompra { get; set; } //EBELN: corresponde a la orden de compra
        public string CodigoEstadoDocumentoDescripcion { get; set; }
       
    }

    public class ComprobanteView : ComprobanteNGLista
    {
        public string FechaDocumento { get; set; } //BLDAT: corresponde a la fecha de documento del documento // Fecha comprobante
        public DateTime FechaComprobanteDate { get; set; }
        public string DescripcionTipoDocumento { get; set; } //LTEXT: corresponde a la descripción del tipo de documento
        public string NumeroLegalDocumento { get; set; } //XBLNR: corresponde al número legal del documento
        public string TotalMasMoneda { get; set; }
        public string OrdenDeCompra { get; set; } //EBELN: corresponde a la orden de compra
        public string CodigoEstadoDocumentoDescripcion { get; set; }
        public decimal ImporteMercaderiaDocumento { get; set; } //NET_AMOUNT: corresponde al importe de la mercadería del documento
        public decimal ImporteImpuestosDocumento { get; set; } //VAT_AMOUNT: corresponde al importe de los impuestos del documento
        public decimal TotalDocumento { get; set; } //GROSS_AMOUNT: corresponde al total del documento
        public string MonedaDocumento { get; set; } //WAERS: corresponde a la moneda del documento

        //public string TotalMasMoneda { get { return "$" + " " + this.TotalDocumento; } }      
        public string ColorEstado { get; set; }
        public string Sociedad { get; set; } //BUKRS: corresponde a la sociedad MOA que no se utilizará para la web
        public string CodigoProveedorSAP { get; set; } //LIFNR: corresponde al código de proveedor en SAP
        public string RazonSocialProveedorSAP { get; set; } //VEND_NAME: corresponde a la razón social del proveedor en SAP
        public string TipoDocumento { get; set; } //BLART: corresponde al tipo de documento
        public EstadoComprobantesNG CodigoEstadoDocumento { get; set; } //STATUS: corresponde al código de estado del documento
        public string CodigoRolDocumento { get; set; } //CURR_ROLE: corresponde al código del rol que tiene asignado este documento
        public string CodigoMotivoRechazo { get; set; } //DELREASON: corresponde al código del motivo de rechazo
        public DateTime fechaComprobanteDate { get; set; }

    }
}
