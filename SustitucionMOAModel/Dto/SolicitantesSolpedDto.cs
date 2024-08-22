using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class SolicitantesSolpedDto
    {
        public string NumeroSolp { get; set; }
        public SolicitanteDto Solicitante { get; set; }

    }

    public class SolicitanteDto
    {
        public string Aprobador { get; set; }

        public string Suplente { get; set; }
    }

}
