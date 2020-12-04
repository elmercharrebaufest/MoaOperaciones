using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.DataAgro
{
    public class ParamInformeComercial
    {
        public int ProveedorId { get; set; }
        public List<ParamInformeComercialMaterial> Materiales { get; set; }
        public int CampañaId { get; set; }
        public string Campaña { get; set; }
        public bool EmplRelDep { get; set; }
        public string EmplRelDepCant { get; set; }
        public int? Rodados { get; set; }
        public string RodadosOtros { get; set; }
        public int? Chacra { get; set; }
        public string ChacraOtros { get; set; }
        public string AntigActividad { get; set; }
        public string ActuacionProd { get; set; }
        public string ClienteAnt { get; set; }
        public string Comentarios { get; set; }
        public string Domicilio { get; set; }
        public int? InformeComercialId { get; set; }

        public ParamInformeComercial()
        {
            Materiales = new List<ParamInformeComercialMaterial>();
        }

        public int ComercialID { get; set; } 
        public List<NuevoProduccion> NuevosCampos { get; set; }
        public List<NuevoAcopio> NuevosAcopios { get; set; }

        public ContactoComercial ContactoComercial { get; set; }

        public string direccion { get; set; }
        public string codigoPostal { get; set; }
        public int? localidadId { get; set; }
    }
}
