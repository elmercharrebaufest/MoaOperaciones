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
    public class RepositorioComprasSolicitante : RepositorioEF, IRepositorioComprasSolicitante
    {
        public RepositorioComprasSolicitante(DbContext context) : base(context) { }

        public List<UsuarioDto> ObtenerFiscalesAprobadores()
        {
            var fiscales = (
                from f in Set<FiscalAprobador>()
                where f.Usuario.Habilitado
                orderby f.Usuario.Mail
                select new UsuarioDto
                {
                    Mail = f.Usuario.Mail,
                    UsuarioSap = f.Usuario.UsuarioSap
                })
                .ToList();

            return fiscales;
        }
    }
}
