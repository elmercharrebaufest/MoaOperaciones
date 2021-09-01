namespace SustitucionMOAUtils.Interfaces.Wrappers
{
    public interface IExcelExportWrapper
    {
        string ToExcel(object dataList, string[] headers, string titulo);
    }
}