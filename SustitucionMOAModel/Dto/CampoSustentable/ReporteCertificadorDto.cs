using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.CampoSustentable
{
    public class ReporteCertificadorDto
    {
        public int Id { get; set; }

        public string RazonSocial { get; set; }
        
        public string CUIT { get; set; }
        
        public string Nombre { get; set; }
        
        public string Provincia { get; set; }
        
        public string Departamento { get; set; }
        
        public string Localidad { get; set; }
        
        public string Latitud { get; set; }
        
        public string Longitud { get; set; }
        
        public double HectareasSoja { get; set; }
        
        public string NombreCosecha { get; set; }
    }
}
