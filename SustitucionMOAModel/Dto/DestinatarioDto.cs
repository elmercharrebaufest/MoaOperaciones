using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class DestinatarioDto
    {
        public string Campo { get; set; }
        public string Mail { get; set; }
        public int UsuarioId { get; set; }
        public string NombreTipoUsuario { get; set; }
        public DestinatarioDto(Usuario u) { 
            Campo = "Usuario Web";
            Mail = u.Mail;
            UsuarioId = u.Id;
            NombreTipoUsuario = u.TipoUsuario.NombreCorto;
        }
        public DestinatarioDto() { }
    }
}
