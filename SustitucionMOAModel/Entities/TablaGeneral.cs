using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class TablaGeneral
    {
        [Key]
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        public int? Padre_Id { get; set; }

        [ForeignKey("Padre_Id")]
        public TablaGeneral Padre { get; set; }
        //public virtual ICollection<TablaGeneral> Hijos { get; set; }

    }
}
