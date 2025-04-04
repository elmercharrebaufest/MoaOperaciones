
namespace SustitucionMOAModel.Dto
{
    public class PeticionDeOfertaSolpPosicionDto
    {
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int SolpPosicion_Id { get; set; }
        public SolpPosicionDto Posicion { get; set; }
        public SolpPosicionDto Posiciones { get; set; }
        public int SolpId { get; set; }
        public SolpPosicionDto PosicionPeticion { get; set; }
        public bool EstaEliminado { get; set; }
    }
}
