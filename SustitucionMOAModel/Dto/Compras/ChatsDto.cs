using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class ChatsDto
    {    
        public List<ChatProveedoresDto> ChatProveedores { get; set; }
        public ChatComprasDto ChatCompras { get; set; }
    }
    
}