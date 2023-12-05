using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.AplicacionCartaPorte
{
    public class AplicacionCartaPorteApiDto
    {
        public int Id { get; set; }
        
        public string Contrato { get; set; }
        
        public string CartaPorte { get; set; }
        
        public EstadoAplicacionCartaPorte Estado { get; set; }
        
        public int Kilogramos { get; set; }


        public AplicacionCartaPorteApiDto(Entities.AplicacionCartaPorte entidad)
        {
            Id = entidad.Id;
            Contrato = entidad.Contrato;
            CartaPorte = entidad.CartaPorte;
            Estado = entidad.Estado;
            Kilogramos = entidad.Kilogramos;
        }
    }
}
