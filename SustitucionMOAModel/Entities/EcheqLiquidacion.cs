using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Dto;

namespace SustitucionMOAModel.Entities
{
    public class EcheqLiquidacion
    {
        [Key]
        public int Id { get; set; }
        public int UsuarioCreacionId { get; set; }
        public int? UsuarioModificacionId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int EcheqNegocioId { get; set; }
        public string Documento { get; set; }
        public string Ejercicio { get; set; }
        public string Fecha { get; set; }
        public string NumeroCOE { get; set; }
        public string Solapa { get; set; }
        public decimal ImporteMonedaDocumento { get; set; }
        public decimal ImporteEnPesos { get; set; }
        public string Moneda { get; set; }
        public bool MarcaCheque { get; set; }

        [ForeignKey("UsuarioCreacionId")]
        public virtual Usuario UsuarioCreacion { get; set; }

        [ForeignKey("UsuarioModificacionId")]
        public virtual Usuario UsuarioModificacion { get; set; }

        [ForeignKey("EcheqNegocioId")]
        public virtual EcheqNegocio EcheqNegocio { get; set; }

        [InverseProperty("EcheqLiquidacion")]
        public virtual List<EcheqApertura> Aperturas { get; set; } = new List<EcheqApertura>();

        public EcheqLiquidacion() { }

        public EcheqLiquidacion(EcheqLiquidacionDto echeqLiquidacion)
        {
            this.Id = echeqLiquidacion.Id;
            this.UsuarioCreacionId = echeqLiquidacion.UsuarioCreacionId;
            this.UsuarioModificacionId = echeqLiquidacion.UsuarioModificacionId;
            this.FechaCreacion = echeqLiquidacion.FechaCreacion;
            this.FechaModificacion = echeqLiquidacion.FechaModificacion;
            this.EcheqNegocioId = echeqLiquidacion.EcheqNegocioId;
            this.Documento = echeqLiquidacion.Documento;
            this.Ejercicio = echeqLiquidacion.Ejercicio;
            this.Fecha = echeqLiquidacion.Fecha;
            this.NumeroCOE = echeqLiquidacion.NumeroCOE;
            this.Solapa = echeqLiquidacion.Solapa;
            this.ImporteMonedaDocumento = echeqLiquidacion.ImporteMonedaDocumento;
            this.ImporteEnPesos = echeqLiquidacion.ImporteEnPesos;
            this.Moneda = echeqLiquidacion.Moneda;
            this.MarcaCheque = echeqLiquidacion.MarcaCheque;
            this.UsuarioCreacion = echeqLiquidacion.UsuarioCreacion;
            this.UsuarioModificacion = echeqLiquidacion.UsuarioModificacion;

        }

    }
}
