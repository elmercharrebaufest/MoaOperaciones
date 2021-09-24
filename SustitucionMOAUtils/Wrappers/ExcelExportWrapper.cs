using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces.Wrappers;

namespace SustitucionMOAUtils.Wrappers
{
    public class ExcelExportWrapper : IExcelExportWrapper
    {
        public string ToExcel(object dataList, string[] headers, string titulo)
        {
            return ExcelExport.ToExcel(dataList, headers, titulo);
        }
    }
}