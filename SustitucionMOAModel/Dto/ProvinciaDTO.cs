using SustitucionMOAModel.Entities;


namespace SustitucionMOAModel.Dto
{
    public class ProvinciaDTO
    {

        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }

        public int Orden { get; set; }


        public ProvinciaDTO() { }

        public ProvinciaDTO(Provincia entity)
        {
            if (entity != null)
            {
                this.ProvinciaId = entity.ProvinciaId;
                this.Nombre = entity.Nombre;
                this.Orden = entity.Orden;
            }
        }


    }

}
