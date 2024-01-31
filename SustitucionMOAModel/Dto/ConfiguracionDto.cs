using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class ConfiguracionDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }

        public ConfiguracionDto() { }

        public ConfiguracionDto(Configuracion configuracion)
        {
            Id = configuracion.Id;
            Code = configuracion.Code;
            Value = configuracion.Value;
        }
    }
}
