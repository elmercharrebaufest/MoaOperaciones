using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioOrdenDeCarga : RepositorioEF, IRepositorioOrdenDeCarga
    {
        public RepositorioOrdenDeCarga(DbContext context) : base(context) { }

        public List<string> ObtenerCuitsClientesDeOrdenesPendientesParaChofer(string cuilChofer)
        {
            var estadosIncluidos = new List<EstadoOrdenDeCarga>
            {
                EstadoOrdenDeCarga.Confirmado,
                EstadoOrdenDeCarga.Vencida,
                EstadoOrdenDeCarga.EntregaPendiente,
                EstadoOrdenDeCarga.EdicionSolicitada,
                EstadoOrdenDeCarga.AnulacionSolicitada,
                EstadoOrdenDeCarga.EntregaAnuladaPedidoPendienteAnulacion,
                EstadoOrdenDeCarga.EntregaGenerada
            };

            var cuitsClientes =
                (
                    from o in Set<OrdenDeCarga>()
                    where
                        o.CUITChofer == cuilChofer &&
                        estadosIncluidos.Contains(o.Estado)
                    select o.CUITCliente
                )
                .Distinct()
                .ToList();

            return cuitsClientes;
        }
    }
}
