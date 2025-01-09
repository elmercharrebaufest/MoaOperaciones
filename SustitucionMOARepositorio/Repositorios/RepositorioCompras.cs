using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioCompras : RepositorioEF, IRepositorioCompras
    {
        public RepositorioCompras(DbContext context) : base(context) { }

        public List<MaterialSolpDto> BuscarMaterialesCatalogadosPorCodigoSap(string codigoSapMatch, int centroId)
        {
            //return ListarProyeccion<MaterialSolp, MaterialSolpDto>(
            //    m => new MaterialSolpDto(m),
            //    m =>
            //        m.Centro_Id == centroId &&
            //        m.CodigoSap.Contains(codigoSapMatch) &&
            //        m.Estado);
            var materiales = Listar<MaterialSolp>(
                m =>
                    m.Centro_Id == centroId &&
                    m.CodigoSap.Contains(codigoSapMatch) &&
                    m.Estado);
            return materiales.ConvertAll(m => new MaterialSolpDto(m));
        }
    }
}
