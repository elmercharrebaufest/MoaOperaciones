using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IConsultaCommon
    {
        string AgregarAdjuntoComentario(int consultaId, int comentarioId, HttpFileCollectionBase files);
        void EnviarMailInterno(Consulta consulta, Comentario comentario, HttpFileCollectionBase files);
    }
}
