using SustitucionMOAModel.Entities;
using System.IO;

namespace SustitucionMOAModel.Dto
{
    public class ArchivoDto
    {
        public int Id { get; set; }
        public string FileKey { get; set; }
        public string Nombre { get; set; }

        public ArchivoDto()
        {

        }

        public ArchivoDto(Archivo archivo)
        {
            Id = archivo.Id;
            FileKey = archivo.FileKey;
            Nombre = ObtenerNombre(archivo.Ruta);
        }

        private string ObtenerNombre(string ruta)
        {
            if (Path.GetFileName(ruta) != null)
                return Path.GetFileName(ruta);

            return "";
        }
    }
}
