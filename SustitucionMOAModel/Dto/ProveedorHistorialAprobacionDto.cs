using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class ProveedorHistorialAprobacionDto
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string EstadoAprobacionDescripcion { get { return this.EstadoAprobacion.ToFriendlyString(); } }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public string Observacion { get; set; }
        public DateTime Fecha { get; set; }
        public string ObservacionParaProveedor { get; set; }
        public int Proveedor_Id { get; set; }

        public ProveedorHistorialAprobacionDto() { }

        public ProveedorHistorialAprobacionDto(ProveedorHistorialAprobacion historial)
        {
            Id = historial.Id;
            EstadoAprobacion = historial.EstadoAprobacion;
            Fecha = historial.Fecha;
            Observacion = historial.Observacion;
            Usuario = historial.Usuario.Mail;
            ObservacionParaProveedor = historial.ObservacionParaProveedor;
        }

        public override bool Equals(object obj)
        {
            return obj is ProveedorHistorialAprobacionDto dto &&
                   Id == dto.Id &&
                   Usuario == dto.Usuario &&
                   EstadoAprobacionDescripcion == dto.EstadoAprobacionDescripcion &&
                   Observacion == dto.Observacion &&
                   Fecha == dto.Fecha;
        }

        public override int GetHashCode()
        {
            int hashCode = 1815923877;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Usuario);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EstadoAprobacionDescripcion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Observacion);
            hashCode = hashCode * -1521134295 + Fecha.GetHashCode();
            return hashCode;
        }
    }
}
