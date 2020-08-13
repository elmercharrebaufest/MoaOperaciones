using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAzureB2CService
    {
        Usuario LoguearUsuario(string mail, string CUIT, string granosFlag);
    }
}
