using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class TablaSap
    {
        [Key]
        public int Id { get; set; }
        public string Tabla { get; set; }
        public string Codigo { get; set; }
        public string CodigoSap { get; set; }
        public string Descripcion { get; set; }
        public int? Padre_id { get; set; }

        [ForeignKey("Padre_id")]
        public TablaSap Padre { get; set; }

        public override bool Equals(object obj)
        {
            return obj is TablaSap sap &&
                   Id == sap.Id &&
                   Tabla == sap.Tabla &&
                   Codigo == sap.Codigo &&
                   CodigoSap == sap.CodigoSap &&
                   Descripcion == sap.Descripcion &&
                   Padre_id == sap.Padre_id &&
                   EqualityComparer<TablaSap>.Default.Equals(Padre, sap.Padre);
        }

        public override int GetHashCode()
        {
            int hashCode = 2000866701;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tabla);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Codigo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodigoSap);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Descripcion);
            hashCode = hashCode * -1521134295 + Padre_id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<TablaSap>.Default.GetHashCode(Padre);
            return hashCode;
        }
        //public virtual ICollection<TablaSap> Hijos { get; set; }
    }
}
