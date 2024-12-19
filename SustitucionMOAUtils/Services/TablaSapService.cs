using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOAUtils.Services
{
    public class TablaSapService : ITablaSapService
    {
        private readonly IRepositorio repositorio;

        public TablaSapService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public TablaSap GetById(int id)
        {
            return repositorio.Obtener<TablaSap>(x => x.Id == id);
        }

        public TablaSap Obtener(Expression<Func<TablaSap, bool>> filtro)
        {
            return repositorio.Obtener(filtro);
        }

        public List<TablaSap> Listar(Expression<Func<TablaSap, bool>> filtros)
        {
            return repositorio.Listar(filtros);
        }
    }
}
