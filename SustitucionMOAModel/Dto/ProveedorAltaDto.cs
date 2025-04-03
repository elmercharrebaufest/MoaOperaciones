using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class ProveedorAltaDto
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
        public string EstadoAprobacionDescripcion { get { return this.EstadoAprobacion.ToFriendlyString(); } }

        public virtual List<ProveedorHistorialAprobacionDto> HistorialAprobaciones { get; set; }
        public string Comercial { get; set; }
        public string SISAEstadoCuit { get; set; }
        public string EstadoSIPER { get; set; }

        public DateTime? UltimaEdicion { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public string RazonSocialCorredor { get; set; }
        public int IdTipoUsuario { get; set; }

        public bool? IngresoAPlanta { get; set; }

        public bool? AltaInterna { get; set; }

        public string Telefono { get; set; }
        public bool? RealizarAnalisisNOSIS { get; set; }

        public string CondicionDePago { get; set; }
        public string ServicioPrestado { get; set; }
        public string OrganizacionDeCompra { get; set; }
        public string RazonDeEleccion { get; set; }
        public long? FacturacionAnual { get; set; }
        public string SolicitanteInterno { get; set; }
        public DateTime? FechaAltaAceptada { get; set; }

        public string Rubro { get; set; }
        public bool? RequiereVerificacionCompras { get; set; }
        public int? IdSituacionIVA { get; set; }
        public string SituacionIVA { get { return ((SituacionIVA)(this.IdSituacionIVA ?? 0)).ToFriendlyString(); } }
        public int? IdIngresoBruto { get; set; }
        public string IngresoBruto { get { return ((IngresosBrutos)(this.IdIngresoBruto ?? 0)).ToFriendlyString(); } }
        public string CBU { get; set; }
        public bool? SiperObligatorio { get; set; }

        public bool? ContieneDocumentacionFisica { get; set; }
        public string TipoProveedorNombre { get; set; }
    }
}
