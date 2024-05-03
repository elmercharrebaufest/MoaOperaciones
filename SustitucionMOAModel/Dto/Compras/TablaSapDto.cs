using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class TablaGeneralDto
    {
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? IdPadre { get; set; }
        public TablaGeneralDto Padre { get; set; }
        public string CodigoVisualizacion { get; set; }

        public TablaGeneralDto()
        {

        }

        public TablaGeneralDto(TablaGeneral entity)
        {
            if(entity != null){
                this.Id = entity.Id;
                this.Tabla = entity.Tabla;
                this.Codigo = entity.Codigo;
                this.Descripcion = entity.Descripcion;
                this.IdPadre = entity.Padre_Id;
                if (entity.Padre != null)
                    this.Padre = new TablaGeneralDto(entity.Padre);
            }
            
        }
    }
}
