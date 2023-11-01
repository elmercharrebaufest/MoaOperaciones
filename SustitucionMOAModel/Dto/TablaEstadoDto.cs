using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class TablaEstadoDto
    {
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public string Color { get; set; }

        public TablaEstadoDto() { }

        public TablaEstadoDto(TablaEstado entity)
        {
            this.Id = entity.Id;
            this.Tabla = entity.Tabla;
            this.Codigo = entity.Codigo;
            this.Descripcion = entity.Descripcion;
            this.Orden = entity.Orden;
            this.Color = entity.Color;
        }
    }
}
