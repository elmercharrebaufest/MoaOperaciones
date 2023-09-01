using System;

namespace SustitucionMOAModel.Dto
{
    public class RegistroInfoDto
    {
        public string Id { get; set; }
        public decimal Precio { get; set; }
        public decimal Cantidad { get; set; }
        public string Vendedor { get; set; }
        public string NombreProveedor { get; set; }
        public string Unidad { get; set; }
        public string Moneda { get; set; }
        public string Centro { get; set; }
        public string Fecha { get; set; }
        public string Codigo { get; set; }
        public int PosicionId { get; set; }
        public string DescripcionPosicion { get; set; }
        public decimal CantidadAdjudicacion { get; set; }
        public string Cuit { get; set; }
        public int? Indice { get; set; }
        public int ProveedorId { get; set; }
        public int Numero { get; set; }
        public bool Deshabilitado { get; set; }
        public DateTime? FechaFormateada { get; set; }
        public int MonedaId { get; set; }
        public int UnidadId { get; set; }
        public string FechaUltimaCompra { get; set; }
        public string FechaVigencia { get; set; }
        public string MaterialCodigo { get; set; }
        public string NumeroOrdenDeCompra { get; set; }
    }
}