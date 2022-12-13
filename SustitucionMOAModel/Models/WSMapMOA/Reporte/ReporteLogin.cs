using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Reporte
{
    public class ReporteLoginData
    {

        public string Mail { get; set; }
        public string CUITRegistro { get; set; }
        public string UltimoLogin { get; set; }
        public string Nombre { get; set; }
        public string CUITProveedor { get; set; }
        public string RazonSocial { get; set; }
        public ReporteLoginData(string Mail, string CUITRegistro, DateTime? UltimoLogin, string Nombre, string CUITProveedor = null, string RazonSocial=null)
        {
            this.Mail = Mail;
            this.CUITRegistro = CUITRegistro;
            this.UltimoLogin = UltimoLogin?.ToString("dd/MM/yyyy hh:mm:ss") ?? string.Empty;
            this.Nombre = Nombre;
            this.CUITProveedor = CUITProveedor ?? string.Empty;
            this.RazonSocial = RazonSocial ?? string.Empty;
        }

    }
    public class ReporteLogin : ReporteBase
    {

        public override string GetBody()
        {
            return "Adjuntamos el excel con los últimos datos de login";
        }

        public override string GetFecha()
        {
            return DateTime.Now.ToString("dd-MM-yyyy");
        }
    }
}
