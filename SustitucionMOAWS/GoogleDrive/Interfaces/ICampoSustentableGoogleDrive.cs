using SustitucionMOAWS.GoogleDrive.Models;

namespace SustitucionMOAWS.GoogleDrive.Interfaces
{
    public interface ICampoSustentableGoogleDrive : IGoogleDriveHelperUtils
    {
        string UploadFile(GoogleDriveFileUploadRequest uploadFileRequest, string inputFolderId);
    }
}