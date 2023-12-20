using SustitucionMOAModel.Dto;
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
        List<string> Names { get; }
        Consulta AgregarConsulta(Consulta consulta, Comentario comentario);
        string Name { get; }
        Consulta AgregarConsulta(Consulta consulta, Comentario comentario, List<DestinatarioDto> destinatarios, HttpFileCollectionBase files);
        string GuardarAdjuntoComentario(int consultaId, HttpFileCollectionBase files);
        void EnviarMail(Consulta consulta, Comentario comentario, HttpFileCollectionBase files, List<DestinatarioDto> destinatarios);
    }
}
