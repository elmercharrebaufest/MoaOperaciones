using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Models.WSMapMOA.Echeq;

namespace SustitucionMOAModel.Dto.Echeq
{
    public class EcheqObtenerContratoDTO
    {
        public string Contrato { get; set; }
        public List<EcheqDocumentoDTO> Documentos { get; set; }
        public string Pedido { get; set; }
        public decimal Kilos { get; set; }
        public bool kILOSFieldSpecified { get; set; }
        public decimal KilosPagados { get; set; }
        public bool kILOS_PAGADOSFieldSpecified { get; set; }
        public decimal Precio { get; set; }
        public bool pRECIOFieldSpecified { get; set; }
        public string Moneda { get; set; }
        public string Material { get; set; }
        public string DescripcionMaterial { get; set; }
        public string Fecha { get; set; }
        public string zLSCHField { get; set; }


        public EcheqObtenerContratoDTO() { }


        public EcheqObtenerContratoDTO(EcheqVisualizacionPendientePago visualizacionPendientePago) {

            this.Contrato = visualizacionPendientePago.Contrato;
            this.Pedido = visualizacionPendientePago.Pedido;
            this.Kilos = visualizacionPendientePago.Kilos;
            this.KilosPagados = visualizacionPendientePago.KilosPagados;
            this.Precio = visualizacionPendientePago.Precio;
            this.Moneda = visualizacionPendientePago.Moneda;
            this.Material = visualizacionPendientePago.Material;
            this.DescripcionMaterial = visualizacionPendientePago.DescripcionMaterial;
            this.Fecha = visualizacionPendientePago.Fecha;
            this.Documentos = visualizacionPendientePago.Documentos.Select(a => new EcheqDocumentoDTO(a)).ToList();
        }
    }

}
