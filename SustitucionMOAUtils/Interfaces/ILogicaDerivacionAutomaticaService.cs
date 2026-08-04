using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models;
using SustitucionMOAModel.Models.DataAgro;
using SustitucionMOAModel.Models.ViewModel.AltaEmpresa;
using SustitucionMOARepositorio;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ILogicaDerivacionAutomaticaService
    {
        string CorrerProcesoReasignacion();

        bool isUserAllowed(string mail);
        bool isUserAllowedDerivacion(string mail);

    }
}
