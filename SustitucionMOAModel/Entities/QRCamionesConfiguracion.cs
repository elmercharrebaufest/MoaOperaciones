using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    [Table("QRCamionesConfiguracion")]
    public class QRCamionesConfiguracion
    {
        [Key]
        public int Id { get; set; }

        public string NombreEtapa { get; set; }

        public int TiempoEstimado { get; set; }

        public string TipoWorkflow { get; set; }

        public string FinEtapa { get; set; }

        public bool FinEtapaEsControlRecorrido { get; set; }
    }
}