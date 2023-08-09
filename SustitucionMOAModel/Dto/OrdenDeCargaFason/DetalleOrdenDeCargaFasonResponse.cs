using AutoMapper;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class DetalleOrdenDeCargaFasonResponse
    {
        public OrdenDeCargaFasonDto Response { get; set; }

        //public DetalleOrdenDeCargaFasonResponse(Entities.OrdenDeCargaFason entity, bool esInterno)
        //{
        //    var config = new MapperConfiguration(cfg => cfg
        //        .CreateMap<Entities.OrdenDeCargaFason, OrdenDeCargaFasonDto>().ForMember(d => d.Cliente, o => o.MapFrom(s => s.Cliente.CodigoProveedor)));
        //    var mapper = config.CreateMapper();
        //    Response = new OrdenDeCargaFasonDto();

        //    var dto = mapper.Map<OrdenDeCargaFasonDto>(entity);
        //    dto.ColorSemaforo = dto.Estado.ObtenerSemaforo();
        //    dto.DescripcionEstado = esInterno ? dto.Estado.ToFriendlyString() : dto.Estado.ToUserFriendlyString();
        //    Response = dto;
        //}

        public DetalleOrdenDeCargaFasonResponse()
        {
        
        }
    }
}
