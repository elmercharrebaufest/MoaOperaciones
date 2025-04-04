using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOAUtils.Interfaces
{
    public interface ITablaSapService
    {
        List<TablaSap> ActualizarTablaSap(List<TablaSapDto> listaSap, string tablaSap);

        List<TablaSapDto> AutocompleteTablaSap(string tabla, string valor);

        TablaSap GetById(int id);

        TablaSap Obtener(Expression<Func<TablaSap, bool>> filtro);

        List<TablaSap> Listar(Expression<Func<TablaSap, bool>> filtros);

        List<TablaSapDto> ListarTablaSap(List<string> tablas);
        List<TablaSap> ObtenerMonedas();
    }
}
