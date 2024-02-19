using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class AdjudicacionEditarDto
    {
        public string NumeroOrdenDeCompra { get; set; }
        public List<AdjudicacionPosicionEditarDto> Posiciones { get; set; } = new List<AdjudicacionPosicionEditarDto>();
        public string TextoDeCabecera { get; set; }
        public string CondicionesDeEntrega { get; set; }
        public string CondicionesDePago { get; set; }
        public string Garantias { get; set; }
        public string CondicionDePagoCodigo { get; set; }
        public decimal PagoEn1 { get; set; }
        public decimal PagoEn2 { get; set; }
        public decimal PagoEn3 { get; set; }
        public decimal PagoEn1Porcentaje { get; set; }
        public decimal PagoEn2Porcentaje { get; set; }
        public string CondicionDeImportacionCodigo { get; set; }
        public string CondicionDeImportacionComplemento { get; set; }
        public string MonedaCodigo { get; set; }
    }
}