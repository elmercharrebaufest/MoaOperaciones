using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class TipoImputacionSAP
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public int? TablaGeneral_Id { get; set; }
    }
}