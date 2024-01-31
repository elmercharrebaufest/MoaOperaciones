using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class TipoUsuarioDto
    {
        public TipoUsuarioDto() { }

        public TipoUsuarioDto(TipoUsuario tipoUsuario)
        {
            Id = tipoUsuario.Id;
            Nombre = tipoUsuario.Nombre;
            NombreCorto = tipoUsuario.NombreCorto;
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
    }
}
