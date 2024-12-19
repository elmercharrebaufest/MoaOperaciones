using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Interfaces;
using SustitucionMOAWS.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SustitucionMOAUtils.Services
{
    public class RegistroInfoService : IRegistroInfoService
    {
        private readonly IObtenerRegistroInfoConsumerMOA obtenerRegistroInfoConsumerMOA;

        public RegistroInfoService(IObtenerRegistroInfoConsumerMOA obtenerRegistroInfoConsumerMOA)
        {
            this.obtenerRegistroInfoConsumerMOA = obtenerRegistroInfoConsumerMOA;
        }

        public List<RegistroInfoDto> ObtenerRegistroInfoConsumer(string material, string centro, string organizacionDeCompras, string proveedor)
        {
            return obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(material, centro, organizacionDeCompras, proveedor);
        }

        public RegistroInfoDto ObtenerUltimoRegistroPorMaterialYProveedor(string material, string centro, string grupoDeCompras, string proveedor = "")
        {
            RegistroInfoDto ultimoRegistro = new RegistroInfoDto();
            ultimoRegistro.EsModificar = false;
            IOrderedEnumerable<RegistroInfoDto> registros = obtenerRegistroInfoConsumerMOA.ObtenerRegistroInfoConsumer(material, centro, grupoDeCompras, proveedor)
                               /*.Where(x => x.NumeroOrdenDeCompra != null)*/.OrderByDescending(x => x.FechaUltimaCompra);
            if (registros.Any())
            {
                ultimoRegistro = registros.First();
                ultimoRegistro.EsModificar = true;
            }
            return ultimoRegistro;
        }
    }
}
