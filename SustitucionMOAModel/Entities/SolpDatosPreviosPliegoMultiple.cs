using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class SolpDatosPreviosPliegoMultiple
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Solp_Id { get; set; }

        public int? Pliego_Id { get; set; }

        public int? EstadoDocumento_Id { get; set; }

        public int? TipoSolp_Id { get; set; }
    }
}
