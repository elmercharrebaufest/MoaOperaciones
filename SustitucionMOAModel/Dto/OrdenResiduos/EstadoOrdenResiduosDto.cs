using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenResiduos
{
    public class EstadoOrdenResiduosDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string NombreExterno { get; set; }

        public string Semaforo { get; set; }
    }
}
