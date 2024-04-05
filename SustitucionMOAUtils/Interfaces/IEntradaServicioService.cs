using DocumentFormat.OpenXml.Spreadsheet;
using SustitucionMOAModel.Dto;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAModel.Entities;
using SustitucionMOAModel.Models.ViewModel.Notificacion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SustitucionMOAWS.WSConsumers.CrearEntradaDeServicioConsumerMOA;
//C:\Users\jizaguirre\source\repos\MoaOperaciones\SustitucionMOAUtils\Interfaces\IOrderService.cs
//C:\Users\jizaguirre\source\repos\MoaOperaciones\SustitucionMOAUtils\Interfaces\IEntradaServicioService.cs

namespace SustitucionMOAUtils.Interfaces
{
    public interface IEntradaServicioService
    {
        //List<OrdenCompraDto> GetByProveedor(string vendedor);
        //List<OrdenCompraDto> ServicioOrdenesCompraFake(string vendedor);
        Task<List<EntradaServicioCabeceraDto>> ObtenerEntradasServicioCompleta(EntradaServicioParamsDto parametros);
        string BorrarEntradaServicio(EntradaServicioParamsDto parametros);
        Task<List<EntradaServicioCabeceraDto>> ServicioSAP_EntradasServicioCabecera(EntradaServicioParamsDto parametros);
        //List<EntradaServicioCabeceraDto> ObtenerEntradasServicioConDetalle(EntradaServicioParamsDto parametros);
        //Task<EntradaServicioCabeceraDto>CrearEntradaServicioAsync(EntradaServicioCreateParamsDto parametros);
        Task <List<EntradaServicioCreateRespuestaDto>> CrearEntradaServicio(List<EntradaServicioCreateParamsDto> parametros);
    }
}
