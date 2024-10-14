using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace SustitucionMOAModel.Entities
{
    public class Archivo
    {
        [Key]
        public int Id { get; set; }

        [StringLength(400)]
        public string FileKey { get; set; }

        public string Ruta { get; set; }

        public bool? ArchivoSap { get; set; }
        

        public override bool Equals(object obj)
        {
            return obj is Archivo archivo &&
                   Id == archivo.Id &&
                   FileKey == archivo.FileKey &&
                   Ruta == archivo.Ruta;
        }

        [InverseProperty("Archivos")]
        public virtual ICollection<Comentario> Comentarios { get; set; }

        [InverseProperty("Archivos")]
        public virtual ICollection<Pliego> Pliegos { get; set; }
        [InverseProperty("Archivos")]
        public virtual ICollection<Circular> Circular { get; set; }
        [InverseProperty("Archivos")]
        public virtual ICollection<Cotizacion> Cotizaciones { get; set; }
        public string ObtenerNombre(string ruta)
        {
            if (Path.GetFileName(ruta) != null)
                return Path.GetFileName(ruta);

            return "";
        }
        public string ObtenerNombre()
        {
            return this.ObtenerNombre(this.Ruta);
        }

        public override int GetHashCode()
        {
            int hashCode = 173752721;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileKey);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Ruta);
            return hashCode;
        }
    }
}
