using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class AprobacionDto
    {
        public int ID { get; set; }
        public string Ingresante_CDS { get; set; }
        public string NRO_ES_LOCAL { get; set; }
        public int? NRO_ES_SAP { get; set; }
        public DateTime? Fecha_Documento { get; set; }
        public DateTime? Fecha_Contabilizacion { get; set; }
        public string Referencia { get; set; }
        public int? Cantidad { get; set; }
        public string Descripcion_ES { get; set; }
        public double? Importe { get; set; }
        public string Estado_certificacion { get; set; }
        public bool? Aprobada_automaticamente { get; set; }
        public string Aprobador_CDS { get; set; }
        public string Fiscal_SOLPED { get; set; }
        public string Suplente { get; set; }
        public string Area_Fiscal { get; set; }
        public DateTime? Fecha_Carga_ES { get; set; }
        public DateTime? Fecha_aprobacion { get; set; }
        public DateTime? Fecha_rechazo { get; set; }
        public string Motivo_rechazo { get; set; }
        public bool? Notificaciones_enviadas { get; set; }
        public string NRO_OC { get; set; }
        public string NRO_POS { get; set; }
        public string Nro_linea { get; set; }
        public string Nro_servicio { get; set; }
        public string Texto_breve_servicio { get; set; }
        public string UM { get; set; }
        public double? Monto { get; set; }
        public double? Cantidad_Anterior { get; set; }
        //public string Porcentaje { get; set; } // Provisional - no en tabla
        public string Cantidad_a_certificar { get; set; }
        public string Porcentaje_a_certificar { get; set; }
        public double? Monto_a_certificar { get; set; }
        public double? Monto_total { get; set; }
    }
}