using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class UsuarioComprasRelacionConUsuariosDto
    {
        public int? Id { get; set; }
        public UsuarioDto Usuario { get; set; }
        public UsuarioComprasDto UsuarioCompras { get; set; }

        public UsuarioComprasRelacionConUsuariosDto() { }
        public UsuarioComprasRelacionConUsuariosDto(UsuarioComprasRelacionConUsuarios entity)
        {
            this.Id = entity.Id;
            this.Usuario = new UsuarioDto(entity.Usuario);
            this.UsuarioCompras = new UsuarioComprasDto(entity.UsuarioCompras);
        }
    }
}
