using SustitucionMOAUtils.Export;
using SustitucionMOAUtils.Interfaces.Wrappers;

namespace SustitucionMOAUtils.Wrappers
{
    public class FileWrapper : IFileWrapper
    {
        public byte[] ReadAllBytes(string path)
        {
            return System.IO.File.ReadAllBytes(path);
        }
    }
}