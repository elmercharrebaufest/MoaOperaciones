// Ignore Spelling: Solp Nro Posicion Codigo Imputacion Sustitucion
namespace SustitucionMOAModel.Dto
{
    public class SubPosicionCrearPoMultipleDto
    {
        public int Id { get; set; }

        public int? NroSubPosicion { get; set; }

        public string CodigoServicio { get; set; }

        public string Tarea { get; set; }

        public decimal? Cantidad { get; set; }

        public string UnidadMedida { get; set; }

        public decimal? PrecioBruto { get; set; }

        public decimal? ValorNeto { get; set; }

        public string CuentaMayor { get; set; }

        public string Imputacion { get; set; }
    }
}