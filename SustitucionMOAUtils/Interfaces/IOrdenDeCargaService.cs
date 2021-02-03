using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrdenDeCargaService
    {
        string Agregar(OrdenDeCarga ordenDeCarga);
        List<OrdenDeCargaDto> Listar(Usuario usuario);
    }

}
