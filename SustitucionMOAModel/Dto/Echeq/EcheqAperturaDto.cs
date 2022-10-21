using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAModel.Dto
{
    public class EcheqAperturaDto
    {
        public int Id { get; set; }
        public int OrdenCheque { get; set; }
        public int UsuarioCreacionId { get; set; }
        public int? UsuarioModificacionId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int EcheqLiquidacionId { get; set; }
        public decimal ImporteCheque { get; set; }
        public Usuario UsuarioCreacion { get; set; }
        public Usuario UsuarioModificacion { get; set; }
        public EcheqLiquidacion EcheqLiquidacion { get; set; }

        public EcheqAperturaDto() { }
        public EcheqAperturaDto(EcheqApertura echeqApertura)
        {
            this.Id = echeqApertura.Id;
            this.OrdenCheque = echeqApertura.OrdenCheque;
            this.UsuarioCreacionId = echeqApertura.UsuarioCreacionId;
            this.UsuarioModificacionId = echeqApertura.UsuarioModificacionId;
            this.FechaCreacion = echeqApertura.FechaCreacion;
            this.FechaModificacion = echeqApertura.FechaModificacion;
            this.EcheqLiquidacionId = echeqApertura.EcheqLiquidacionId;
            this.ImporteCheque = echeqApertura.ImporteCheque;
            this.UsuarioCreacion = echeqApertura.UsuarioCreacion;
            this.UsuarioModificacion = echeqApertura.UsuarioModificacion;
            this.EcheqLiquidacion = echeqApertura.EcheqLiquidacion;
        }
    }

}
