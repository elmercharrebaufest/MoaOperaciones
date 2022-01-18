using SustitucionMOAModel.Entities;
using System;

namespace SustitucionMOAModel.Dto.LogPesificacion
{
    public class LogPesificacionDto
    {
        public LogPesificacionDto()
        { }

        public LogPesificacionDto(Entities.LogPesificacion entidad)
        {
            Id = entidad.Id;
            Fecha = entidad.Fecha;
            Contrato = entidad.Contrato;
            Fijacion = entidad.Fijacion;
            CantidadKilos = entidad.CantidadKilos;
            Usuario = entidad.Usuario?.Mail;
            Proveedor = entidad.CodigoProveedor;
            RutaFisicaArchivo = entidad.Archivo?.Ruta;
            EsCargaMasiva = entidad.EsCargaMasiva;
            IdArchivo = entidad.IdArchivo;
            Estado = entidad.EnvioExitoso ? "Se envio correctamente.": "Error al enviar";
        }

        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int? Contrato { get; set; }

        public int? Fijacion { get; set; }

        public decimal? CantidadKilos { get; set; }

        public string Usuario { get; set; }

        public string Proveedor { get; set; }

        public string RutaFisicaArchivo { get; set; }

        public int? IdArchivo { get; set; }

        public bool EsCargaMasiva { get; set; }
        public string Estado { get; set; }
    }
}
