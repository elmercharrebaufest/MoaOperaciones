using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class LiberadorSap
    {
        [Key]
        public int Id { get; set; }

        public string Mail { get; set; }

        public string NombreCompleto { get; set; }

        public bool Obligatorio { get; set; }

        public bool Habilitado { get; set; }

        public string Cargo { get; set; }
        
        public int LiberadorSapTipo_Id { get; set; }

        [ForeignKey("LiberadorSapTipo_Id")]
        public virtual LiberadorSapTipo LiberadorSapTipo { get; set; }
    }
    public class LiberadorSapTipo
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }
    }

    public class LiberadorSapSolp
    {
        [Key]
        public int Id { get; set; }

        public int Solp_Id { get; set; }

        public int LiberadorSap_Id { get; set; }

        [ForeignKey("Solp_Id")]
        public Solp Solp { get; set; }

        [ForeignKey("LiberadorSap_Id")]
        public virtual LiberadorSap LiberadorSap { get; set; }
    }
    
}