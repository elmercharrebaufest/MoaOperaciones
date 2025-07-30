using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SustitucionMOARepositorio.Repositorios
{
    public class RepositorioEntradaServicio : RepositorioEF, IRepositorioEntradaServicio
    {
        public RepositorioEntradaServicio(DbContext context) : base(context) { }

        public List<Solp> ObtenerSolpsAutocertificablesDeOC(List<string> nroSolps)
        {
            var solpsQry =
                from solp in Set<Solp>()
                where
                    nroSolps.Contains(solp.NroSolp)
                    && solp.CertificacionAutomatica
                    && solp.TipoSolpSap != 2
                select solp;

            return solpsQry.ToList();
        }

        public Adjudicacion ObtenerUltimaAdjudicacionOC(string nroOC)
        {
            var adjudicacionQry =
                from adjudicacion in Set<Adjudicacion>()
                where adjudicacion.NumeroOrdenDeCompra == nroOC
                orderby adjudicacion.FechaCreacion descending
                select adjudicacion;

            return adjudicacionQry.FirstOrDefault();
        }

        public bool ExisteRemitoActivoParaProveedor(string remitoNro, string proveedorCodigo)
        {
            if (string.IsNullOrEmpty(remitoNro) || string.IsNullOrEmpty(proveedorCodigo)) return false;

            var existeQry =
                Set<Aprobaciones>()
                    .Where(ap =>
                        ap.Referencia == remitoNro &&
                        ap.Proveedor == proveedorCodigo &&
                        !ap.Fecha_rechazo.HasValue &&
                        string.IsNullOrEmpty(ap.Anulado_por))
                    .Any();

            return existeQry;
        }

        public string ObtenerMailSuplenteSegunFecha(string mailUsuario, DateTime fechaReasignacion)
        {
            var fechaFiltro = fechaReasignacion.Date;

            var qrySuplente =
                from usuario in Set<Usuario>()
                join reasignacion in Set<UsuarioReasignacion>() on usuario.Id equals reasignacion.Usuario_Id
                where
                    usuario.Mail == mailUsuario &&
                    reasignacion.FechaDesde <= fechaFiltro &&
                    reasignacion.FechaHasta >= fechaFiltro
                select usuario.Suplente;

            return qrySuplente.FirstOrDefault();
        }
    }
}
