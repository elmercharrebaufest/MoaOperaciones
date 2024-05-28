using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class ChatProveedoresDto
    {    
        public List<ChatExternoComprasDto> Mensajes { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public string FechaCreacion { get; set; }
        public DateTime FechaCreacionDate { get; set; }
        public int UsuarioActualId { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string CuitProveedor { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }

    }

    public class ChatExternoComprasDto
    {
        public int Id { get; set; }
        public int PeticionDeOferta_Id { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime FechaEnvioDate { get; set; }
        public bool Leido { get; set; }
        public string Mensaje { get; set; }
        public int PeticionDeOfertaUsuario_Id { get; set; }
        public string RolUsuario { get; set; }
        public string FechaEnvio { get; set; }
        public string Mail { get; set; }
        public string FechaDiaEnvio { get; set; }

    }
}