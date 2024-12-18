using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IUnidadMedidaService
    {
        List<UnidadMedidaSap> GetUnidadesMedidaSap(Expression<Func<UnidadMedidaSap, bool>> filtro);

        List<TResult> GetUnidadesMedidaSap<TResult>(Expression<Func<UnidadMedidaSap, bool>> filtro, Expression<Func<UnidadMedidaSap, TResult>> projection);
    }
}
