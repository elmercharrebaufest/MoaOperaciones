
using SustitucionMOAModel.Dto.ArchivoBoleto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IArchivoBoletoService
    {
        List<ArchivoBoletoDto> ListarArchivosBoleto(ListarReqArchivoBoletoDto request);
        Task<ArchivoBoletoDto> CrearArchivoBoleto(CrearReqArchivoBoletoDto data);
    }
}
