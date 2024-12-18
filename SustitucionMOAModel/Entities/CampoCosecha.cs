using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CampoCosecha
    {
        [Key]
        public int Id { get; set; }

        public int CampoSustentable_Id { get; set; }

        [ForeignKey("CampoSustentable_Id")]
        public virtual CampoSustentable Campo { get; set; }

        public int Cosecha_Id { get; set; }

        [ForeignKey("Cosecha_Id")]
        public virtual Cosecha Cosecha { get; set; }

        [InverseProperty("CampoCosecha")]
        public ICollection<CampoProveedor> CamposProveedor { get; set; }

        public double ToneladasAprobadas { get; set; }

        public string MotivoRechazo { get; set; }

        [InverseProperty(nameof(Proveedor.CamposCosecha))]
        public virtual ISet<Proveedor> Proveedores { get; set; } = new HashSet<Proveedor>();
    }
}
