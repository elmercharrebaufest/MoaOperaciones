using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCargaFason
{
    public class DetalleOrdenDeCargaFasonResponse
    {
		public OrdenDeCargaFasonDetalleDto Response { get; set; }
		public DetalleOrdenDeCargaFasonResponse(Entities.OrdenDeCargaFason entity)
		{
			var config = new MapperConfiguration(cfg => cfg
				.CreateMap<Entities.OrdenDeCargaFason, OrdenDeCargaFasonDetalleDto>().ForMember(d =>d.Cliente, o =>o.MapFrom(s =>s.Cliente.CodigoProveedor)));
			var mapper = config.CreateMapper();
			Response = new OrdenDeCargaFasonDetalleDto();
			
				var dto = mapper.Map<OrdenDeCargaFasonDetalleDto>(entity);
			    Response = dto;
		}
	}
}
