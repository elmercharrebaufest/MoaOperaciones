using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class CampoCosecha
    {
        public int Id { get; set; }
        public int CampoSustentable_Id { get; set; }
        public virtual CampoSustentable Campo {get; set; }
        public int Cosecha_Id { get; set; }
        public virtual Cosecha Cosecha { get; set; }
        public ICollection<CampoProveedor> Proveedores { get; set; }
        public double ToneladasAprobadas { get; set; }
    }
}
