using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class ArchivoCampoSustentable
    {
        [Key]
        public int Id { get; set; }

        public int CampoCosechaId { get; set; }
        [ForeignKey("CampoCosechaId")]
        public virtual CampoCosecha CampoCosecha { get; set; }

        public int ProveedorId { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }

        public bool ProcesadoUcropit { get; set; }

        public int IdArchivoRecepcion { get; set; }
        [ForeignKey("IdArchivoRecepcion")]
        public virtual Archivo Archivo { get; set; }

    }
}
