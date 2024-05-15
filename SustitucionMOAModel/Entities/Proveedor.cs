using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SustitucionMOAModel.Enums;

namespace SustitucionMOAModel.Entities
{
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string CodigoProveedor { get; set; }

        public virtual TipoUsuario TipoProveedor { get; set; }

        public string Mail { get; set; }
        public EstadoAprobacion EstadoAprobacion { get; set; }
        public string Observaciones { get; set; }
        public int? IdDataAgro { get; set; }
        public int? IdComercialDataAgro { get; set; }
        public string EstadoSIPER { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public int? IdProveedorCorredor { get; set; }

        [ForeignKey("IdProveedorCorredor")]
        public virtual Proveedor ProveedorCorredor { get; set; }

        public virtual ICollection<Archivo> Archivos { get; set; }

        [InverseProperty("Proveedores")]
        public virtual ICollection<Usuario> UsuariosAsociados { get; set; }

        [InverseProperty("Proveedor")]
        public virtual ICollection<ProveedorHistorialAprobacion> HistorialAprobaciones { get; set; } = new List<ProveedorHistorialAprobacion>();


        public bool? VinculoConEmpleadosDeMolinos { get; set; }
        public bool? VinculoConFuncionariosPublicos { get; set; }

        [InverseProperty("Proveedor")]
        public virtual ICollection<ProveedorRelacionConEmpleados> RelacionConEmpleados { get; set; }

        [InverseProperty("Proveedor")]
        public virtual ICollection<ProveedorRelacionConFuncionarios> RelacionConFuncionarios { get; set; }

        public string Comercial { get; set; }
        public string Telefono { get; set; }
        public bool? RealizarAnalisisNOSIS { get; set; }

        public int? IdRubro { get; set; }
        
        public string CondicionDePago { get; set; }
        public string ServicioPrestado { get; set; }
        public string OrganizacionDeCompra { get; set; }
        public string RazonDeEleccion { get; set; }
        public long? FacturacionAnual { get; set; }
        public string SolicitanteInterno { get; set; }

        [ForeignKey("IdRubro")]
        public virtual Rubro Rubro { get; set; }
        public bool? RequiereVerificacionCompras { get; set; }
        public int? IdSituacionIVA { get; set; }
        public int? IdIngresoBruto { get; set; }
        public string CBU { get; set; }

        public bool? IngresoAPlanta { get; set; }
        public bool? AltaInterna { get; set; }

        public bool? SiperObligatorio { get; set; }

        //public virtual ICollection<DeclaracionCampoSustentable> DeclaracionesCamposSustentables { get; set; }

        public bool? ContieneDocumentacionFisica { get; set; }

        public int? IdSolicitanteInternoAltaGranos { get; set; }
        [ForeignKey("IdSolicitanteInternoAltaGranos")]
        public virtual Usuario SolicitanteInternoAltaGranos { get; set; }
        public string EstadoSISA { get; set; }

        public bool EsRevendedor { get; set; }
    }
}
