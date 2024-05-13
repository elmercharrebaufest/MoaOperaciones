using System;
using EntitiesToSymbol =  SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto.ArchivoBoleto
{
    public class ArchivoBoletoDto
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; }
        public string ColorEstado { get; set; }
        public string NombreEstado { get; set; }

        public DateTime FechaCarga { get; set; }

        public ArchivoBoletoDto() { }
        public ArchivoBoletoDto(Entities.ArchivoBoleto archivo)
        {
            Id = archivo.Id;
            NombreArchivo = archivo.NombreArchivo;
            ColorEstado = archivo.EstadoArchivoBoleto.Color;
            NombreEstado = archivo.EstadoArchivoBoleto.Nombre;
            FechaCarga = archivo.FechaCarga;
        }
    }
}
