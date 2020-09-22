using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SustitucionMOAModel.Enums;
using SustitucionMOAModel.Entities;

namespace SustitucionMOAModel.Dto
{
    public class ProveedorHistorialAprobacionDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string EstadoAprobacionDescripcion { get; set; }
        public string Observacion { get; set; }
        public DateTime Fecha { get; set; }

        public ProveedorHistorialAprobacionDto() { }

        public ProveedorHistorialAprobacionDto(ProveedorHistorialAprobacion historial)
        {
            Id = historial.Id;
            EstadoAprobacionDescripcion = historial.EstadoAprobacion.ToString();
            Fecha = historial.Fecha;
            Observacion = historial.Observacion;
            Usuario = historial.Usuario.Mail;
        }

    }
}
