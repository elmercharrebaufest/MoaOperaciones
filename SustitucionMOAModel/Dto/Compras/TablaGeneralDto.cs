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

        public TablaSapDto() { }

        public TablaSapDto(TablaSap entity)
        {
            if(entity != null)
            {
                this.Id = entity.Id;
                this.Tabla = entity.Tabla;
                this.Codigo = entity.Codigo;
                this.CodigoSap = entity.CodigoSap;
                this.Descripcion = entity.Descripcion;
                this.IdPadre = entity.Padre_id;
            }
        }

        public string CodigoDescripcion
        {
            get
            {
                return string.Format("{0} {1} {2}", this.CodigoSap, !string.IsNullOrEmpty(this.Descripcion) && !string.IsNullOrEmpty(this.CodigoSap) ? "-" : string.Empty, this.Descripcion);
            }
        }
    }
}