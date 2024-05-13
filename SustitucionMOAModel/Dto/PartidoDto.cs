namespace SustitucionMOAModel.Dto
{
    public class PartidoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int ProvinciaId { get; set; }
        public virtual string Provincia { get; set; }
    }
}
