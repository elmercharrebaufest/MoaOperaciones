namespace SustitucionMOAModel.Models.WebApiMap.ScatoRepositorio
{
    public class ConsultaListado<T>
    {
        public T[] Data { get; set; }

        public bool IsValid { get; set; }

        public MessageItem[] Messages { get; set; }
    }

    public class MessageItem
    {
        public string Message { get; set; }
        public int MessageType { get; set; }
    }
}
