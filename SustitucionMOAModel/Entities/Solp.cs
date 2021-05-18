using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class Solp
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioCreacion_Id { get; set; }
        public int UsuarioModificacion_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int Pliego_Id { get; set; }
        public int ClaseDocumento_Id { get; set; }
        public string NroSolp { get; set; }
        public int EstadoSolpSap_Id { get; set; }
        public int EstadoDocumento_Id { get; set; }

        [ForeignKey("UsuarioCreacion_Id")]
        public virtual Usuario UsuarioCreacion { get; set; }
        [ForeignKey("UsuarioModificacion_Id")]
        public virtual Usuario UsuarioModificacion { get; set; }
        [ForeignKey("Pliego_Id")]
        public virtual Pliego Pliego { get; set; }
        [ForeignKey("ClaseDocumento_Id")]
        public virtual TablaSap ClaseDocumento { get; set; }
        [ForeignKey("EstadoSolpSap_Id")]
        public virtual TablaSap EstadoSolpSap { get; set; }
        [ForeignKey("EstadoDocumento_Id")]
        public virtual TablaEstado EstadoDocumento { get; set; }

        public virtual ICollection<SolpPosicion> Posiciones { get; set; }

    }
}
