namespace SustitucionMOAModel.Dto
{
    public class EstadoSolicitudOrdenDeCargaBaseDto
    {
        public int OrdenId { get; set; }
        public string MailUsuario { get; set; }
        public bool Aprobado { get; set; }

        public EstadoSolicitudOrdenDeCargaBaseDto(int ordenId, string mailUsuario, bool aprobado)
        {
            OrdenId = ordenId;
            MailUsuario = mailUsuario;
            Aprobado = aprobado;
        }
    }
}
