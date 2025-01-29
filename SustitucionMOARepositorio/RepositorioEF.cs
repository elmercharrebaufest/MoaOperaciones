using Molinos.Scato.Repositorio;
using SustitucionMOAModel.Consultas;
using SustitucionMOARepositorio.Extensiones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace SustitucionMOARepositorio
{
    //public sealed class RepositorioEF : IRepositorio
    public class RepositorioEF : IRepositorio
    {
        private readonly DbContext context;
        private const int SqlFkError = 547;


        public RepositorioEF(DbContext context)
        { //Forzar el uso del Sql Provider para que la EntityFramework.SqlServer.dll se copie al proyecto web
            //http://robsneuron.blogspot.com/2013/11/entity-framework-upgrade-to-6.html
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;

            this.context = context;
        }

        public TEntidad Agregar<TEntidad>(TEntidad entidad) where TEntidad : class
        {
            return Set<TEntidad>().Add(entidad);
        }

        public int Contar<TEntidad>() where TEntidad : class
        {
            return Set<TEntidad>().Count();
        }

        public int Contar<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().Count(filtro);
        }
        public void Dispose()
        {
            context.Dispose();
        }

        public bool Existe<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().Any(filtro);
        }

        public int GuardarCambios()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (DataException e)
            {
                if (ObtenerCodigoError(e) == SqlFkError)
                {
                    throw new EntidadReferenciadaException(string.Empty, e);
                }
                throw;
            }
        }


        public IQueryable<TEntidad> Incluir<TEntidad>(params Expression<Func<TEntidad, object>>[] includes) where TEntidad : class
        {
            IQueryable<TEntidad> query = this.Set<TEntidad>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }


        public List<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> filtro = null, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc, IEnumerable<Expression<Func<TEntidad, object>>> includes = null) where TEntidad : class
        {
            return ListarQueryable(Set<TEntidad>(), filtro, orden, direccionOrden, maxResultados, includes).ToList();
        }
        public List<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> condicion = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (condicion != null)
            {
                resultado = resultado.Where(condicion);
            }
            return resultado.ToList();
        }

        public List<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }

            var resultadoFinal = resultado.GroupBy(proyeccion).Select(g => g.Key);
            resultadoFinal = ListarProyeccionQueryable(resultadoFinal, orden, direccionOrden, maxResultados);
            return resultadoFinal.ToList();
        }

        //Si, se escribe así: https://dle.rae.es/intersecar
        public List<TProyeccion> ListarIntersecar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, IEnumerable<Expression<Func<TEntidad, bool>>> filtros, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            foreach (Expression<Func<TEntidad, bool>> filtro in filtros)
            {
                resultado = resultado.Where(filtro);
            }

            var resultadoFinal = resultado.GroupBy(proyeccion).Select(g => g.Key);
            resultadoFinal = ListarProyeccionQueryable(resultadoFinal, orden, direccionOrden, maxResultados);
            return resultadoFinal.ToList();
        }

        public IEnumerable<List<TProyeccion>> ListarAgrupado<TEntidad, TKey, TProyeccion>(Expression<Func<TEntidad, TKey>> agrupamiento, Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }

            return resultado.GroupBy(agrupamiento, proyeccion).Select(g => g.ToList());
        }


        public ListaPaginada<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Paginacion paginacion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }

            var resultadoFinal = resultado.GroupBy(proyeccion).Select(g => g.Key);
            int itemsTotales = resultadoFinal.Count();

            resultadoFinal = ListarProyeccionQueryable(resultadoFinal, paginacion.OrdenarPor, paginacion.DireccionOrden, 0);

            resultadoFinal = resultadoFinal.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TProyeccion>(resultadoFinal.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }

        public ListaPaginada<TEntidad> ListarConOrdenYPaginado<TEntidad>(IQueryable<TEntidad> resultadoFinal, Paginacion paginacion) where TEntidad : class
        {
            int itemsTotales = resultadoFinal.Count();

            resultadoFinal = ListarProyeccionQueryable(resultadoFinal, paginacion.OrdenarPor, paginacion.DireccionOrden, 0);

            resultadoFinal = resultadoFinal.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TEntidad>(resultadoFinal.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }

        public ListaPaginada<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> condicion, Paginacion paginacion) where TEntidad : class
        {
            IQueryable<TEntidad> resultados = Set<TEntidad>();
            if (condicion != null)
            {
                resultados = resultados.Where(condicion);
            }

            int itemsTotales = resultados.Count();

            if (paginacion.OrdenarPor != null)
            {
                var selectorOrden = Expresiones.Propiedad<TEntidad>(paginacion.OrdenarPor);
                resultados = paginacion.DireccionOrden == DirOrden.Asc
                                 ? resultados.OrderBy(selectorOrden)
                                 : resultados.OrderByDescending(selectorOrden);
            }

            resultados = resultados.Skip((paginacion.Pagina - 1) * paginacion.ItemsPorPagina).Take(paginacion.ItemsPorPagina);

            return new ListaPaginada<TEntidad>(resultados.ToList(), paginacion.Pagina, paginacion.ItemsPorPagina, itemsTotales);
        }
        public ListaPaginada<TEntidad> ListarConsultaPaginada<TEntidad>(IConsultaPaginada<TEntidad> consulta) where TEntidad : class
        {
            return consulta.Ejecutar(context);
        }

        public List<TProyeccion> ListarProyeccion<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class
        {
            var queryEntidad = filtro != null ? Set<TEntidad>().Where(filtro) : Set<TEntidad>();

            return queryEntidad.Select(proyeccion).ToList();
        }

        public IQueryable<TEntidad> ListarTodos<TEntidad>() where TEntidad : class
        {
            return this.Set<TEntidad>();
        }

        public IQueryable<TEntidad> ListarTodos<TEntidad>(params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class
        {
            var res = this.context.Set<TEntidad>().Include(navProperties[0]);
            for (var i = 1; i < navProperties.Count(); i++)
            {
                res = res.Include(navProperties[i]);
            }

            return res;
        }

        public IQueryable<TEntidad> ListarConsultable<TEntidad>(Expression<Func<TEntidad, bool>> condition) where TEntidad : class
        {
            return this.context.Set<TEntidad>().Where(condition);
        }

        public IQueryable<TEntidad> ListarConsultable<TEntidad>(Expression<Func<TEntidad, bool>> condition, params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class
        {
            if (navProperties.Any())
            {
                var res = this.context.Set<TEntidad>().Include(navProperties[0]);
                for (var i = 1; i < navProperties.Count(); i++)
                {
                    res = res.Include(navProperties[i]);
                }

                return res.Where(condition);
            }

            return this.ListarConsultable(condition);
        }

        public IQueryable<TEntidad> ListarPaginado<TEntidad>(Expression<Func<TEntidad, bool>> condition, Expression<Func<TEntidad, object>> orderBy, int page, int pageSize) where TEntidad : class
        {
            return this.ListarConsultable(condition).OrderBy(orderBy).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public IQueryable<TEntidad> ListarPaginado<TEntidad>(Expression<Func<TEntidad, bool>> condition, Expression<Func<TEntidad, object>> orderBy, int page, int pageSize, params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class
        {
            return this.ListarConsultable(condition, navProperties).OrderBy(orderBy).Skip((page - 1) * pageSize).Take(pageSize);
        }

        public TEntidad Obtener<TEntidad>(object id) where TEntidad : class
        {
            return Set<TEntidad>().Find(id);
        }

        public TEntidad Obtener<TEntidad>(Expression<Func<TEntidad, bool>> condicion) where TEntidad : class
        {
            return Set<TEntidad>().FirstOrDefault(condicion);
        }

        public TProyeccion Obtener<TEntidad, TProyeccion>(Expression<Func<TEntidad, bool>> condicion, Expression<Func<TEntidad, TProyeccion>> proyeccion) where TEntidad : class
        {
            return Set<TEntidad>().Where(condicion).Select(proyeccion).FirstOrDefault();
        }
        public TEntidad ObtenerNoTracking<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            return Set<TEntidad>().AsNoTracking().FirstOrDefault(filtro);
        }

        public TEntidad Obtener<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, bool>> filtro) where TEntidad : class
        {
            IQueryable<TEntidad> resultado = Set<TEntidad>();

            foreach (var i in includes)
            {
                resultado = resultado.Include(i);
            }

            return resultado.SingleOrDefault(filtro);
        }

        public TEntidad Obtener<TEntidad>(Expression<Func<TEntidad, bool>> condition, params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class
        {
            if (navProperties.Any())
            {
                var res = this.context.Set<TEntidad>().Include(navProperties[0]);
                for (var i = 1; i < navProperties.Count(); i++)
                {
                    res = res.Include(navProperties[i]);
                }

                return res.Where(condition).FirstOrDefault();
            }

            return this.Obtener(condition);
        }

        public TEntidad ObtenerConsultaEscalar<TEntidad>(IConsultaEscalar<TEntidad> consulta)
        {
            return consulta.Ejecutar(context);
        }

        public TEntidad Remover<TEntidad>(object id) where TEntidad : class
        {
            return Remover(Obtener<TEntidad>(id));
        }

        public TEntidad Remover<TEntidad>(TEntidad entidad) where TEntidad : class
        {
            return Set<TEntidad>().Remove(entidad);
        }
        public void RemoverTodos<TEntidad>(IEnumerable<TEntidad> entidades) where TEntidad : class
        {
            foreach (var entidad in entidades)
            {
                Set<TEntidad>().Remove(entidad);
            }
        }
        public void TruncarTabla<TEntidad>() where TEntidad : class
        {
            var tabla = typeof(TEntidad).Name;
            context.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + tabla + "]");
        }
        public DbRawSqlQuery<TEntidad> ExecuteQuery<TEntidad>(string query, SqlParameter parameters = null)
        {
            return context.Database.SqlQuery<TEntidad>(query, parameters);
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

        private IQueryable<TEntidad> ListarQueryable<TEntidad>(IQueryable<TEntidad> resultado, Expression<Func<TEntidad, bool>> filtro, string orden, DirOrden direccionOrden, int maxResultados, IEnumerable<Expression<Func<TEntidad, object>>> includes = null) where TEntidad : class
        {
            if (filtro != null)
            {
                resultado = resultado.Where(filtro);
            }

            if (maxResultados != 0)
            {
                resultado = resultado.Take(maxResultados);
            }

            if (orden != null)
            {
                var selectorOrden = Expresiones.Propiedad<TEntidad>(orden);
                resultado = direccionOrden == DirOrden.Asc
                                 ? resultado.OrderBy(selectorOrden)
                                 : resultado.OrderByDescending(selectorOrden);
            }

            if (includes != null)
            {
                foreach (var i in includes)
                {
                    resultado = resultado.Include(i);
                }
            }

            return resultado;
        }

        private int ObtenerCodigoError(DataException e)
        {
            var code = 0;
            if (e.InnerException != null)
            {
                var sqlEx = e.InnerException.InnerException as SqlException;
                if (sqlEx != null)
                {
                    code = sqlEx.Number;
                }
            }
            return code;
        }

        protected IDbSet<TEntidad> Set<TEntidad>() where TEntidad : class
        {
            return context.Set<TEntidad>();
        }

        public List<TEntidad> ListarConsulta<TEntidad>(IConsulta<TEntidad> consulta) where TEntidad : class
        {
            return consulta.Ejecutar(context);
        }

        public virtual void AgregarTodos<TEntidad>(IEnumerable<TEntidad> items, List<KeyValuePair<string, string>> properties = null) where TEntidad : class
        {
            var enumerable = items as IList<TEntidad> ?? items.ToList();
            if (enumerable.Any())
            {
                var dataTable = enumerable.ToDataTable(true, properties);
                context.SqlBulkInsert(dataTable, dataTable.TableName);
            }
        }
    }
}
