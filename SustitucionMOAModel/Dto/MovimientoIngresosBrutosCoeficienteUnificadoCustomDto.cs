using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class MovimientoIngresosBrutosCoeficienteUnificadoCustomDto
    {
        public int IdIngreso { get; set; }
        public string Persona { get; set; }
        public int EstadoNuevo { get; set; }
        public int Tipo { get; set; }
        public int Origen { get; set; }
        public string Accion { get; set; }
    }
}
