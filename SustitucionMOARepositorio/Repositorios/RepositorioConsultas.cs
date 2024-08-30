using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioConsultas : RepositorioEF, IRepositorioConsultas
    {
        public RepositorioConsultas(DbContext context) : base(context) { }

        public List<CategoriaDto> ListaCategorias(IEnumerable<string> incluir = null, IEnumerable<string> excluir = null)
        {
            var filtraPorIncluir = incluir != null;
            var filtraPorExcluir = excluir != null;
            if (!filtraPorIncluir)
            {
                incluir = new List<string>();
            }
            if (!filtraPorExcluir)
            {
                excluir = new List<string>();
            }
            var conteos = (from consulta in Set<Consulta>()
                           group consulta by consulta.Categoria_Id into grupo
                           select new  // Create anonymous object for each group
                           {
                               Cantidad = grupo.Count(),
                               CategoriaId = grupo.Key
                           }).ToList();
            var categorias = from categoria in Set<Categoria>()
                             where (filtraPorIncluir && incluir.Contains(categoria.Code))
                             || (filtraPorExcluir && !excluir.Contains(categoria.Code))
                             orderby categoria.Nombre
                             select new CategoriaDto
                             {
                                 Id = categoria.Id,
                                 Nombre = categoria.Nombre,
                                 Code = categoria.Code,
                             };


            return categorias.ToList().Select(c =>
            {
                var conteo = conteos.Find(q => q.CategoriaId == c.Id);
                c.Cantidad = conteo != null ? conteo.Cantidad : 0;
                return c;
            }).ToList();
        }
        public List<EstadoConsultaDto> ListaEstadosConsultas(IEnumerable<int> categoriasPermitidasId = null)
        {
            var conteos = categoriasPermitidasId != null
                            ? (from consulta in Set<Consulta>()
                               where categoriasPermitidasId.Contains(consulta.Categoria_Id)
                               group consulta by consulta.EstadoConsulta_Id into grupo
                               select new  // Create anonymous object for each group
                               {
                                   Cantidad = grupo.Count(),
                                   EstadoConsultaId = grupo.Key
                               }).ToList()
                           : (from consulta in Set<Consulta>()
                              group consulta by consulta.EstadoConsulta_Id into grupo
                              select new  // Create anonymous object for each group
                              {
                                  Cantidad = grupo.Count(),
                                  EstadoConsultaId = grupo.Key
                              }).ToList();
            var estadosConsulta = from m in Set<EstadoConsulta>()
                                  select new EstadoConsultaDto
                                  {
                                      Id = m.Id,
                                      Code = m.Code,
                                      Descripcion = m.Descripcion,
                                      Color = m.Color
                                  };


            return estadosConsulta.ToList().Select(e =>
            {
                var conteo = conteos.Find(q => q.EstadoConsultaId == e.Id);
                e.Cantidad = conteo != null ? conteo.Cantidad : 0;
                return e;
            }).ToList();
        }
    }
}
