using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class ChatComprasDto
    {    
        public List<ChatInternoComprasDto> Mensajes { get; set; }
        public int Solp_Id { get; set; }
        public string FechaCreacion { get; set; }
        public DateTime FechaCreacionDate { get; set; }

        public List<ProveedorDto> Proveedores { get; set; }
        public int UsuarioActualId { get; set; }
    }

    public class ChatInternoComprasDto
    {
        public int Id { get; set; }
        public int Usuario_Id { get; set; }
        public string RolUsuario { get; set; }
        public int Solp_Id { get; set; }
        public string FechaEnvio { get; set; }
        public DateTime FechaEnvioDate { get; set; }

        public bool Leido { get; set; }
        public string Mensaje { get; set; }
        public string Mail { get; set; }
        public string FechaDiaEnvio { get; set; }
    }
}