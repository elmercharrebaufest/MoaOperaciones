using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class CircularArchivo
    {
        public int Circular_Id { get; set; }
        public int Archivo_Id { get; set; }   

        [ForeignKey("Circular_Id")]
        public virtual Circular Circular { get; set; }

        [ForeignKey("Archivo_Id")]
        public virtual Archivo Archivo { get; set; }

    }
}
