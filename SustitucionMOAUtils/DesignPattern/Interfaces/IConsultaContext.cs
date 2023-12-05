using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.DesignPattern.Interfaces
{
    public interface IConsultaContext
    {
        IConsultaStrategy GetStrategy(string strategyName);
    }
}
