using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class PeticionDeOfertaUsuarioAdicionalDto
    {
        public PeticionDeOfertaUsuarioAdicionalDto() { }

        public PeticionDeOfertaUsuarioAdicionalDto(PeticionDeOfertaUsuarioAdicional entidad)
        {
            UsuarioId = entidad.Usuario_Id;
            Id = entidad.Id;
            RazonSocial = entidad.Usuario.ObtenerRazonSocial();
            CUIT = entidad.Usuario.CUITRegistro;
            Mail = entidad.Usuario.Mail;
        }

        public int UsuarioId { get; set; }
        public string RazonSocial { get; set; }
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string Mail { get; set; }
    }
}
