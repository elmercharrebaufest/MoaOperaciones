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
    public class RepositorioUbicacionGeografica : RepositorioEF, IRepositorioUbicacionGeografica
    {
        public RepositorioUbicacionGeografica(DbContext context) : base(context) { }


        public DistanciaDomicilio ObtenerDistanciaSegunDescripcionDomicilio(string domicilioDescripcion)
        {
            var distanciaQry =
                from dist in Set<DistanciaDomicilio>()
                where
                    dist.DomicilioDescripcion.Equals(domicilioDescripcion)
                select dist;

            return distanciaQry.FirstOrDefault();
        }

        public List<DistanciaDomicilioReemplazos> ObtenerReemplazosParaDomicilios()
        {
            return Listar<DistanciaDomicilioReemplazos>(null);
        }
    }
}
