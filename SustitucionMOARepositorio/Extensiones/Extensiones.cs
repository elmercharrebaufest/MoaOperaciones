using SustitucionMOAModel.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Extensiones
{
    public static class Extensiones
    {
        public static decimal ToDecimal(this string str)
        {
            // you can throw an exception or return a default value here
            if (string.IsNullOrEmpty(str))
                return 0;

            decimal d;

            // you could throw an exception or return a default value on failure
            if (!decimal.TryParse(str, out d))
                return 0;

            return d;
        }
        public static ListaPaginada<TEntidad> OrdenarPaginarLista<TEntidad>(this IQueryable<TEntidad> resultado, Paginacion paginacion) 
        {
            int itemsTotales = resultado.Count();

            resultado = ListarProyeccionQueryable(resultado, paginacion.OrdenarPor, paginacion.DireccionOrden, 0);

            resultado = resultado.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TEntidad>(resultado.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
        private static IQueryable<TProyeccion> ListarProyeccionQueryable<TProyeccion>(IQueryable<TProyeccion> resultadoFinal, string orden, DirOrden direccionOrden, int maxResultados)
        {

            if (orden != null)
            {
                var selectorOrden = Expresiones.Propiedad<TProyeccion>(orden);
                resultadoFinal = direccionOrden == DirOrden.Asc
                                 ? resultadoFinal.OrderBy(selectorOrden)
                                 : resultadoFinal.OrderByDescending(selectorOrden);
            }
            //CAMBIE ESTO ARA ACA ABAJO
            if (maxResultados != 0)
            {
                resultadoFinal = resultadoFinal.Take(maxResultados);
            }

            return resultadoFinal;
        }
    }
}
