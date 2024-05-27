using SustitucionMOAModel.Entities;
using System.Collections.Generic;

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
        public List<string> Permisos { get; set; }
        public bool NuevoUsuario { get; set; }
        public string ApiKey { get; set; }
        public string UsuarioSap { get; set; }
        public string RazonSocial { get; set; }
        public TipoUsuarioDto TipoUsuario { get; set; }
        public string OrganizacionDeCompra { get; set; }
        public string Suplente { get; set; }

        public UsuarioDto() { }

        public UsuarioDto(Usuario usuario)
        {
            Id = usuario.Id;
            Mail = usuario.Mail;
            Habilitado = usuario.Habilitado;
            CUIT = usuario.CUITRegistro;
            UsuarioSap = string.IsNullOrEmpty(usuario.UsuarioSap) ? "" : usuario.UsuarioSap;
            TipoUsuario = new TipoUsuarioDto(usuario.TipoUsuario);
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

            CodigoProveedor = ObtenerCodigoProveedor();
            Permisos = new List<string>();
            RazonSocial = usuario.ObtenerRazonSocial();
            OrganizacionDeCompra = usuario.OrganizacionDeCompra;
            Suplente = usuario.Suplente;
    }

        private string ObtenerCodigoProveedor()
        {
            try
            {
                //
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
            catch
            {
                return "CUIT INVALIDO";
            }

        }
    }
}
