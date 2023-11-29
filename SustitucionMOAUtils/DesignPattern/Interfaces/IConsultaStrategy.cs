using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.DesignPattern.Interfaces
{
    public interface IConsultaStrategy
    {
        string Name { get; }
        Consulta AgregarConsulta(Consulta consulta, Comentario comentario);
    }
}
