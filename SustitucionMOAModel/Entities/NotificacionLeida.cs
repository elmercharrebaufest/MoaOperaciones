using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class NotificacionLeida
    {
        [Key]
        public int Id { get; set; }
        public int Notificacion_Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime FechaLeida { get; set; }

        //public bool Any(Func<object, bool> value)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
