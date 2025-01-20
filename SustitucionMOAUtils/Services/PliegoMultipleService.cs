// Ignore Spelling: solps Roslynator

using SustitucionMOAModel.Dto.PliegoMultiple;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
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
                                                               bool mantenimiento)
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

            return consultaSolp
                .OrderByDescending(solp => solp.FechaCreacion)
                .ToList()
                .ConvertAll(solp => (SolpDto)solp);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1155:Use StringComparison when comparing strings", Justification = "EF does not support StringComparison")]
        public void CrearPliegoMultiple(ComprasDto.SolpDto pliegoData, HttpFileCollectionBase adjuntos, IEnumerable<int> solpsAsociar)
        {
            bool condEsp = comprasService.TieneCondicionEspecial(pliegoData);

            Pliego pliego = comprasService.GuardarPliego(pliegoData, adjuntos, condEsp, esPliegoMultiple: true);

            List<Solp> solps = repositorio.Listar<Solp>(solp => solpsAsociar.Contains(solp.Id));

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

            foreach (Solp solp in pliego.Solps)
            {
                SolpDatosPreviosPliegoMultiple backUp = repositorio.Obtener<SolpDatosPreviosPliegoMultiple>(x => x.Solp_Id == solp.Id)
                    ?? throw new NotImplementedException("En caso de no encontrar el back-up...");

                solp.Pliego_Id = backUp.Pliego_Id;
                solp.EstadoDocumento_Id = backUp.EstadoDocumento_Id;
                solp.TipoSolp_Id = backUp.TipoSolp_Id;

                repositorio.Remover(backUp);
            }

            repositorio.RemoverTodos(pliego.Archivos.ToList());
            repositorio.Remover(pliego);

            repositorio.GuardarCambios();
        }
    }
}
