using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Compras;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SustitucionMOAUtils.Services
{
    public class UnidadMedidaService : IUnidadMedidaService
    {
        private readonly IRepositorio repositorio;
        private readonly IObtenerUnidadesDeMedidaAlternativasConsumerMOA obtenerUnidadesDeMedidaConsumerMOA;

        public UnidadMedidaService(IRepositorio repositorio,
                                   IObtenerUnidadesDeMedidaAlternativasConsumerMOA obtenerUnidadesDeMedidaAlternativasConsumerMOA)
        {
            this.repositorio = repositorio;
            this.obtenerUnidadesDeMedidaConsumerMOA = obtenerUnidadesDeMedidaAlternativasConsumerMOA;
        }

        public List<UnidadMedidaSap> GetUnidadesMedidaSap(Expression<Func<UnidadMedidaSap, bool>> filtro)
        {
            return repositorio.Listar(filtro);
        }

        public List<TResult> GetUnidadesMedidaSap<TResult>(Expression<Func<UnidadMedidaSap, bool>> filtro, Expression<Func<UnidadMedidaSap, TResult>> projection)
        {
            return repositorio.Listar(projection, filtro);
        }

        public List<UnidadesDeMedida> ObtenerUnidadesDesdeServicioSap(List<string> codigosMaterialSap)
        {
            return obtenerUnidadesDeMedidaConsumerMOA.Request(codigosMaterialSap);
        }

        public List<UnidadesDeMedida> ObtenerUnidadesDesdeServicioSap(string codigoMaterial)
        {
            return obtenerUnidadesDeMedidaConsumerMOA.Request(codigoMaterial);
        }
    }
}
