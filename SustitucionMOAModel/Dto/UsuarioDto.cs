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

            switch (usuario.TipoUsuario.NombreCorto)
            {
                case "G":
                case "NG":
                case "A":
                    Tipo = "Proveedor";
                    break;
                case "CORR":
                    Tipo = "Corredor";
                    break;
                case "CLI":
                    Tipo = "Cliente";
                    break;
                default:
                    Tipo = "";
                    break;
            }

            CodigoProveedor = FormatearCodigo();
        }

        private string FormatearCodigo()
        {

            if (string.IsNullOrEmpty(CUIT))
                return "";

            if (Tipo == "Corredor")
            {
                return string.Concat("C", CUIT.Substring(2, 8));
            }
            else
            {
                return string.Concat("00", CUIT.Substring(2, 8));
            }
        }
    }
}
