using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EstadoIngresosBrutosCoeficienteUnificadoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public EstadoIngresosBrutosCoeficienteUnificadoDto() { }

        public EstadoIngresosBrutosCoeficienteUnificadoDto(EstadoIngresosBrutosCoeficienteUnificado x)
        {
            Id = x.Id;
            Descripcion = x.Descripcion;
        }
    }
}
