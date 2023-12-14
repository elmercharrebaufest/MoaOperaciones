using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.DesignPattern.Interfaces
{
    public interface IConsultaStrategy
    {
        string Name { get; }
        Consulta AgregarConsulta(Consulta consulta, Comentario comentario);
        string GuardarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files);
        void EnviarMail(Consulta consulta, Comentario comentario, HttpFileCollectionBase files);
    }
}
