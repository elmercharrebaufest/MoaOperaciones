using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class SecuenciaIngresosBrutosCoeficienteUnificadoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public SecuenciaIngresosBrutosCoeficienteUnificadoDto (){}

        public SecuenciaIngresosBrutosCoeficienteUnificadoDto(SecuenciaIngresosBrutosCoeficienteUnificado s) 
        {
            Id = s.Id;
            Descripcion = s.Descripcion;
        }
    }
}
