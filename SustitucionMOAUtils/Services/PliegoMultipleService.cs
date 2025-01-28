// Ignore Spelling: solps Roslynator

using SustitucionMOAModel.Dto.PliegoMultiple;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Extensions;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ComprasDto = SustitucionMOAModel.Dto;

namespace SustitucionMOAUtils.Services
{
    public class PliegoMultipleService : IPliegoMultipleService
    {
        private readonly IRepositorio repositorio;

        private readonly IComprasService comprasService;

        public PliegoMultipleService(IRepositorio repositorio,
                                     IComprasService comprasService)
        {
            this.repositorio = repositorio;
            this.comprasService = comprasService;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1155:Use StringComparison when comparing strings", Justification = "Not supported by EF")]
        public List<PliegoDto> GetPliegosMultiples(string nombrePliego)
        {
            IQueryable<Pliego> pliegos = repositorio.ListarConsultable<Pliego>(pliego => pliego.Multiple);
            if (string.IsNullOrWhiteSpace(nombrePliego))
            {
                return pliegos
                    .ToList()
                    .ConvertAll(pliego => (PliegoDto)pliego);
            }
            else
            {
                return pliegos
                    .Where(p => p.NombreObra.ToLower().Contains(nombrePliego.ToLower()))
                    .ToList()
                    .ConvertAll(pliego => (PliegoDto)pliego);
            }
        }

        public List<SolpDto> GetSolpDisponiblesPliegosMultiple(string numeroSolp,
                                                               DateTime? fechaInicio,
                                                               DateTime? fechaFin,
                                                               IEnumerable<int> creador,
                                                               IEnumerable<string> fiscal,
                                                               bool sap,
                                                               bool mantenimiento,
                                                               int? pliegoId = null)
        {
            IEnumerable<string> codigosSapEstadosSolpValidos = new HashSet<string> { "02", "05" };
            IEnumerable<string> tiposSolpValidos = new HashSet<string> { "CON_PLIEGO", "SIN_PLIEGO" };
            IEnumerable<string> tiposPosicionSolpValidos = new HashSet<string> { "SERVICIO", "MATERIALES" };

            IQueryable<Solp> consultaSolp = repositorio
                .ListarConsultable<Solp>(solpQuery =>
                        codigosSapEstadosSolpValidos.Contains(solpQuery.EstadoSolpSap.CodigoSap)
                        && tiposSolpValidos.Contains(solpQuery.TipoSolp.Codigo)
                        && solpQuery.Posiciones.Any(posicion => tiposPosicionSolpValidos.Contains(posicion.TipoPosicion.Codigo))
                        && !(solpQuery.TrabajoYaHecho == true || solpQuery.Adicional == true || solpQuery.Urgencia == true || solpQuery.CondEspProveedorAsignado == true)
                        && !solpQuery.Pliego.Multiple
                        && !solpQuery.Posiciones.Any(posicion => posicion.AdjudicacionPosiciones.Any())
                    )
                ;

            List<Solp> solpsPrevias = new List<Solp>();
            if (pliegoId != null)
            {
                solpsPrevias = repositorio.Listar<Solp>(x => x.Pliego_Id == pliegoId);
            }

            if (!string.IsNullOrWhiteSpace(numeroSolp))
            {
                consultaSolp = consultaSolp
                    .Where(solp => solp.NroSolp.Contains(numeroSolp));
            }

            if (!(fechaInicio is null))
            {
                consultaSolp = consultaSolp
                    .Where(solp => solp.FechaCreacion >= fechaInicio);
            }

            if (!(fechaFin is null))
            {
                DateTime ff = new DateTime(fechaFin.Value.Year, fechaFin.Value.Month, fechaFin.Value.Day, 23, 59, 59, 999, fechaFin.Value.Kind);

                consultaSolp = consultaSolp
                    .Where(solp => solp.FechaCreacion <= ff);
            }

            if (creador?.Any() == true)
            {
                consultaSolp = consultaSolp
                    .Where(solp => solp.UsuarioCreacion_Id != null && creador.Contains(solp.UsuarioCreacion_Id.Value));
            }

            if (fiscal.Any())
            {
                consultaSolp = consultaSolp
                    .Where(solp => solp.Pliego != null && solp.Pliego.Email != null && fiscal.Contains(solp.Pliego.Email));
            }

            if (sap && mantenimiento)
            {
                consultaSolp = consultaSolp
                    .Where(solp => solp.TipoSolpSap == (int)TipoSolpSap.Sap || solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento);
            }
            else
            {
                if (sap)
                {
                    consultaSolp = consultaSolp
                        .Where(solp => solp.TipoSolpSap == (int)TipoSolpSap.Sap);
                }

                if (mantenimiento)
                {
                    consultaSolp = consultaSolp
                        .Where(solp => solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento);
                }
            }

            List<SolpDto> result = consultaSolp
                .ToList()
                .ConvertAll(solp => (SolpDto)solp);

            if (pliegoId != null)
            {
                List<SolpDto> dtoPrevias = solpsPrevias.ConvertAll(solp =>
                {
                    SolpDto dto = (SolpDto)solp;
                    dto.Selected = true;
                    return dto;
                });
                result.AddRange(dtoPrevias);
            }

            return result
                .DistinctBy(solp => solp.Id)
                .OrderByDescending(solp => solp.Selected)
                .ThenByDescending(solp => solp.FechaCreacion)
                .ToList();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1155:Use StringComparison when comparing strings", Justification = "EF does not support StringComparison")]
        public void CrearPliegoMultiple(ComprasDto.SolpDto pliegoData, HttpFileCollectionBase adjuntos, IEnumerable<int> solpsAsociar)
        {
            bool condEsp = comprasService.TieneCondicionEspecial(pliegoData);
            bool esEdicion = pliegoData.Pliego_Id != null;

            Pliego pliego = null;
            if (esEdicion)
            {
                pliego = repositorio.Obtener<Pliego>(pliegoData.Pliego_Id);
            }

            pliego = comprasService.GuardarPliego(pliegoData, adjuntos, condEsp, pliegoEntity: pliego, esPliegoMultiple: true);

            List<Solp> solps = repositorio.Listar<Solp>(solp => solpsAsociar.Contains(solp.Id));

            if (esEdicion) { DesvincularSolps(solps); }

            foreach (Solp solp in solps)
            {
                SolpDatosPreviosPliegoMultiple solpDatosPrevios = new SolpDatosPreviosPliegoMultiple
                {
                    Solp_Id = solp.Id,
                    Pliego_Id = solp.Pliego_Id,
                    EstadoDocumento_Id = solp.EstadoDocumento_Id,
                    TipoSolp_Id = solp.TipoSolp_Id
                };

                repositorio.Agregar(solpDatosPrevios);

                solp.Pliego = pliego;
                if (solp.TipoSolpSap == (int)TipoSolpSap.Sap || solp.TipoSolpSap == (int)TipoSolpSap.Mantenimiento)
                {
                    solp.EstadoDocumento_Id = (int)EstadoDocumentoSolp.Creado;
                }
                solp.TipoSolp = repositorio
                    .Obtener<TablaGeneral>(x => x.Tabla.ToLower() == "TipoSolp".ToLower() && x.Codigo.ToLower() == "CON_PLIEGO".ToLower());
            }

            repositorio.GuardarCambios();
        }

        public void EliminarPliegoMultiple(int idPliego)
        {
            Pliego pliego = repositorio.Obtener<Pliego>(idPliego)
                ?? throw new InvalidOperationException($"No se encuentra Pliego con id = {idPliego}");

            DesvincularSolps(pliego.Solps);

            repositorio.RemoverTodos(pliego.Archivos.ToList());
            repositorio.Remover(pliego);

            repositorio.GuardarCambios();
        }

        /// <summary>
        /// Desvincula Solp del pliego.
        /// </summary>
        /// <remarks>
        /// No se guardan los cambios en la base de datos. Esa responsabilidad recae en el método que llama a este.
        /// </remarks>
        /// <param name="solps"></param>
        /// <exception cref="NotImplementedException">Este método depende de la existencia de un back-up de datos
        /// para asignar el pliego original a la solp. Se lanza esta excepción en caso de no encontrar dicho back-up</exception>
        private void DesvincularSolps(ICollection<Solp> solps)
        {
            foreach (Solp solp in solps)
            {
                SolpDatosPreviosPliegoMultiple backUp = repositorio.Obtener<SolpDatosPreviosPliegoMultiple>(x => x.Solp_Id == solp.Id)
                    ?? throw new NotImplementedException("En caso de no encontrar el back-up...");

                solp.Pliego_Id = backUp.Pliego_Id;
                solp.EstadoDocumento_Id = backUp.EstadoDocumento_Id;
                solp.TipoSolp_Id = backUp.TipoSolp_Id;

                repositorio.Remover(backUp);
            }
        }

        public string GenerarZipPliego(int idPliego, string pathBase, out string mimeType)
        {
            Pliego pliego = repositorio.Obtener<Pliego>(idPliego) ?? throw new ArgumentException("Invalid Pliego ID");
            string middleFileName = pliego.NombreObra ?? "xxxx";
            string pdfFilename = $"Pliego-{middleFileName}-{DateTime.Now:yyyyMMdd}.pdf";
            string pdfFilePath = $"{pathBase}/{pdfFilename}";
            bool pdfPliegoDisponible = false;

            File.WriteAllBytes(pdfFilePath, comprasService.GenerarSolpPdf(idPliego, esPliego: true));
            pdfPliegoDisponible = true;

            if (pliego.Archivos?.Any<Archivo>(x => x.FileKey == FileKeys.AdjuntoSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolp || x.FileKey == FileKeys.AdjuntoCotizacionesSolpCondEsp) == true)
            {
                string filePath = comprasService.AgregarArchivosAlZipPliego(pliego.Archivos, middleFileName, pathBase, pdfPliegoDisponible, pdfFilePath, pdfFilename);
                mimeType = CustomMediaTypeNames.Application.Zip;
                return filePath;
            }

            mimeType = CustomMediaTypeNames.Application.Pdf;
            return pdfFilePath;
        }

        public TraerPliegoDto TraerPliegoId(int idPliego)
        {
            Pliego pliego = repositorio.Obtener<Pliego>(idPliego) ?? throw new ArgumentException($"Pliego con id {idPliego} no encontrado");

            ComprasDto.SolpDto pliegoReturn = comprasService.TraerSolpId(pliego.Solps.First().Id);
            pliegoReturn.Id = null;
            pliegoReturn.Pliego_Id = pliego.Id;


            return new TraerPliegoDto
            {
                Pliego = pliegoReturn,
                Solps = pliego.Solps.Select(x => x.Id),
            };
        }
    }
}
