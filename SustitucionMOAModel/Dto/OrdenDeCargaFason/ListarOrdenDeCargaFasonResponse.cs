using AutoMapper;
using SustitucionMOAModel.Enums;
using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
	public class ListarOrdenDeCargaFasonResponse
	{
		public List<OrdenDeCargaFasonDto> Response { get; set; }

		//public ListarOrdenDeCargaFasonResponse(List<Entities.OrdenDeCargaFason> listEntity)
		//{
		//	var config = new MapperConfiguration(cfg => cfg
		//		.CreateMap<Entities.OrdenDeCargaFason, OrdenDeCargaFasonDto>()
		//		.ForMember(des => des.Cliente, act => act.MapFrom(src => src.Cliente.RazonSocial))
		//		.ForMember(des => des.Estado, act => act.MapFrom(src => EstadoOrdenDeCargaFasonExtensions.ToFriendlyString((EstadoOrdenDeCargaFason)src.Estado)))
		//		.ForMember(des => des.Producto, act => act.MapFrom(src => src.Producto.Nombre))
		//		);
		//	var mapper = config.CreateMapper();
		//	Response = new List<OrdenDeCargaFasonDto>();
		//	foreach (var entity in listEntity)
		//	{
		//		var dto = mapper.Map<OrdenDeCargaFasonDto>(entity);
		//		Response.Add(dto);
		//	}
		//}

        public ListarOrdenDeCargaFasonResponse()
        {
        }
    }
}
