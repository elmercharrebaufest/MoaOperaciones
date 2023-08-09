using SustitucionMOAModel.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models
{
    public class EcheqRequestModel
    {
        public string CodigoProveedor { get; set; }
        public int ProveedorId { get; set; }
        public int UsuarioCreacionId { get; set; }
        public string Contrato { get; set; }
        public string Pedido { get; set; }
        public string Documento { get; set; }
        public string Ejercicio { get; set; }
        public List<EcheqAperturaDto> Apertura { get; set; }

        public EcheqRequestModel(string pedido, string contrato) {
            this.Pedido = pedido;
            this.Contrato = contrato;
        }

        public EcheqRequestModel() { }
    }
}
