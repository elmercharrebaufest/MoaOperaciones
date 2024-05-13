namespace SustitucionMOAModel.Dto
{
    public class DatosUltimaSolpDto
    {
        public string FiscalContrato { get; set; }
        public string EmailFiscalContrato { get; set; }
        public string Telefono { get; set; }
        public TablaSapDto ClaseDocumento { get; set; }
        public TablaGeneralDto TipoPosicion { get; set; }
        public TablaSapDto Almacen { get; set; }
        public TablaSapDto CuentaMayor { get; set; }
        public TablaSapDto CuentaMayorSP { get; set; }
        public TablaSapDto GrupoCompras { get; set; }
        public TablaSapDto Centro { get; set; }
    }
}
