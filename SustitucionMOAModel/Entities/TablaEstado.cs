using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class TablaEstado
    {
        [Key]
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public string Color { get; set; }

    }
    
}
