using System;
using System.Collections.Generic;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class AdjudicacionDto
    {    
        public int Id { get; set; }
        public int Cotizacion_Id { get; set; }
        public int Solp_Id { get; set; }
        public string NumeroOrdenDeCompra { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int UsuarioCreador_Id { get; set; }
        public string UsuarioCreador { get; set; }
        public int Moneda_Id { get; set; }
        public string MonedaDescripcion { get; set; }
        public decimal MontoTotal { get; set; }
        public List<AdjudicacionPosicionDto> AdjudicacionPosiciones { get; set; } = new List<AdjudicacionPosicionDto>();
        public decimal PrecioFinal { get; set; }
        public string Proveedor { get; set; }
        public string TipoPosicionCodigo { get; set; }
        public string TextoDeCabecera { get; set; }
        public string CondicionesDeEntrega { get; set; }
        public string CondicionesDePago { get; set; }
        public string Garantias { get; set; }
        public string Centro { get; set; }
        public string CalleEntrega { get; set; }
        public string CodigoPostal { get; set; }
        public decimal PrecioBruto { get; set; }
        public string EstadoLiberacionCodigo { get; set; }
        public string EstadoLiberacionDetalle { get; set; }
        public bool EsMonedaProveedor { get; set; }
        public int RegionSap { get; set; }
        public bool CreadoAutomatico { get; set; }
        public string MonedaCodigo { get; set; }
        public CondicionDePagoDto CondicionDePago { get; set; }
        public CondicionDeImportacionDto CondicionDeImportacion { get; set; }       
        public string CondicionDePagoCodigo { get; set; }
        public string CondicionDePagoId { get; set; }
        public string CondicionDeImportacionCodigo { get; set; }
        public string CondicionDeImportacionId { get; set; }
        public string CondicionDeImportacionDescripcion { get; set; }

        public decimal PagoEn1 { get; set; }
        public decimal PagoEn2 { get; set; }
        public decimal PagoEn3 { get; set; }
        public decimal PagoEn1Porcentaje { get; set; }
        public decimal PagoEn2Porcentaje { get; set; }
        public string NroSolp { get; set; }
    }

}