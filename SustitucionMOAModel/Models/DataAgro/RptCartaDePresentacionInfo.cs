using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class RptCartaDePresentacionInfo
    {

        /*
        public int ProveedorId { get; set; }
        
        public string CUITVendedor { get; set; }
        public string RazonSocialVendedor { get; set; }
        public int ActividadID { get; set; }
        public string Actividad { get; set; }
        public int CampaniaID { get; set; }
        public string AntiguedadActividad { get; set; }

        public string AntecedentesComerciales { get; set; }

        public string MailContacto { get; set; }
        public int? TelefonoContacto { get; set; }
        */

        public string corredorCuit { get; set; }
        public string corredorRazonSocial { get; set; }
        public string corredorBolsa { get; set; }
        public string corredorNroRegistro { get; set; }
        public string vendedorCuit { get; set; }
        public string vendedorRazonSocial { get; set; }
        public string vendedorDomicilioFiscal { get; set; }
        public string vendedorActividad { get; set; }
        public string vendedorAntiguedadEnActividad { get; set; }
        public string vendedorAntecedentesComerciales { get; set; }
        public string vendedorMailContacto { get; set; }
        public string vendedorTelefonoContacto { get; set; }
        public string vendedorDomicilioReal { get; set; }
        public string vendedorCosecha { get; set; }


        public RptCartaDePresentacionInfo()
        {
        
            NuevosCampos = new List<NuevoProduccion>();
            NuevosAcopios = new List<NuevoAcopio>();
        }

        public List<NuevoProduccion> NuevosCampos { get; set; }
        public List<NuevoAcopio> NuevosAcopios { get; set; }
    }
}
