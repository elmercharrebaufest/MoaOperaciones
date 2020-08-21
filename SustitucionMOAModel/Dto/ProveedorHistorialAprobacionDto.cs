using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Dto
{
    public class ProveedorHistorialAprobacionDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string EstadoAprobacionDescripcion { get; set; }
        public string Observacion { get; set; }
        public DateTime Fecha { get; set; }

    }
}
