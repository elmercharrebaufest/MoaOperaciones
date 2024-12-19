// Ignore Spelling: Utils Sustitucion

using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<TablaSap> ActualizarTablaSap(List<TablaSapDto> listaSap, string tablaSap)
        {
            List<TablaSap> nuevosItems = new List<TablaSap>();
            if (listaSap.Count > 0)
            {
                var listaBaseCodigoSAP = repositorio.Listar<TablaSap>(c => c.Tabla == tablaSap).ConvertAll(a => a.CodigoSap);

                nuevosItems = listaSap.Where(x => !listaBaseCodigoSAP.Contains(x.CodigoSap)).Select(item => new TablaSap()
                {
                    Codigo = item.Codigo,
                    CodigoSap = item.CodigoSap,
                    Descripcion = item.Descripcion,
                    Tabla = item.Tabla,
                    Padre_id = null,
                }).ToList();

                foreach (var item in nuevosItems)
                {
                    // uso un Agregar en lugar de AgregarTodos para que me devuelva el id de la entidad generar ya que necesito usarlo mas adelante.
                    //el AgregarTodos no devuelve el id de las entidades agregadas.
                    repositorio.Agregar(item);
                }
                repositorio.GuardarCambios();
            }
            return nuevosItems;
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
