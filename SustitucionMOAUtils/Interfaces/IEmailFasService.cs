using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEmailFasService
    {
        void EnviarMailContratoVencido(OrdenDeCarga ordenDeCarga);
    }
}
