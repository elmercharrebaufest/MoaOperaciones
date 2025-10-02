using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CampoSustentableExportBaseDTO
    {
        public string Cosecha { get; set; }
        public string Campo { get; set; }
        public string Renspa { get; set; }
        public string ProveedorRazonSocial { get; set; }
        public string CuitProveedor { get; set; }
        public string Estado
        {
            get
            {
                return ToneladasAprobadas > 0 ? "Aprobado" : ToneladasAprobadas == 0 ? "Desaprobado" : "En gestión";
            }
        }
        public string Motivo { get; set; }
        public double HectareasTotales { get; set; }
        public double HectareasSoja { get; set; }
        public double ToneladasAprobadas { get; set; }
        public string Normativa { get; set; }
        public string RazonSocial { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
