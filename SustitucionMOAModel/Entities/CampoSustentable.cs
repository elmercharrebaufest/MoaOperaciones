using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class CampoSustentable
    {
        public int Id { get; set; }
        public ICollection<CampoCosecha> Cosechas { get; set; }
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
    }
}
