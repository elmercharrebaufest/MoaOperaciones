using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class CausaConsultaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public CausaConsultaDto(CausaConsulta causaconsulta)
        {
            Id = causaconsulta.Id;
            Nombre = causaconsulta.Nombre;
        }
    }
}
