using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ITablaSapService
    {
        TablaSap GetById(int id);

        TablaSap Obtener(Expression<Func<TablaSap, bool>> filtro);

        List<TablaSap> Listar(Expression<Func<TablaSap, bool>> filtros);
    }
}
