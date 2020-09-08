using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class UsuarioDto
    {
        public int Id { get; set; }

        public string Mail { get; set; }

        public bool Habilitado { get; set; }

        public string CodigoProveedor { get; set; }

        public string CUIT { get; set; }

        public string Tipo { get; set; }


        public UsuarioDto(Usuario usuario)
        {
            Id = usuario.Id;
            Mail = usuario.Mail;
            Habilitado = usuario.Habilitado;
            CUIT = usuario.CUITRegistro;
            CodigoProveedor = usuario.ObtenerCodigoProveedor();
            var rol = usuario.ObtenerRolPrincipal();

            if (rol != null)
                Tipo = rol.Codigo;
            else
                Tipo = "";
        }
    }
}
