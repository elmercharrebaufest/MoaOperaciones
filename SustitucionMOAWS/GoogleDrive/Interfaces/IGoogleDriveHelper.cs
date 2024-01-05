using Google.Apis.Drive.v3.Data;
using System.Collections.Generic;
using SustitucionMOAWS.GoogleDrive.Models;

namespace SustitucionMOAWS.GoogleDrive.Interfaces
{
    public interface IGoogleDriveHelper
    {

        /// <summary>
        /// Automatic download a file from drive (if found), to the specified path.
        /// </summary>
        /// <param name="downloadFileRequest"></param>
        /// <exception cref="FileNotFoundException"></exception>
        void DownloadFile(GoogleDriveFileDownloadRequest downloadFileRequest);

        /// <summary>
        /// Return list of files, with the option to filter with a personal query and select specific fields for the files
        /// </summary>
        /// <param name="driveService"></param>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        IList<File> GetFiles(string query = null, string fields = "nextPageToken, files(id, name)");

        /// <summary>
        /// Get files inside a specific folder.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        IList<File> GetFolderFiles(string folderId);

        /// <summary>
        /// Allows you to get the Folder ID of the first folder with the name passed by.
        /// </summary>
        /// <param name="driveService"></param>
        /// <param name="folderName"></param>
        /// <returns>The possible FolderId</returns>
        string GetFolderIdByName(string folderName);

        /// <summary>
        /// Method that allows you to upload files to the drive application connected with the driveService in the parameter. Returns the FileId.
        /// </summary>
        /// <param name="uploadFileRequest"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        string UploadFile(GoogleDriveFileUploadRequest uploadFileRequest);
    }
}
