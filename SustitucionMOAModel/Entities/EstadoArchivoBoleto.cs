using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Models.WebApiMap.CNRT;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Entities
{
    public class EstadoArchivoBoleto
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Color { get; set; }

        public override bool Equals(object obj)
        {
            return obj is EstadoArchivoBoleto estado &&
                   Id == estado.Id &&
                   Nombre == estado.Nombre &&
                   Color == estado.Color;
        }

        [InverseProperty("EstadoArchivoBoleto")]
        public virtual ICollection<ArchivoBoleto> ArchivosBoleto { get; set; }

        public override int GetHashCode()
        {
            int hashCode = 173752721;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nombre);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Color);
            return hashCode;
        }
    }
}
