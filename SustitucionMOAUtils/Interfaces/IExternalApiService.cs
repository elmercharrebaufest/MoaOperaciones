using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Login;
using SustitucionMOAModel.Models.WSMapMOA.Usuario;
using SustitucionMOAModel.Models.WSMapMOA.Usuario.Perfil;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IExternalApiService
    {
        List<Rol> GetRolesApiKey(string apikey);
    }
}
