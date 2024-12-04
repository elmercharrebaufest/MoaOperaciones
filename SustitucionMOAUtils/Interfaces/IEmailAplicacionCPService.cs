using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailAplicacionCPService
    {
        void EnviarMailAplicacionRechazada(IEnumerable<AplicacionCartaPorte> aplicaciones, string motivo, string destinatario);
    }
}
