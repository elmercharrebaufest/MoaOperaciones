using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
