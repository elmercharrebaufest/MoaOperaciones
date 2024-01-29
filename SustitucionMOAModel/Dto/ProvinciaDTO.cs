using SustitucionMOAModel.Entities;


namespace SustitucionMOAModel.Dto
{
    public class ProvinciaDto
    {
        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }

        public int Orden { get; set; }

        public ProvinciaDto() { }

        public ProvinciaDto(Provincia entity)
        {
            if (entity != null)
            {
                ProvinciaId = entity.ProvinciaId;
                Nombre = entity.Nombre;
                Orden = entity.Orden;
            }
        }
    }

}
