using SustitucionMOAModel.Entities;
using SustitucionMOAUtils.DesignPattern.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.DesignPattern.Classes
{
    public class ConsultaContext : IConsultaContext
    {
        private readonly IEnumerable<IConsultaStrategy> _strategies;

        public ConsultaContext(IEnumerable<IConsultaStrategy> strategies)
        {
            this._strategies= strategies;
        }

        public IConsultaStrategy GetStrategy(string strategyName)
        {
            var instance = _strategies.FirstOrDefault(x =>
                x.Names.Contains(strategyName));

                return instance;
        }

    }
}
