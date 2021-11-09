using System.ComponentModel.DataAnnotations;

namespace SustitucionMOAModel.Entities
{
    public class ServicioSolp
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int CodigoSap { get; set; }//SERVICE
        public string Descripcion { get; set; }//SHORT TEXT
        public int? GrupoArticulos { get; set; }//MATL GROUP
        public string TipoServicio { get; set; }//SERV CAT
        public string AmbitoServicio { get; set; }//SERV TYPE
        public int Edicion { get; set; }//EDITION
        public string UnidadMedidaBase { get; set; }//BASE UOM
        public string SSCItem { get; set; }//SSCItem
    }
}