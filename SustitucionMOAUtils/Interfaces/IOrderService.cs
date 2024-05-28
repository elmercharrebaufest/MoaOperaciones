using SustitucionMOAModel.Consultas;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.ViewModel.Notificacion;
using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IOrderService
    {
        //List<OrdenCompraDto> GetByProveedor(string vendedor);
        //List<OrdenCompraDto> ServicioOrdenesCompraFake(string vendedor);
        ListaPaginada<DetalleOrdenDeCompraDto> ObtenerOrdenesCompraConDetalle(OrderParamsDto parametros);

    }
}
