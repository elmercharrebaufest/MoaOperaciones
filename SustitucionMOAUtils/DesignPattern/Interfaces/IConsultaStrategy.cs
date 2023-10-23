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
        void AgregarConsulta(Consulta consulta, Comentario comentario);
        void EnviarMailInterno(Consulta consulta, Comentario comentario);
    }
}
