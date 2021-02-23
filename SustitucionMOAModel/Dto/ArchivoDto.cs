using SustitucionMOAModel.Entities;
using System.Collections.Generic;
using System.IO;

namespace SustitucionMOAModel.Dto
{
    public class ArchivoDto
    {
        public int Id { get; set; }
        public string FileKey { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }

        public ArchivoDto()
        {

        }

        public ArchivoDto(Archivo archivo)
        {
            Id = archivo.Id;
            FileKey = archivo.FileKey;
            Nombre = ObtenerNombre(archivo.Ruta);
            Ruta = archivo.Ruta;
        }
        private string ObtenerNombre(string ruta)
        {
            if (Path.GetFileName(ruta) != null)
                return Path.GetFileName(ruta);

            return "";
        }

        public override bool Equals(object obj)
        {
            return obj is ArchivoDto dto &&
                   Id == dto.Id &&
                   FileKey == dto.FileKey &&
                   Nombre == dto.Nombre;
        }

        public override int GetHashCode()
        {
            int hashCode = 2018846538;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileKey);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nombre);
            return hashCode;
        }
    }
}
