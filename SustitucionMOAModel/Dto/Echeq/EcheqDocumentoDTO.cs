using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAModel.Dto.Echeq
{
    public class EcheqDocumentoDTO
    {
        public string Contrato { get; set; }
        public string Pedido { get; set; }
        public string Sociedad { get; set; }
        public string Documento { get; set; }
        public string Ejercicio { get; set; }
        public string Fecha { get; set; }
        public string NumeroCOE { get; set; }
        public string Solapa { get; set; }
        public decimal ImporteMonedaDocumento { get; set; }
        public decimal ImporteEnPesos { get; set; }
        public string Moneda { get; set; }
        public bool DMBTRSpecified { get; set; }
        public bool WRBTRSpecified { get; set; }


        public EcheqDocumentoDTO() { }

        public EcheqDocumentoDTO(EcheqDocumento echeqDocumento) {
            this.Contrato = echeqDocumento.Contrato;
            this.Pedido = echeqDocumento.Pedido;
            this.Sociedad = echeqDocumento.Sociedad;
            this.Documento = echeqDocumento.Documento;
            this.Ejercicio = echeqDocumento.Ejercicio;
            this.Fecha = echeqDocumento.Fecha;
            this.NumeroCOE = echeqDocumento.NumeroCOE;
            this.Solapa = echeqDocumento.Solapa;
            this.ImporteMonedaDocumento = echeqDocumento.ImporteMonedaDocumento;
            this.ImporteEnPesos = echeqDocumento.ImporteEnPesos;
            this.Moneda = echeqDocumento.Moneda;    
        }

    }
}
