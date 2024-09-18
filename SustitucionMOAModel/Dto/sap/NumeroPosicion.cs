namespace SustitucionMOAModel.Dto.sap
{
    public class NumeroPosicion
    {
        public int Id { get; set; }

        public NumeroPosicion(int id)
        {
            Id = id;
        }

        /// <summary>
        /// $"{Id:00000}"
        /// </summary>
        /// <returns></returns>
        public string AsPreqItem() => $"{Id:00000}";

        /// <summary>
        /// $"{Id:00000}"
        /// </summary>
        /// <returns></returns>
        public string AsDocItem() => $"{Id:00000}";

        /// <summary>
        /// $"{Id:0000000000}"
        /// </summary>
        /// <returns></returns>
        public string AsNumeroPaquete() => $"{Id:0000000000}";

        /// <summary>
        /// $"{Id:00}"
        /// </summary>
        /// <returns></returns>
        public string AsSerialNumber() => $"{Id:00}";

        public static implicit operator NumeroPosicion(int id) => new NumeroPosicion(id);
    }
}
