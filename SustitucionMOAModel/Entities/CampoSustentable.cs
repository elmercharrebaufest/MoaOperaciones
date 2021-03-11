using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class CampoSustentable
    {
        [Key]
        public int Id { get; set; }
        public ICollection<CampoCosecha> Cosechas { get; set; }
        public string Nombre { get; set; }

        public virtual Localidad Localidad { get; set; }

        public int Localidad_Id { get; set; }
    }
}
