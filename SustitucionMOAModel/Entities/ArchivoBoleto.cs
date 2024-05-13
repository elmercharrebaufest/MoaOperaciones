using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using SustitucionMOAModel.Dto.ArchivoBoleto;
using SustitucionMOAModel.CustomExceptions;

namespace SustitucionMOAModel.Entities
{
    public class ArchivoBoleto
    {
        [Key]
        public int Id { get; set; }
        public string NombreArchivo { get; set; }
        public string CUIT { get; set; }
        public int EstadoArchivoBoleto_Id { get; set; }
        public int UsuarioCreacion_Id { get; set; }
        public DateTime FechaCarga { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        
        public override bool Equals(object obj)
        {
            return obj is ArchivoBoleto archivo &&
                   Id == archivo.Id &&
                   NombreArchivo == archivo.NombreArchivo &&
                   EstadoArchivoBoleto_Id == archivo.EstadoArchivoBoleto_Id &&
                   FechaCarga == archivo.FechaCarga &&
                   FechaActualizacion == archivo.FechaActualizacion &&
                   UsuarioCreacion_Id == archivo.UsuarioCreacion_Id;
        }

        [ForeignKey("EstadoArchivoBoleto_Id")]
        public virtual EstadoArchivoBoleto EstadoArchivoBoleto { get; set; }
        [ForeignKey("UsuarioCreacion_Id")]
        public virtual Usuario UsuarioCreador { get; set; }

        public override int GetHashCode()
        {
            int hashCode = 173752721;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NombreArchivo);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUIT);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FechaCarga.ToLongDateString());
            return hashCode;
        }

        public ArchivoBoleto(CrearReqArchivoBoletoDto data, EstadoArchivoBoleto estado)
        {
            if (data.ProveedorUsado == null || data.UsuarioCreador == null)
            {
                throw new ValidationCustomException("Asigne un proveedor o usuario creador correcto.");
            }
            NombreArchivo = data.Archivo.FileName;
            CUIT = data.ProveedorUsado.CUIT;
            UsuarioCreacion_Id = data.UsuarioCreador.Id;
            FechaCarga = DateTime.Now;
            FechaActualizacion = null;
            EstadoArchivoBoleto = estado;
        }
        public string ObtenerReferenciaBlob()
        {
            return $"{Id}_{NombreArchivo}";
        }
    }
}
