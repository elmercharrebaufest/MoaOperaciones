using System.ComponentModel.DataAnnotations;


namespace SustitucionMOAModel.Entities
{
    public class TipoUsuario
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }

        public  bool EsCorredor {  get
            {
                return NombreCorto != null && NombreCorto == "CORR";
            }
        }
        public bool EsCliente
        {
            get
            {
                return NombreCorto != null && NombreCorto == "CLI";
            }
        }

    }
}
