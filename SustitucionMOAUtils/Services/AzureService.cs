using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using SustitucionMOARepositorio;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAUtils.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SustitucionMOAUtils.Services
{
    public class AzureService: IAzureService
    {
        protected readonly IRepositorio repositorio;
        protected readonly IComputerVisionClient visionClient;
        protected readonly CloudBlobClient blobClient;
        private const string NOMBRE_CONTENEDOR = "liquidaciones";

        public AzureService(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            this.visionClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(ConfigurationManager.AppSettings["ComputerVisionApiKey"]))
                { Endpoint = ConfigurationManager.AppSettings["ComputerVisionEndpoint"] };
            this.blobClient = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobStorageEndpoint"]).CreateCloudBlobClient(); 
        }

        public async Task<string> AnalizarImagenAsync(HttpPostedFileBase file)
        {
            var stream = await CrearNuevoStreamAsync(file);

            var resultado = await visionClient.ReadInStreamAsync(stream, "es");
            //Solo queremos quedarnos con el ID de la operación para poder consultarlo en background a la hora de generar el reporte.
            //Una vez que llamamos a la API para analizar una imagen, la operación y el resultado quedan disponibles por 48hs para ser consultados.
            return resultado.OperationLocation.Substring(resultado.OperationLocation.Length - 36);
        }

        public async Task<IList<string>> ObtenerResultadoOCRAsync(string operacionId)
        {
            ReadOperationResult results;

            do
            {
                results = await visionClient.GetReadResultAsync(Guid.Parse(operacionId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));
            
            return results.AnalyzeResult.ReadResults.SelectMany(rr => rr.Lines.Select(rrl => rrl.Text)).ToList();
        }

        public async Task SubirArchivoABlobStorageAsync(HttpPostedFileBase archivo, string coe)
        {
            var contenedor = blobClient.GetContainerReference(NOMBRE_CONTENEDOR);
            var referenciaArchivo = contenedor.GetBlockBlobReference($"{coe}{Path.GetExtension(archivo.FileName).ToLower()}");

            referenciaArchivo.Properties.ContentType = archivo.ContentType;

            var stream = await CrearNuevoStreamAsync(archivo);

            await referenciaArchivo.UploadFromStreamAsync(stream);
        }

        private static async Task<MemoryStream> CrearNuevoStreamAsync(HttpPostedFileBase file)
        {
            var stream = new MemoryStream();
            await file.InputStream.CopyToAsync(stream);

            file.InputStream.Seek(0, SeekOrigin.Begin);
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
