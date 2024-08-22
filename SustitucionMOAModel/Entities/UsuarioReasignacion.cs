using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class UsuarioReasignacion
    {
        [Key]
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime FechaDesde { get; set; }

        public DateTime FechaHasta { get; set; }
    }
}
