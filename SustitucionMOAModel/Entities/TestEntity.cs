using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class TestEntity
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }


    }
}
