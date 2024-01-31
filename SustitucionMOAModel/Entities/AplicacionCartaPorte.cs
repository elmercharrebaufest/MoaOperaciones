using SustitucionMOAModel.Dto.AplicacionCartaPorte;
using SustitucionMOAModel.Enums.SustitucionMOAModel.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SustitucionMOAModel.Entities
{
    public class AplicacionCartaPorte
    {
        [Key]
        public int Id { get; set; }
        public int Proveedor_Id { get; set; }
        [ForeignKey("Proveedor_Id")]
        public virtual Proveedor Proveedor { get; set; }
        public string Contrato { get; set; }
        public string CartaPorte { get; set; }
        public EstadoAplicacionCartaPorte Estado { get; set; }
        public int Kilogramos { get; set; }
        public int Usuario_Id { get; set; }
        [ForeignKey("Usuario_Id")]
        public virtual Usuario Usuario { get; set; }
        public string Error { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public AplicacionCartaPorte() { }
        public AplicacionCartaPorte(CrearAplicacionCartaPorte aplicacionACrear, Usuario usuario, Proveedor proveedorSeleccionado)
        {
            Usuario_Id = usuario.Id;
            Proveedor_Id = proveedorSeleccionado.Id;
            Contrato = aplicacionACrear.ContratoSeleccionado.NumeroContrato;
            CartaPorte = aplicacionACrear.CartaPorteSeleccionada.NumeroCartaPorte;
            Kilogramos = (int)aplicacionACrear.Kilogramos;
            Estado = EstadoAplicacionCartaPorte.Pendiente;
            FechaAlta = DateTime.Now;
            FechaActualizacion = null;
        }
    }
}
