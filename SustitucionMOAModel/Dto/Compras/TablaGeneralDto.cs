using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class TablaSapDto
    {
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }
        public int? IdPadre { get; set; }
        public bool? FiltroComprador { get; set; }

        public TablaSapDto() { }

        public TablaSapDto(TablaSap entity)
        {
            if (entity != null)
            {
                Id = entity.Id;
                Tabla = entity.Tabla;
                Codigo = entity.Codigo;
                CodigoSap = entity.CodigoSap;
                Descripcion = entity.Descripcion;
                IdPadre = entity.Padre_id;
                FiltroComprador = entity.FiltroComprador;
            }
        }

        public string CodigoDescripcion
        {
            get
            {
                return string.Format("{0} {1} {2}", CodigoSap, !string.IsNullOrEmpty(Descripcion) && !string.IsNullOrEmpty(CodigoSap) ? "-" : string.Empty, Descripcion);
            }
        }
    }
}