using System.Collections.Generic;
using comprasDto = SustitucionMOAModel.Dto;

namespace SustitucionMOAModel.Dto.PliegoMultiple
{
    public class TraerPliegoPMDto
    {
        public comprasDto.SolpDto Pliego { get; set; }

        public IEnumerable<int> Solps { get; set; }
    }
}
