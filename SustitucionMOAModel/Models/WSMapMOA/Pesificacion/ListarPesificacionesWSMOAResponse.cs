using SustitucionMOAModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pesificacion
{
    public class ListarPesificacionesWSMOAResponse
    {
        public List<PesificacionSapDto> Pesificaciones { get; set; }

        public ListarPesificacionesWSMOAResponse()
        {
            Pesificaciones = new List<PesificacionSapDto>() { };
        }
    }
}
