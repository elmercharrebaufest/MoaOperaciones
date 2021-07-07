using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class ComprasService : IComprasService
    {
        private readonly IRepositorio repositorio;

        private readonly string rutaArchivosCompras = ConfigurationManager.AppSettings["RutaArchivosCompras"];

        public ComprasService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public SolpDto GuardarSolp(SolpDto solp, HttpFileCollectionBase adjuntos)
        {
            Solp solpEntity = null;
            Pliego pliegoEntity = null;

            if (solp.Id.HasValue)
            {
                var includes = new List<Expression<Func<Solp, object>>>();
                includes.Add(x => x.Pliego);
                includes.Add(x => x.Pliego.VisitasMasivas);
                includes.Add(x => x.Pliego.Archivos);
                includes.Add(x => x.Posiciones);
                includes.Add(x => x.Posiciones.Select(y=> y.Subposiciones));
                includes.Add(x => x.UsuarioCreacion);
                includes.Add(x => x.UsuarioModificacion);

                solpEntity = repositorio.Obtener<Solp>(solp.Id.Value);

                if(solpEntity != null)
                {
                    //TODO: validar si está en un estado modificable

                    solpEntity.UsuarioModificacion_Id = solp.UsuarioActual.Id;
                    solpEntity.FechaModificacion = DateTime.Now;

                    pliegoEntity = solpEntity.Pliego;
                }
            }
            else
            {
                solpEntity = new Solp()
                {
                    UsuarioCreacion_Id = solp.UsuarioActual.Id,
                    FechaCreacion = DateTime.Now
                };

                var estadoIncompletoCodigo = EstadoDocumentoSolp.Incompleto.Code();
                var estadoIncompleto = repositorio.Obtener<TablaEstado>(x => x.Tabla == TablasEstado.EstadoDocumento && x.Codigo == estadoIncompletoCodigo);
                solpEntity.EstadoDocumento_Id = estadoIncompleto.Id;

                solpEntity.Pliego = new Pliego();
                pliegoEntity = solpEntity.Pliego;

                repositorio.Agregar(solpEntity);
            }
             
            if (solpEntity != null)
            {
                solpEntity.ClaseDocumento_Id = solp.ClaseDocumentoId;

                pliegoEntity.NombreObra = solp.NombreDeObra;
                pliegoEntity.FiscalContrato = solp.FiscalContrato;
                pliegoEntity.Telefono = solp.Telefono;
                pliegoEntity.Email = solp.Email;
                pliegoEntity.FechaHoraEntrega = solp.FechaHoraEntrega?.ToLocalTime();
                pliegoEntity.SupervisorSector = solp.SupervisorSector;
                pliegoEntity.SupervisorTrabajo = solp.SupervisorTrabajo;
                
                pliegoEntity.TieneVisitaObra = solp.TieneVisitaObra;
                pliegoEntity.TieneVisitaObraMasiva = solp.TieneVisitaObraMasiva;
                pliegoEntity.TieneObradores = solp.TieneObradores;
                pliegoEntity.TieneMedioElevacion = solp.TieneMedioElevacion;
                pliegoEntity.TieneTecnicoSeguridad = solp.TieneTecnicoSeguridad;
                pliegoEntity.TieneDescripcionTecnica = solp.TieneDescripcionTecnica;
                pliegoEntity.TieneDocumentacionTecnica = solp.TieneDocumentacionTecnica;
                pliegoEntity.FechaHoraLimiteConsulta = solp.FechaHoraLimiteConsulta?.ToLocalTime();
                pliegoEntity.ObservacionesGeneracion = solp.ObservacionesGeneracion;
                pliegoEntity.ObservacionesCotizacion = solp.ObservacionesCotizacion;
                pliegoEntity.DiasEjecucion = solp.DiasEjecucion;
                pliegoEntity.JornadaLaboralDias = solp.JornadaLaboral != null ? string.Join(",", solp.JornadaLaboral.Select(x=>(int)x)) : string.Empty;
                pliegoEntity.JornadaLaboralHorasDesde = solp.JornadaLaboralDesde?.ToLocalTime();
                pliegoEntity.JornadaLaboralHorasHasta = solp.JornadaLaboralHasta?.ToLocalTime();

                if(solp.TieneVisitaObraMasiva && solp.VisitasObraMasiva != null)
                {
                    if (pliegoEntity.VisitasMasivas == null)
                    {
                        pliegoEntity.VisitasMasivas = new List<PliegoVisita>();
                    }
                    var idVisita = -1;

                    foreach (var visita in solp.VisitasObraMasiva)
                    {
                        var visitaExistente = pliegoEntity.VisitasMasivas.FirstOrDefault(x => x.Codigo == visita.Codigo);

                        if(visitaExistente != null)
                        {
                            visitaExistente.FechaHora = visita.FechaHora.ToLocalTime();
                        }
                        else
                        {
                            pliegoEntity.VisitasMasivas.Add(new PliegoVisita()
                            {
                                Codigo = visita.Codigo,
                                Id = idVisita--,
                                FechaHora = visita.FechaHora.ToLocalTime()
                            }) ;
                        }
                    }
                }

                if (solp.Adjuntos != null && pliegoEntity.Archivos != null)
                {
                    var archivosParaBorrar = pliegoEntity.Archivos.Where(x => !solp.Adjuntos.Select(y=>y.Id).Contains(x.Id)).ToList();

                    foreach (var archivo in archivosParaBorrar)
                    {
                        repositorio.Remover<Archivo>(archivo);
                    }
                }

                if (pliegoEntity.Archivos == null)
                {
                    pliegoEntity.Archivos = new List<Archivo>();
                }
            }

            repositorio.GuardarCambios();

            solp.Id = solpEntity.Id;

            var rutaArchivo = string.Concat(ObtenerRutaArchivos(solpEntity.Id), "/", FileKeys.EspecificacionesTecnicasPliego, ".txt");
            
            Directory.CreateDirectory(ObtenerRutaArchivos(solpEntity.Id));
            File.WriteAllText(rutaArchivo, solp.EspecificacionesTecnicas);
            
            var archivoEspecificacionesTecnicasPliego = pliegoEntity.Archivos.FirstOrDefault(x => x.FileKey == FileKeys.EspecificacionesTecnicasPliego);

            if(archivoEspecificacionesTecnicasPliego == null)
            {
                pliegoEntity.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.EspecificacionesTecnicasPliego,
                    Ruta = rutaArchivo,
                });
            }

            GuardarAdjuntosSolp(solp, adjuntos, pliegoEntity);

            repositorio.GuardarCambios();

            solp.Adjuntos = pliegoEntity.Archivos.Where(x=>x.FileKey == FileKeys.AdjuntoSolp).Select(x => new ArchivoDto()
            {
                Id = x.Id,
                FileKey = x.FileKey,
                Nombre = x.ObtenerNombre(x.Ruta)
            }).ToList();
            
            return solp;
        }

        private string ObtenerRutaArchivos(int solpId)
        {
            return string.Format("{0}/Solp_{1}", rutaArchivosCompras, solpId);
        }

        private void GuardarAdjuntosSolp(SolpDto solp, HttpFileCollectionBase files, Pliego pliego)
        {
            var ruta = ObtenerRutaArchivos(solp.Id.Value);

            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var rutaArchivo = string.Concat(ruta, "/", Path.GetFileName(file.FileName));

                if (File.Exists(rutaArchivo))
                {
                    file.SaveAs(rutaArchivo);
                    continue;
                }

                Directory.CreateDirectory(ruta);

                pliego.Archivos.Add(new Archivo
                {
                    FileKey = FileKeys.AdjuntoSolp,
                    Ruta = rutaArchivo,
                });

                file.SaveAs(rutaArchivo);
            }
        }

        public string ObtenerRutaArchivo(int archivoId)
        {
            var archivo = repositorio.Obtener<Archivo>(archivoId);

            return archivo?.Ruta;
        }

        public List<TablaSapDto> ObtenerTablaSap(string tabla)
        {
            return repositorio.Listar<TablaSap>(x=>x.Tabla == tabla).Select(x=> new TablaSapDto(x)).ToList();
        }

        public List<TablaGeneralDto> ObtenerTablaGeneral(string tabla)
        {
            return repositorio.Listar<TablaGeneral>(x => x.Tabla == tabla).Select(x => new TablaGeneralDto(x)).ToList();
        }

        public List<CentroDireccionDto> ObtenerCentrosDireccion()
        {
            return repositorio.Listar<CentroDireccion>().Select(x=> new CentroDireccionDto(x)).ToList();
        }
    }
}
