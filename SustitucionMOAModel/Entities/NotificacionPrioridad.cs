using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class NotificacionPrioridad
    {
        [Key]
        public int Prioridad_Id { get; set; }
        public string Prioridad_Descripcion { get; set; }
    }
}
