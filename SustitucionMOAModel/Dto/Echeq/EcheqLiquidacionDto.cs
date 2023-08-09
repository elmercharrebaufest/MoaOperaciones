using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAModel.Dto
{
    public class EcheqLiquidacionDto
    {
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
        public Usuario UsuarioCreacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }
        public string Contrato { get; set; }
        public string Pedido { get; set; }

        public List<EcheqAperturaDto> Aperturas { get; set; } = new List<EcheqAperturaDto>();


        public EcheqLiquidacionDto() { }

        public EcheqLiquidacionDto(EcheqLiquidacion echeqLiquidacion) 
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
