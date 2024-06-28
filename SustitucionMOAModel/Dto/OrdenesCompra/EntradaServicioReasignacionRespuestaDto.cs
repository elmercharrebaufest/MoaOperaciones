using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioReasignacionRespuestaDto
    {
        public string newApprover {  get; set; }

        public string newSubstitute { get; set; }

        public string status { get; set; }
    }
}
