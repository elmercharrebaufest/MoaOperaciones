namespace SustitucionMOAModel.Dto.sap
{
    public class NumeroSubPosicion
    {
        public int Id { get; set; }

        public NumeroSubPosicion(int id)
        {
            Id = id;
        }

        /// <summary>
        /// $"{Id:000000000}0"
        /// </summary>
        /// <returns></returns>
        public string AsServiceLineNumber() => $"{Id:000000000}0";

        public static implicit operator NumeroSubPosicion(int id) => new NumeroSubPosicion(id);
    }
}
