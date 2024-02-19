using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class SolpPosicion
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int Solp_Id { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int? TipoPosicion_Id { get; set; }
        public int? TipoImputacion_Id { get; set; }
        public DateTime? FechaEntregaServicio { get; set; }
        public DateTime? FechaLiberacion { get; set; }
        public int? PlazoEntrega { get; set; }
        public int Centro_Id { get; set; }
        public int? Almacen_Id { get; set; }
        public string NombreEntrega { get; set; }
        public string CalleEntrega { get; set; }
        public string NumeroEntrega { get; set; }
        public string CpEntrega { get; set; }
        public string PaisEntrega { get; set; }
        public int? GrupoCompras_Id { get; set; }
        public string Solicitante { get; set; }
        public string NroNecesidad { get; set; }
        public string TextoSuministro { get; set; }
        public string Motivo { get; set; }
        public string Modelo { get; set; }
        public int? GrupoArticulo_Id { get; set; }
        public int? Moneda_Id { get; set; }
        public bool Estado { get; set; }
        public int? Indice { get; set; }
        public string Tarea { get; set; }
        public decimal? Cantidad { get; set; }
        public int? Unidad_Id { get; set; }
        public decimal? PrecioBruto { get; set; }
        public int? ServicioSolp_Id { get; set; }
        public int? MaterialSolp_Id { get; set; }

        public int? ValorTipoImputacion_Id { get; set; }
        public int? CuentaMayor_Id { get; set; }
        public bool? EsConcluido { get; set; }
        public int? ProvinciaId { get; set; }

        //Contrato Marco
        public string NumeroContratoSuperior { get; set; }
        public string NumeroPosicionContratoSuperior { get; set; }
        public string NombreProveedor { get; set; }
        public string ProveedorFijo { get; set; }
        public string OrganizacionCompras { get; set; }
        public string NumeroPedido { get; set; }
        public int? ProveedorAdjudicado_Id { get; set; }
        public string RegistroInfoNro { get; set; }
        public string OrganizacionDeComprasCodigo { get; set; }


        public int? CantidadSubposicionesEnSAP { get; set; }

        [ForeignKey("Solp_Id")]
        public virtual Solp Solp { get; set; }
        [ForeignKey("TipoPosicion_Id")]
        public virtual TablaGeneral TipoPosicion { get; set; }
        [ForeignKey("TipoImputacion_Id")]
        public virtual TablaGeneral TipoImputacion { get; set; }
        [ForeignKey("Centro_Id")]
        public virtual TablaSap Centro { get; set; }
        [ForeignKey("Almacen_Id")]
        public virtual TablaSap Almacen { get; set; }
        [ForeignKey("GrupoCompras_Id")]
        public virtual TablaSap GrupoCompras { get; set; }
        [ForeignKey("GrupoArticulo_Id")]
        public virtual TablaSap GrupoArticulo { get; set; }
        [ForeignKey("Moneda_Id")]
        public virtual TablaSap Moneda { get; set; }

        [ForeignKey("ServicioSolp_Id")]
        public virtual ServicioSolp ServicioSolp { get; set; }

        [ForeignKey("MaterialSolp_Id")]
        public virtual MaterialSolp MaterialSolp { get; set; }

        [ForeignKey("Unidad_Id")]
        public virtual TablaSap Unidad { get; set; }
        [ForeignKey("CuentaMayor_Id")]
        public virtual TablaSap CuentaMayorSap { get; set; }
        [ForeignKey("ValorTipoImputacion_Id")]
        public virtual TablaSap TipoImputacionSap { get; set; }
        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }
        [ForeignKey("ProveedorAdjudicado_Id")]
        public virtual Usuario ProveedorAdjudicado { get; set; }
        public virtual ICollection<SolpSubposicion> Subposiciones { get; set; }
        public virtual ICollection<SolpProveedor> Proveedores { get; set; }

        [InverseProperty("SolpPosicion")]
        public virtual ICollection<PeticionDeOfertaSolpPosicion> Peticiones { get; set; }

    }
}
