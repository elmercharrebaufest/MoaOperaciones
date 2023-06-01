using System.Collections.Generic;
using System.Linq;
using SustitucionMOAWS.ScatoWebService;
using SustitucionMOAModel.Models.WSMapMOA.CartaPorte;
using SustitucionMOAWS.Interfaces;
using SustitucionMOAModel.Dto.OrdenDeCarga;
using SustitucionMOAWS.Logger;
using SustitucionMOAFotmatter;
using System;
using SustitucionMOAWS.Util;

namespace SustitucionMOAWS.WSConsumers
{
    public class ScatoConsumer : IScatoConsumer
    {
        private readonly IServicioRepositorio service;

        public ScatoConsumer(IServicioRepositorio service)
        {
            this.service = service;
        }

        public List<CartaPorteFoto> ObtenerFotoCartaPorte(string cartaPorteId)
        {
            return ObtenerFotoCartasPorte(new List<string> { cartaPorteId });
        }

        public List<CartaPorteFoto> ObtenerFotoCartasPorte(List<string> cartaPorteIds)
        {
            try
            {
                List<CartaPorteFoto> cartaPorteFotos = new List<CartaPorteFoto>();
                foreach (string cartaPorteId in cartaPorteIds)
                {
                    ObtenerFotosPorCartaPorteID(cartaPorteFotos, cartaPorteId);
                }

                if (cartaPorteFotos.Count == 0)
                {
                    throw new SustitucionMOAModel.CustomExceptions.InfoCustomException("No hay imagen para la/s carta/s porte seleccionada");
                }

                return cartaPorteFotos;
            }
            catch
            {
                throw;
            }
        }

        private void ObtenerFotosPorCartaPorteID(List<CartaPorteFoto> cartaPorteFotos, string cartaPorteId)
        {
            try
            {
                FotosDto fotos = service.ObtenerFotosCartaPortePorNumero(cartaPorteId.TrimStart('0'));
                foreach (FotoDto foto in fotos.Fotos)
                {
                    cartaPorteFotos.Add(new CartaPorteFoto(cartaPorteId, foto.Foto, foto.FotoChica, foto.Extension));
                }
                if (fotos.Fotos == null || fotos.Fotos.Length == 0)
                {
                    fotos = service.ObtenerFotosCartaPortePorNumero(cartaPorteId);
                    foreach (FotoDto foto in fotos.Fotos)
                    {
                        cartaPorteFotos.Add(new CartaPorteFoto(cartaPorteId, foto.Foto, foto.FotoChica, foto.Extension));
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public List<LocalidadDto> ObtenerLocalidades()
        {
            return service.ListarLocalidades().ToList();
        }

        public List<ProvinciaDto> ObtenerProvincias()
        {
            return service.ListarProvincias().ToList();
        }

        public List<KmPorProveedorDto> BuscarDestinos(string cuit)
        {
            if (!cuit.Contains("-"))
            {
                cuit = cuit.Substring(0, 2) + "-" + cuit.Substring(2, 8) + "-" + cuit.Substring(10, 1);
            }

            var cliente = service.BuscarCliente(cuit);
            if (cliente == null)
            {
                return new List<KmPorProveedorDto>();
            }
            var destinos = service.ListarKmPorProveedorYCentro(cliente.Id, 5).ToList();
            return destinos;
        }

        public bool CuilChoferExiste(string cuil, bool logger = true)
        {
            if (logger)
                Log.Info(string.Format("Validar CUIL Chofer: {0}", cuil));

            var chofer = service.ObtenerChoferPorCuit(DataFormatter.CuitConGuion(cuil));
            var result = !(chofer is null);

            if (logger)
                Log.Info(string.Format("Result Validar CUIL: {0}; Result: {1}", cuil, result ? "Existe" : "No existe"));

            return result;
        }
        public ValidarCuitExisteScatoResponse ExisteCuitDestinoDestinatario(string cuit, bool logger = true)
        {
            if (logger)
                Log.Info(string.Format("Validar Existe CUIT en SCATO: {0}", cuit));

            var listaClientes = service.ListarClientesPorCuit(DataFormatter.CuitConGuion(cuit));
            var result = ValidarCuitExisteScatoResponse.Nuevo(listaClientes.Length > 0, listaClientes.FirstOrDefault()?.Descripcion);

            if (logger)
                Log.Info(string.Format("Result Existe CUIT en SCATO: {0}; Result: {1}", cuit, result.Existe ? "Existe" : "No existe"));

            return result;
        }

        public ProveedorDto ObtenerProveedorPorCuit(string cuit)
        {
            var cuitGuiones = string.Empty;
            try
            {
                cuitGuiones = DataFormatter.CuitConGuion(cuit);
                var proveedor = service.ObtenerProveedorPorCuit(cuitGuiones, new TiposProveedor { PR = true });

                Log.Info(string.Format("ScatoConsumer.ObtenerProveedorPorCuit. cuit: {0}, cuitGuiones: {1}, proveedor: {2}",
                    cuit, cuitGuiones, proveedor.ToJson()));
                
                return proveedor;
            }
            catch (Exception ex)
            {
                Log.Error("", "", "ScatoConsumer", "ObtenerProveedorPorCuit", string.Format("cuit: {0}, cuitGuiones: {1}", cuit, cuitGuiones));
                throw ex;
            }
        }
    }

}