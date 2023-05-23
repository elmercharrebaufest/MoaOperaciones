using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Enums;
using System;
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

        public DateTime? UltimaEdicion { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public string RazonSocialCorredor { get; set; }
        public int IdTipoUsuario { get; set; }
        public int IdTipoProveedor { get; set; }

        public bool? IngresoAPlanta { get; set; }

        public bool? AltaInterna { get; set; }

        public bool? ContieneDocumentacionFisica { get; set; }

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
            FechaSolicitud = proveedor.FechaSolicitud;
            Comercial = proveedor.Comercial;
            IdTipoProveedor = proveedor.TipoProveedor.Id;
            //if (proveedor.UsuariosAsociados.Count() > 0)
            //{
            //    if (proveedor.UsuariosAsociados.First() is UsuarioGranos)
            //    {
            //        Comercial = (proveedor.UsuariosAsociados.First() as UsuarioGranos).Comercial;
            //    }
            //}

            EstadoSIPER = proveedor.EstadoSIPER;

            if (proveedor.HistorialAprobaciones != null && proveedor.HistorialAprobaciones.Count() > 0)
            {
                UltimaEdicion = proveedor.HistorialAprobaciones.OrderByDescending(x => x.Fecha).FirstOrDefault().Fecha;
                HistorialAprobaciones = proveedor.HistorialAprobaciones.Select(a => new ProveedorHistorialAprobacionDto(a)).ToList();
            }
            else
            {
                UltimaEdicion = null;
                HistorialAprobaciones = new List<ProveedorHistorialAprobacionDto>();
            }

            //Para el tipo proveedor No Granos
            ContieneDocumentacionFisica = proveedor.ContieneDocumentacionFisica;
        }

        public ProveedorDto(Models.WSMapMOA.Usuario.Usuario x)
        {
            CodigoProveedor = x.vendedor;
            RazonSocial = x.usuario;
        }

        public override bool Equals(object obj)
        {
            return obj is ProveedorDto dto &&
                   Id == dto.Id &&
                   CUIT == dto.CUIT &&
                   RazonSocial == dto.RazonSocial &&
                   CodigoProveedor == dto.CodigoProveedor &&
                   Mail == dto.Mail &&
                   EstadoAprobacion == dto.EstadoAprobacion &&
                   Observaciones == dto.Observaciones &&
                   IdDataAgro == dto.IdDataAgro &&
                   IdComercialDataAgro == dto.IdComercialDataAgro &&
                   EstadoAprobacionDescripcion == dto.EstadoAprobacionDescripcion &&
                   Comercial == dto.Comercial &&
                   SISAEstadoCuit == dto.SISAEstadoCuit &&
                   EstadoSIPER == dto.EstadoSIPER;
        }

        public override int GetHashCode()
        {
            int hashCode = 1873437470;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CUIT);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RazonSocial);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodigoProveedor);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mail);
            hashCode = hashCode * -1521134295 + EstadoAprobacion.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Observaciones);
            hashCode = hashCode * -1521134295 + IdDataAgro.GetHashCode();
            hashCode = hashCode * -1521134295 + IdComercialDataAgro.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EstadoAprobacionDescripcion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Comercial);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SISAEstadoCuit);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(EstadoSIPER);
            return hashCode;
        }
    }
}
