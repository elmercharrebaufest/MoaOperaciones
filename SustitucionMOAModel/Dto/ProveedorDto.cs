using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAModel.Dto
{
    public class ProveedorDto
    {
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoProveedor { get; set; }
        public string Mail { get; set; }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public string Observaciones { get; set; }

        public int? IdDataAgro { get; set; }
        public int? IdComercialDataAgro { get; set; }
        public string EstadoAprobacionDescripcion { get; set; }

        public virtual List<ProveedorHistorialAprobacionDto> HistorialAprobaciones { get; set; }
        public string Comercial { get; set; }
        public string SISAEstadoCuit { get; set; }
        public string EstadoSIPER { get; set; }

        public ProveedorDto() { }
        public ProveedorDto(Proveedor proveedor)
        {
            CodigoProveedor = proveedor.CodigoProveedor ?? "";
            CUIT = proveedor.CUIT;
            EstadoAprobacion = proveedor.EstadoAprobacion;
            EstadoAprobacionDescripcion = proveedor.EstadoAprobacion.ToFriendlyString();
            Id = proveedor.Id;
            IdComercialDataAgro = proveedor.IdComercialDataAgro;
            IdDataAgro = proveedor.IdDataAgro;
            Mail = proveedor.Mail ?? "";
            Observaciones = proveedor.Observaciones;
            RazonSocial = proveedor.RazonSocial ?? "";
            Comercial = "";

            if (proveedor.UsuariosAsociados.Count() > 0)
            {
                if (proveedor.UsuariosAsociados.First() is UsuarioGranos)
                {
                    Comercial = (proveedor.UsuariosAsociados.First() as UsuarioGranos).Comercial;
                }
            }

            EstadoSIPER = proveedor.EstadoSIPER;

            if (proveedor.HistorialAprobaciones != null)
                HistorialAprobaciones = proveedor.HistorialAprobaciones.Select(a => new ProveedorHistorialAprobacionDto(a)).ToList();
            else
                HistorialAprobaciones = new List<ProveedorHistorialAprobacionDto>();
        }
    }
}
