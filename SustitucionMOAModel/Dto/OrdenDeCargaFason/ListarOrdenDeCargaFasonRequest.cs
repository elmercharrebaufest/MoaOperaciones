namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
	public class ListarOrdenDeCargaFasonRequest
	{
		public string MailUsuario { get; set; }
		public string FechaDesde { get; set; }
		public string FechaHasta { get; set; }
        public bool EsCorredor { get; set; }
    }
}
