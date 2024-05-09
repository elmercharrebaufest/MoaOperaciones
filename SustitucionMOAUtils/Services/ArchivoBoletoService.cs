using SustitucionMOAAssets;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAModel.Dto.ArchivoBoleto;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Services
{
    public class ArchivoBoletoService: IArchivoBoletoService
    {
        private IRepositorio repositorio { get; set; }
        private IAzureService azureService { get; set; }

        private readonly Expression<Func<ArchivoBoleto,ArchivoBoletoDto>> proyectorDto = 
            ab => new ArchivoBoletoDto
            {
                Id= ab.Id,
                ColorEstado = ab.EstadoArchivoBoleto.Color,
                NombreArchivo = ab.NombreArchivo,
                NombreEstado = ab.EstadoArchivoBoleto.Nombre,
                FechaCarga = ab.FechaCarga
    };

        public ArchivoBoletoService(IRepositorio repositorio, IAzureService azureService)
        {
            this.repositorio = repositorio;
            this.azureService = azureService;
        }

        public List<ArchivoBoletoDto> ListarArchivosBoleto(ListarReqArchivoBoletoDto request)
        {
            var proveedor = repositorio.Obtener<Proveedor>(request.ProveedorId);
            request.CUIT = proveedor.CUIT;

            var listaDeArchivos = repositorio.ListarProyeccion(
                proyectorDto,
                ab => ab.CUIT == request.CUIT &&
                    request.FechaInicio <= ab.FechaCarga && ab.FechaCarga < request.FechaFinLimite
                );

            if(listaDeArchivos.Count == 0)
            {
                throw new InfoCustomException(string.Format(InfoMsg.SinRegistros, "archivos de boleto."));
            }

            return listaDeArchivos; 
        }

        public async Task<ArchivoBoletoDto> CrearArchivoBoleto(CrearReqArchivoBoletoDto data)
        {
            data.ProveedorUsado = repositorio.Obtener<Proveedor>(data.ProveedorId);
            data.UsuarioCreador = repositorio.Obtener<Usuario>(u=> u.Mail == data.EmailUsuario);
            var archivoAGuardar = new ArchivoBoleto(data, ObtenerEstadoPendiente());
            repositorio.Agregar(archivoAGuardar);
            repositorio.GuardarCambios();

            try
            {
                await azureService.SubirArchivoABlobStorageAsync(data.Archivo, archivoAGuardar.ObtenerReferenciaBlob(), "boletos");
            }
            catch (Exception _ex)
            {
                repositorio.Remover(archivoAGuardar);
                repositorio.GuardarCambios();
                throw;
            }


            return new ArchivoBoletoDto(archivoAGuardar);
        }

        private EstadoArchivoBoleto ObtenerEstadoPendiente()
        {
            return repositorio.Obtener<EstadoArchivoBoleto>(estado=> estado.Nombre=="Pendiente");
        }
    }
}