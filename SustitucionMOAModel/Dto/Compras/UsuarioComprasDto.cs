using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class UsuarioComprasDto
    {
        public int? Id { get; set; }
        public string Mail { get; set; }
        public string Nombres { get; set; }
        public bool Habilitado { get; set; }
        public bool PorDefecto { get; set; }
        public UsuarioComprasDto() { }

        public UsuarioComprasDto(UsuarioCompras usuario)
        {
            if (usuario != null)
            {
                Id = usuario.Id;
                Mail = usuario.Mail;
                Nombres = usuario.Nombres;
                Habilitado = usuario.Habilitado;
                PorDefecto = usuario.PorDefecto;
            }
        }
    }
}
