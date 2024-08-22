using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioRejectRespuestaDto
    {
      public List<Aprobaciones> result {  get; set; }

      public string status { get; set; }
        public string Fecha_rechazo_string { get; set; }
    }
}
