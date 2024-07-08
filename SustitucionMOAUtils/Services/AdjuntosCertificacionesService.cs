using SustitucionMOAModel.Dto.Compras;
using SustitucionMOAModel.Entities;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class AdjuntosCertificacionesService : IAdjuntosCertificacionesService
    {

        protected readonly IAzureService _azureService;
        private readonly IRepositorio _repositorio;

        public AdjuntosCertificacionesService(IAzureService azureService, IRepositorio repositorio)
        {
            this._azureService = azureService;
            this._repositorio = repositorio;
        }

        public async Task<List<string>> AdjuntarAsync(HttpFileCollectionBase files, string name)
        {
            DateTime dateTime = DateTime.UtcNow;

            string blobName = string.Empty;
            List<string> blobNames = new List<string>();

            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    string nombreArchivo = file.FileName;
                    string extensionArchivo = Path.GetExtension(nombreArchivo);

                    blobName = $"{Guid.NewGuid().ToString()}-Adjuntos";

                    await _azureService.SubirArchivoABlobStorageAsync(file, blobName, "certificaciones");

                    AdjuntosEntradasDeServicio adjuntos = new AdjuntosEntradasDeServicio();
                    adjuntos.NombreEnBlob = blobName;
                    adjuntos.NroESTemporal = string.Empty;
                    adjuntos.NombreArchivo = nombreArchivo;
                    adjuntos.Extension = extensionArchivo;

                    _repositorio.Agregar(adjuntos);

                    blobNames.Add(blobName);
                }
                
                _repositorio.GuardarCambios();
            }
            
            return blobNames;
        }


        public async Task<List<ESAdjuntosDto>> GetAdjuntos(string idESTemporal)
        {
            List<ESAdjuntosDto> adjuntosDtoList = new List<ESAdjuntosDto>();

            if (!idESTemporal.StartsWith("T_"))
            {
                idESTemporal = _repositorio.Obtener<Aprobaciones>(x => x.NRO_ES_SAP.ToString() == idESTemporal).NRO_ES_LOCAL;
            }

            if (!string.IsNullOrEmpty(idESTemporal))
            {
                var adjuntos = _repositorio.Listar<AdjuntosEntradasDeServicio>(x => x.NroESTemporal.Contains(idESTemporal));

                if (adjuntos.Count > 0)
                {
                    foreach (var adjunto in adjuntos)
                    {
                        ESAdjuntosDto adjuntoDto = new ESAdjuntosDto();

                        
                        var blobAdjunto = await _azureService.ObtenerArchivoBlobStorageAsync(adjunto.NombreEnBlob, "certificaciones");
                        blobAdjunto.Position = 0;
                        adjuntoDto.Adjuntos = blobAdjunto.GetBuffer();
                        adjuntoDto.Extension = adjunto.Extension;
                        adjuntoDto.NombreArchivo = adjunto.NombreArchivo;

                        adjuntosDtoList.Add(adjuntoDto);
                    }
                }

            }
            
            return adjuntosDtoList;

        }
    }
}
