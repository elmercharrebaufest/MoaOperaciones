using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class PliegoArchivo
    {
        [Key]
        public int Id { get; set; }
        public int Archivo_Id { get; set; }
        public int Pliego_Id { get; set; }
        public int TipoPliegoArchivo_Id { get; set; }

        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }
        [ForeignKey("Pliego_Id")]
        public virtual Pliego Pliego { get; set; }
        [ForeignKey("TipoPliegoArchivo_Id")]
        public virtual TablaGeneral TipoPliegoArchivo { get; set; }
    }
}
