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
    public class RepositorioOrdenDeCargaFason : RepositorioEF, IRepositorioOrdenDeCargaFason
    {
        public RepositorioOrdenDeCargaFason(DbContext context) : base(context) { }

        public List<string> ObtenerCuilsChofer(int clienteId, string patenteAcoplado)
        {
            var cuilsChoferes =
                (
                    from o in Set<OrdenDeCargaFason>()
                    where
                        o.Cliente_Id == clienteId &&
                        o.PatenteAcoplado == patenteAcoplado
                    select o.CUILChofer
                )
                .Distinct()
                .ToList();

            return cuilsChoferes;
        }

        public List<string> ObtenerCuitsTransporte(int clienteId, string patenteAcoplado)
        {
            var cuitsTransporte =
                (
                    from o in Set<OrdenDeCargaFason>()
                    where
                        o.Cliente_Id == clienteId &&
                        o.PatenteAcoplado == patenteAcoplado
                    select o.CUITTransporte
                )
                .Distinct()
                .ToList();

            return cuitsTransporte;
        }
    }
}
