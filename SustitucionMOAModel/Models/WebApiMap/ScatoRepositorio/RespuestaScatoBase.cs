namespace SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio
{
    public abstract class RespuestaScatoBase
    {
        public bool IsValid { get; set; }

        public MessageItem[] Messages { get; set; }
    }

    public class MessageItem
    {
        public string MessageCode { get; set; }

        public string Message { get; set; }

        public int MessageType { get; set; }
    }
}
