// Ignore Spelling: Solp Nro
namespace SustitucionMOAModel.Dto
{
    public class PosicionCrearPoMultipleDto
    {
        public int Id { get; set; }

        public int? NroPosicion { get; set; }

        public string Descripcion { get; set; }

        public string TipoImputacion { get; set; }

        public string Centro { get; set; }

        public string Almacen { get; set; }

        public string Moneda { get; set; }

        public decimal? ValorTotal { get; set; }

        public string nroSolp { get; set; }
    }
}