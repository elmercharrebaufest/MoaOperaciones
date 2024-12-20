// Ignore Spelling: Sustitucion Utils paginacion nro repo automatica imputacion

using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class ComprasSolicitanteService : IComprasSolicitanteService
    {
        private readonly IRepositorio repositorio;

        public ComprasSolicitanteService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public ListaPaginada<SolpDto> ListarSolp(UsuarioDto usuarioActual, Paginacion paginacion, string nroSolp, string nombrePedido, DateTime? desde, DateTime? hasta, bool sap, bool mantenimiento, bool web, bool repoAutomatica, bool contratoMarco, List<int> usuarios = null, List<int> estados = null, List<int> centros = null, List<int> grupoDeCompras = null, List<int> claseDocumento = null, List<string> tipoImputacion = null, List<int> valorTipoImputacion = null)
        {
            var fechaHasta = hasta != null ? hasta.Value.AddDays(1) : (DateTime?)null;
            Usuario usuario = repositorio.Obtener<Usuario>(u => u.Id == usuarioActual.Id);
            var rol = usuario.Roles.Any(r => r.Codigo == "COMPRADOR") ? "COMPRADOR" : "SOLP";
            nroSolp = nroSolp.Trim();
            //var peticionCierre = repositorio.Listar<PeticionDeOfertaCierre, PeticionDeOfertaCierreDto>(pc => new PeticionDeOfertaCierreDto());

            if (!string.IsNullOrEmpty(nroSolp) && !nroSolp.StartsWith("0"))
            {
                nroSolp = "0" + nroSolp;
            }

            string[] pedidos = nombrePedido.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var todasLasSolp = repositorio.Listar<Solp, SolpDto>(x => new SolpDto
            {
                UsuarioActual = new UsuarioDto { Mail = x.UsuarioCreacion != null ? x.UsuarioCreacion.Mail : "" },
                Id = x.Id,
                NroSolp = x.NroSolp,
                VerCircular = x.TrabajoYaHecho == null || x.TrabajoYaHecho == false,
                NombreDeObra = x.Pliego == null ? "" : x.Pliego.NombreObra,
                FechaCreacion = x.FechaCreacion,
                EstadoDocumento = new TablaEstadoDto { Descripcion = x.EstadoDocumento == null ? "" : x.EstadoDocumento.Descripcion, Color = x.EstadoDocumento == null ? "" : x.EstadoDocumento.Color, Codigo = x.EstadoDocumento == null ? "" : x.EstadoDocumento.Codigo },
                EstadoSolpSap_Id = x.NroSolp != null && x.Posiciones.All(p => !p.Estado) ? -1 : (x.EstadoSolpSap != null ? x.EstadoSolpSap_Id : 0),
                EstadoSolpSap = new TablaSapDto { Descripcion = x.EstadoSolpSap != null ? x.EstadoSolpSap.Descripcion : "", Id = x.EstadoSolpSap != null ? x.EstadoSolpSap.Id : 0 },
                EstadoSolpDescripcion = x.NroSolp != null && x.Posiciones.All(p => !p.Estado) ? "Borrado en SAP" : (x.EstadoSolpSap != null ? x.EstadoSolpSap.Descripcion : ""),
                TipoSolp = new TablaGeneralDto { Descripcion = x.TipoSolp != null ? x.TipoSolp.Descripcion : "", Codigo = x.TipoSolp != null ? x.TipoSolp.Codigo : "" },
                VincularPliego = !x.Pliego_Id.HasValue,
                TieneCondicionesGenerales = x.Pliego == null ? null : x.Pliego.TieneCondicionesGenerales,
                RevisadoPor = x.Pliego == null ? "" : x.Pliego.RevisadoPor,
                TipoSolpSap = x.TipoSolpSap,
                EstadoPasos = x.EstadoPasos,
                PosicionesEstado = x.Posiciones.All(p => !p.Estado),
                ItemPorPagina = paginacion.ItemsPorPagina,
                Pagina = paginacion.Pagina,
                TipoPosicionCodigo = x.Posiciones.Select(posiciones => posiciones.TipoPosicion.Codigo).FirstOrDefault(),
                SolpConAdjuntos = x.Pliego.Archivos.Any(r => r.FileKey == FileKeys.AdjuntoCotizacionesSolp || r.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp),
                ChatSinLeer = x.ChatInternoCompras.Any(a => !a.Leido && a.Usuario.Roles.Any(r => r.Codigo != rol))
                             || x.Posiciones.Any(po => po.Peticiones
                            .SelectMany(se => se.PeticionDeOferta.Usuarios
                            .SelectMany(re => re.ChatExterno))
                            .Any(al => !al.Leido && al.Usuario.Roles.Any(r => r.Codigo != rol)))
                            ,
                TieneMensajesChatInterno = x.ChatInternoCompras.Count > 0,
                TienePeticionDeOferta = x.Posiciones.Any(posi => posi.Peticiones.Any()),
                ClaseDocumento_Id = x.ClaseDocumento_Id,
                TieneMensajesChatExterno = x.Posiciones.Select(po => po.Peticiones
                                        .SelectMany(se => se.PeticionDeOferta.Usuarios
                                        .SelectMany(re => re.ChatExterno)))
                                        .Any(chatExterno => chatExterno.Any()),


            },
            paginacion,
            x => x.FechaBorrado == null && (string.IsNullOrEmpty(nroSolp) || x.NroSolp.ToUpper().StartsWith(nroSolp.ToUpper())) &&
            (!estados.Any() || (x.EstadoSolpSap_Id != null && estados.Contains((int)x.EstadoSolpSap_Id)) || (estados.Any(y => y == -1) && x.NroSolp != null && x.Posiciones.All(p => !p.Estado))) &&
            (!usuarios.Any() || (x.UsuarioCreacion_Id != null && usuarios.Contains((int)x.UsuarioCreacion_Id))) &&
            (sap && x.TipoSolpSap == 3 || mantenimiento && x.TipoSolpSap == 2 || repoAutomatica && x.TipoSolpSap == 4 ||
            (web && (x.TipoSolpSap == null || x.TipoSolpSap == 1)) || (!sap && !mantenimiento && !web && !repoAutomatica)) &&
            (desde == null || x.FechaCreacion >= desde.Value) && (fechaHasta == null || x.FechaCreacion <= fechaHasta.Value)
            && (!pedidos.Any() || pedidos.All(p => x.Pliego.NombreObra.ToLower().Contains(p.ToLower()))) &&
            (!contratoMarco || x.Posiciones.Any(p => !string.IsNullOrEmpty(p.NumeroContratoSuperior))) &&
            (!centros.Any() || x.Posiciones.Any(c => centros.Contains(c.Centro_Id))) && (!grupoDeCompras.Any() || x.Posiciones.Any(gc => grupoDeCompras.Contains((int)gc.GrupoCompras_Id))) &&
            (!claseDocumento.Any() || claseDocumento.Contains((int)x.ClaseDocumento_Id)) && (!tipoImputacion.Any() || x.Posiciones.Any(c => tipoImputacion.Contains(c.TipoImputacion.Codigo))) &&
            (!valorTipoImputacion.Any() || x.Posiciones.Any(p => valorTipoImputacion.Contains((int)p.ValorTipoImputacion_Id)) || x.Posiciones.Any(p => p.Subposiciones.Any(sp => valorTipoImputacion.Contains((int)sp.TipoImputacion_Id)))));

            if (todasLasSolp.Items != null && todasLasSolp.Items.Any())
            {
                todasLasSolp.Items.FirstOrDefault().ItemsTotales = todasLasSolp.ItemsTotales;
            }

            return todasLasSolp;
        }

        public DatosUltimaSolpDto ObtenerUltimaSolp(int usuarioId)
        {
            var ultimaSolp = repositorio.Listar<Solp>(a => a.UsuarioCreacion_Id == usuarioId && !string.IsNullOrEmpty(a.NroSolp)).OrderByDescending(a => a.Id).FirstOrDefault();
            //var ultimaSolp = repositorio.Listar<Solp>(a => a.UsuarioCreacion_Id == usuarioId && !string.IsNullOrEmpty(a.NroSolp)).OrderByDescending(a => long.Parse(a.NroSolp)).FirstOrDefault();

            if (ultimaSolp == null) return null;

            DatosUltimaSolpDto result = new DatosUltimaSolpDto();

            result.FiscalContrato = ultimaSolp.Pliego?.FiscalContrato;
            result.EmailFiscalContrato = ultimaSolp.Pliego?.Email;
            result.Telefono = ultimaSolp.Pliego?.Telefono;

            result.ClaseDocumento = ultimaSolp.ClaseDocumento != null ? new TablaSapDto
            {
                Id = ultimaSolp.ClaseDocumento.Id,
                Tabla = ultimaSolp.ClaseDocumento.Tabla,
                Codigo = ultimaSolp.ClaseDocumento.Codigo,
                CodigoSap = ultimaSolp.ClaseDocumento.CodigoSap,
                Descripcion = ultimaSolp.ClaseDocumento.Descripcion,
                IdPadre = ultimaSolp.ClaseDocumento.Padre_id
            } : null;

            result.GrupoCompras = ultimaSolp.Posiciones.FirstOrDefault()?.GrupoCompras != null ? new TablaSapDto
            {
                Id = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Id,
                Tabla = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Tabla,
                Codigo = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Codigo,
                CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.CodigoSap,
                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Descripcion,
                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().GrupoCompras.Padre_id
            } : null;

            result.CuentaMayor = ultimaSolp.Posiciones.FirstOrDefault()?.CuentaMayorSap != null ? new TablaSapDto
            {
                Id = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Id,
                Tabla = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Tabla,
                Codigo = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Codigo,
                CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.CodigoSap,
                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Descripcion,
                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().CuentaMayorSap.Padre_id
            } : null;

            result.Almacen = ultimaSolp.Posiciones.FirstOrDefault()?.Almacen != null ? new TablaSapDto
            {
                Id = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Id,
                Tabla = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Tabla,
                Codigo = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Codigo,
                CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().Almacen.CodigoSap,
                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Descripcion,
                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Almacen.Padre_id
            } : null;

            result.TipoPosicion = ultimaSolp.Posiciones.FirstOrDefault()?.TipoPosicion != null ? new TablaGeneralDto
            {
                Id = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Id,
                Tabla = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Tabla,
                Codigo = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Codigo,
                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Descripcion,
                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().TipoPosicion.Padre_Id
            } : null;

            result.Centro = ultimaSolp.Posiciones.FirstOrDefault()?.Centro != null ? new TablaSapDto
            {
                Id = ultimaSolp.Posiciones.FirstOrDefault().Centro.Id,
                Tabla = ultimaSolp.Posiciones.FirstOrDefault().Centro.Tabla,
                Codigo = ultimaSolp.Posiciones.FirstOrDefault().Centro.Codigo,
                CodigoSap = ultimaSolp.Posiciones.FirstOrDefault().Centro.CodigoSap,
                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Centro.Descripcion,
                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Centro.Padre_id
            } : null;

            result.CuentaMayorSP = ultimaSolp.Posiciones.FirstOrDefault()?.Subposiciones.FirstOrDefault()?.CuentaMayorSap != null ? new TablaSapDto
            {
                Id = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Id,
                Tabla = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Tabla,
                Codigo = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Codigo,
                Descripcion = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.CodigoSap + " - " + ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Descripcion,
                IdPadre = ultimaSolp.Posiciones.FirstOrDefault().Subposiciones.FirstOrDefault().CuentaMayorSap.Padre_id
            } : null;

            return result;
        }

        public List<UsuarioDto> ListarUsuarioSolicitante()
        {
            var usuarios = repositorio.Listar<Usuario, UsuarioDto>(usuario => new UsuarioDto
            {
                Mail = usuario.Mail,
                UsuarioSap = usuario.UsuarioSap
            }, usuario => usuario.Roles.Any(r => r.PermisosAsociados.Select(x => x.Permiso).Contains("ABM SOLP")));
            return usuarios;
        }

        public InfoVisitasDeObraDto ListarVisitasDeObra(List<VisitaObraDto> visitas)
        {
            List<DateTime> fechas = visitas.ConvertAll(x => x.FechaHora.Date);

            IEnumerable<DetalleVisitaDto> detalleVisitas = repositorio.Listar<PliegoVisita, DetalleVisitaDto>(pliegoVisita => new DetalleVisitaDto
            {
                FechaHora = pliegoVisita.FechaHora,
                PliegoId = pliegoVisita.Pliego_Id,
            })
              .AsEnumerable()
              .Where(vis => vis.FechaHora.HasValue && fechas.Any(f => vis.FechaHora.Value.Date == f));

            IEnumerable<int> listaIdPliego = detalleVisitas.Select(x => x.PliegoId);

            List<SolpDto> solpDB = repositorio.Listar<Solp, SolpDto>(x => new SolpDto
            {
                NroSolp = x.NroSolp,
                Pliego_Id = x.Pliego_Id,
                Id = x.Id
            }, so => so.Pliego_Id.HasValue && listaIdPliego.Contains(so.Pliego_Id.Value)).ToList();

            List<int?> solpIds = solpDB.ConvertAll(x => x.Id);

            var po = repositorio.Listar<PeticionDeOferta>(peticion => solpIds.Contains(peticion.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id)).ToList();

            foreach (DetalleVisitaDto detalle in detalleVisitas)
            {
                detalle.NroSolp = solpDB.FirstOrDefault(solp => solp.Pliego_Id == detalle.PliegoId)?.NroSolp ?? "";

                detalle.Proveedores = po.Where(pou => pou.Posiciones.FirstOrDefault().SolpPosicion.Solp_Id == solpDB.FirstOrDefault(solp => solp.Pliego_Id == detalle.PliegoId).Id)?
                    .SelectMany(pro => pro.Usuarios).Select(peticionUsuario => new ProveedorDto
                    {
                        RazonSocial = peticionUsuario.Usuario.ObtenerRazonSocial(),
                        Mail = peticionUsuario.Usuario.Mail
                    }).Distinct().ToList();
            }

            var detalleVisitasConSolp = detalleVisitas.Where(detalle => !string.IsNullOrEmpty(detalle.NroSolp)).ToList();

            var info = new InfoVisitasDeObraDto()
            {
                CantidadVisitas = detalleVisitasConSolp.Count,
                DetalleVisitas = detalleVisitasConSolp
            };

            return info;
        }
    }
}
