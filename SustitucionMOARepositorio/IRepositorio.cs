using Molinos.Scato.Repositorio;
using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SustitucionMOARepositorio
{

    /// <summary>
    /// Representa el comportamiento de un repositorio
    /// </summary>
    public interface IRepositorio : IDisposable
    {
        /// <summary>
        /// Devuelve una entidad dada su clave primaria
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="id">Clave primaria de la entidad</param>
        /// <returns>La entidad que posee esa clave o null si no se encuentra ninguna</returns>
        TEntidad Obtener<TEntidad>(object id) where TEntidad : class;

        /// <summary>
        /// Devuelve una única entidad que cumple con la condición
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <param name="condicion">Expresión que dada una entidad determina si debe devolverse o no</param>
        /// <returns>La única entidad que se corresponde con la condición o null si no la encuentra</returns>
        /// <exception cref="InvalidOperationException">Se lanza cuando más de una entidad comple con la condición</exception>
        TEntidad Obtener<TEntidad>(Expression<Func<TEntidad, bool>> condicion) where TEntidad : class;

        TProyeccion Obtener<TEntidad, TProyeccion>(Expression<Func<TEntidad, bool>> condicion, Expression<Func<TEntidad, TProyeccion>> proyeccion) where TEntidad : class;
        TEntidad ObtenerNoTracking<TEntidad>(Expression<Func<TEntidad, bool>> filtro) where TEntidad : class;

        TEntidad Obtener<TEntidad>(IEnumerable<Expression<Func<TEntidad, object>>> includes, Expression<Func<TEntidad, bool>> condicion) where TEntidad : class;
        /// <summary>
        /// Lista todas las entidades que cumplen con la condición
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a devolver</typeparam>
        /// <returns>Todas las entidades que cumplen con la condición</returns>
        /// 
        List<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, bool>> filtro = null, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc, IEnumerable<Expression<Func<TEntidad, object>>> includes = null) where TEntidad : class;
        IEnumerable<List<TProyeccion>> ListarAgrupado<TEntidad, TKey, TProyeccion>(Expression<Func<TEntidad, TKey>> agrupamiento = null, Expression<Func<TEntidad, TProyeccion>> proyeccion = null, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class;
        List<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null, int maxResultados = 0, string orden = null, DirOrden direccionOrden = DirOrden.Asc) where TEntidad : class;
        ListaPaginada<TProyeccion> Listar<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Paginacion paginacions, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class;

        ListaPaginada<TEntidad> Listar<TEntidad>(Expression<Func<TEntidad, Boolean>> condicion, Paginacion paginacion) where TEntidad : class;
        ListaPaginada<TEntidad> ListarConsultaPaginada<TEntidad>(IConsultaPaginada<TEntidad> consulta) where TEntidad : class;

        List<TProyeccion> ListarProyeccion<TEntidad, TProyeccion>(Expression<Func<TEntidad, TProyeccion>> proyeccion, Expression<Func<TEntidad, bool>> filtro = null) where TEntidad : class;

        /// <summary>
        /// Devuelve la cantidad de entidades en el repositorio
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidades a contar</typeparam>
        /// <returns>La cantidad de entidades de un tipo determinado en el repositorio</returns>
        int Contar<TEntidad>() where TEntidad : class;

        /// <summary>
        /// Devuelve la cantidad de entidades en el repositorio que cumplen con la condición
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidades a contar</typeparam>
        /// <param name="condicion">Expresión que dada una entidad devuelve si debe contarse o no</param>
        /// <returns>La cantidad de entidades de un tipo determinado en el repositorio</returns>
        int Contar<TEntidad>(Expression<Func<TEntidad, Boolean>> condicion) where TEntidad : class;

        /// <summary>
        /// Devuelve si existe una entidad que cumpla con la condición
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidades a comprobar</typeparam>
        /// <param name="condicion">Expresion que dada una entidad devuelve si debe contarse o no</param>
        /// <returns>true si existe alguna entidad que compla con la condición</returns>
        bool Existe<TEntidad>(Expression<Func<TEntidad, bool>> condicion) where TEntidad : class;

        /// <summary>
        /// Agrega una entidad al repositorio
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a agregar</typeparam>
        /// <param name="entidad">Entidad a agregar</param>
        /// <returns>Entidad agregada</returns>
        TEntidad Agregar<TEntidad>(TEntidad entidad) where TEntidad : class;
        void AgregarTodos<TEntidad>(IEnumerable<TEntidad> entidades, List<KeyValuePair<string, string>> properties = null) where TEntidad : class;
        /// <summary>
        /// Remueve una entidad del repositorio
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a remover</typeparam>
        /// <param name="entidad">Entidad a remover</param>
        /// <returns>Entidad removida</returns>
        TEntidad Remover<TEntidad>(TEntidad entidad) where TEntidad : class;

        /// <summary>
        /// Remueve una entidad del repositorio dada su clave primaria
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de la entidad a remover</typeparam>
        /// <param name="id">Clave de la entidad a remover</param>
        /// <returns>Entidad removida</returns>
        TEntidad Remover<TEntidad>(object id) where TEntidad : class;
        void RemoverTodos<TEntidad>(IEnumerable<TEntidad> entidades) where TEntidad : class;
        void TruncarTabla<TEntidad>() where TEntidad : class;
        /// <summary>
        /// Persiste los cambios hechos al repositorio
        /// </summary>
        /// <returns>Cantidad de entidades agregadas o actualizadas</returns>
        int GuardarCambios();
        
        /// <summary>
        /// Obtener las consultas realizadas por EF
        /// </summary>
        /// <returns>Lista de entidades</returns>
        TEntidad ObtenerConsultaEscalar<TEntidad>(IConsultaEscalar<TEntidad> consulta);
        ListaPaginada<TEntidad> ListarConOrdenYPaginado<TEntidad>(IQueryable<TEntidad> lista, Paginacion paginacion) where TEntidad : class;

        /// <summary>
        /// Retorna una consulta IQueryable que incluye propiedades de navegación especificadas para la entidad TEntidad.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <param name="includes">Expresiones de propiedades de navegación a incluir en la consulta.</param>
        /// <returns>Consulta IQueryable con propiedades de navegación incluidas.</returns>
        IQueryable<TEntidad> Incluir<TEntidad>(params Expression<Func<TEntidad, object>>[] includes) where TEntidad : class;

        /// <summary>
        /// Obtiene una entidad TEntidad que cumple con una condición especificada y opcionalmente incluye propiedades de navegación.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <param name="condition">Condición para seleccionar la entidad.</param>
        /// <param name="navProperties">Expresiones de propiedades de navegación a incluir.</param>
        /// <returns>Entidad TEntidad que cumple con la condición.</returns>
        TEntidad Obtener<TEntidad>(Expression<Func<TEntidad, bool>> condition, params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class;
        
        /// <summary>
        /// Lista todas las entidades de tipo TEntidad.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <returns>Consulta IQueryable de todas las entidades TEntidad.</returns>
        IQueryable<TEntidad> ListarTodos<TEntidad>() where TEntidad : class;

        /// <summary>
        /// Lista todas las entidades de tipo TEntidad incluyendo propiedades de navegación especificadas.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <param name="navProperties">Expresiones de propiedades de navegación a incluir en la consulta.</param>
        /// <returns>Consulta IQueryable de todas las entidades TEntidad con propiedades de navegación incluidas.</returns>
        IQueryable<TEntidad> ListarTodos<TEntidad>(params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class;

        /// <summary>
        /// Lista todas las entidades de tipo TEntidad que cumplen con una condición especificada.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <param name="condition">Condición para seleccionar las entidades.</param>
        /// <returns>Consulta IQueryable de entidades TEntidad que cumplen con la condición.</returns>
        IQueryable<TEntidad> ListarConsultable<TEntidad>(Expression<Func<TEntidad, bool>> condition) where TEntidad : class;
        
        /// <summary>
        /// Lista todas las entidades de tipo TEntidad que cumplen con una condición especificada e incluye propiedades de navegación especificadas.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <param name="condition">Condición para seleccionar las entidades.</param>
        /// <param name="navProperties">Expresiones de propiedades de navegación a incluir en la consulta.</param>
        /// <returns>Consulta IQueryable de entidades TEntidad que cumplen con la condición con propiedades de navegación incluidas.</returns>
        IQueryable<TEntidad> ListarConsultable<TEntidad>(Expression<Func<TEntidad, bool>> condition, params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class;

        /// <summary>
        /// Lista entidades de tipo TEntidad paginadas y ordenadas según una condición y expresión de orden.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <param name="condition">Condición para seleccionar las entidades.</param>
        /// <param name="orderBy">Expresión de orden para la consulta.</param>
        /// <param name="page">Número de página.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <returns>Consulta IQueryable de entidades TEntidad paginadas y ordenadas.</returns>
        IQueryable<TEntidad> ListarPaginado<TEntidad>(Expression<Func<TEntidad, bool>> condition, Expression<Func<TEntidad, object>> orderBy, int page, int pageSize) where TEntidad : class;

        /// <summary>
        /// Lista entidades de tipo TEntidad paginadas y ordenadas según una condición y expresión de orden, incluyendo propiedades de navegación especificadas.
        /// </summary>
        /// <typeparam name="TEntidad">Tipo de entidad.</typeparam>
        /// <param name="condition">Condición para seleccionar las entidades.</param>
        /// <param name="orderBy">Expresión de orden para la consulta.</param>
        /// <param name="page">Número de página.</param>
        /// <param name="pageSize">Tamaño de página.</param>
        /// <param name="navProperties">Expresiones de propiedades de navegación a incluir en la consulta.</param>
        /// <returns>Consulta IQueryable de entidades TEntidad paginadas y ordenadas con propiedades de navegación incluidas.</returns>
        IQueryable<TEntidad> ListarPaginado<TEntidad>(Expression<Func<TEntidad, bool>> condition, Expression<Func<TEntidad, object>> orderBy, int page, int pageSize, params Expression<Func<TEntidad, object>>[] navProperties) where TEntidad : class;       
    }
}
