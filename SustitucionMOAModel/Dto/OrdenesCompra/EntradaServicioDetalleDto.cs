namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioDetalleDto
    {
        public int ID { get; set; }
        public string NumeroLinea { get; set; }
        public string NroPosicion { get; set; }
        public string PLN_PCKG { get; set; }
        public string CodigoServicio { get; set; }
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string UM { get; set; }
        public decimal Monto { get; set; }
        public string OrdenCompra { get; set; }
        public int NumeroServicio { get; set; }
        public string TextoBreveServicio { get; set; }
        public double CantidadReal { get; set; }
        public double CantidadAnterior { get; set; }
        public double Porcentaje { get; set; }
        public string CantidadCertificar { get; set; }
        public string PorcentajeCertificar { get; set; }
        public decimal? MontoCertificar { get; set; }
        public string NroRemito { get; set; }
        public string Ext_line { get; set; }
        public string FechaPrestacion { get; set; }
        /// <summary>
        /// Precio por unidad del item
        /// </summary>
        public decimal PrecioUnitario { get; set; }

    }
}
