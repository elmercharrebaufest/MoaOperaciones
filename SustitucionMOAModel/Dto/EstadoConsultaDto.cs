using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EstadoConsultaDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }
        public string Code { get; set; }
        public int Cantidad { get; set; }

        public EstadoConsultaDto(EstadoConsulta estadoConsulta)
        {
            Id = estadoConsulta.Id;
            Descripcion = estadoConsulta.Descripcion;
            Color = estadoConsulta.Color;
            Code = estadoConsulta.Code;
        }
    }
}
