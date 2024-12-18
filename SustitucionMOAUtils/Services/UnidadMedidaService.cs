using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOAUtils.Services
{
    public class UnidadMedidaService : IUnidadMedidaService
    {
        private readonly IRepositorio repositorio;

        public UnidadMedidaService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<UnidadMedidaSap> GetUnidadesMedidaSap(Expression<Func<UnidadMedidaSap, bool>> filtro)
        {
            return repositorio.Listar(filtro);
        }

        public List<TResult> GetUnidadesMedidaSap<TResult>(Expression<Func<UnidadMedidaSap, bool>> filtro, Expression<Func<UnidadMedidaSap, TResult>> projection)
        {
            return repositorio.Listar(projection, filtro);
        }
    }
}
