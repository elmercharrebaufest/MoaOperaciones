using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class CartaDePresentacion
    {
        public int ProveedorId { get; set; }
        public List<ParamInformeComercialMaterial> Materiales { get; set; }
        public string CUITVendedor { get; set; }
        public string RazonSocialVendedor { get; set; }
        public int ActividadID { get; set; }
        public string Actividad { get; set; }
        public int CampaniaID { get; set; }
        public string AntiguedadActividad { get; set; }

        public string AntecedentesComerciales { get; set; }

        public string MailContacto { get; set; }
        public int? TelefonoContacto { get; set; }

        public CartaDePresentacion()
        {
            Materiales = new List<ParamInformeComercialMaterial>();
            NuevosCampos = new List<NuevoProduccion>();
            NuevosAcopios = new List<NuevoAcopio>();
        }

        public List<NuevoProduccion> NuevosCampos { get; set; }
        public List<NuevoAcopio> NuevosAcopios { get; set; }
    }
}
