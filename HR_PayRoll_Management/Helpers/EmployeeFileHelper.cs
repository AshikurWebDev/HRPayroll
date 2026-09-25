using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace HR_PayRoll_Management.Helpers
{
    public static class EmployeeFileHelper
    {
        private static readonly string UploadFolder = "~/Uploads/Employees/Documents/";

        private static readonly List<string> AllowedExtensions =
            new List<string>()
            {
                ".pdf",
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".txt",
                ".jpg",
                ".jpeg",
                ".png"
            };

        public static string GenerateFileName(int employeeId, string extension)
        {
            string year = DateTime.Now.ToString("yyyy");
            string date = DateTime.Now.ToString("MMdd");
            return $"EMP{employeeId}_{year}_{date}{extension}";
        }

        private const int MaxFileSize = 5 * 1024 * 1024;

        public static void Validate(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                throw new Exception("Please select a file.");
            }

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (!AllowedExtensions.Contains(extension))
            {
                throw new Exception("File type is not allowed.");
            }

            if (file.ContentLength > MaxFileSize)
            {
                throw new Exception("File size cannot exceed 5MB.");
            }
        }

        public static string Upload(HttpPostedFileBase file, int employeeId)
        {
            Validate(file);

            string extension = Path.GetExtension(file.FileName).ToLower();

            string folderPath = HttpContext.Current.Server.MapPath(UploadFolder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string newFileName = GenerateFileName(employeeId, extension);

            string fullPath = Path.Combine(folderPath, newFileName);

            file.SaveAs(fullPath);
            return UploadFolder.Replace("~", "") + newFileName;
        }

        public static void Delete(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            string physicalPath = HttpContext.Current.Server.MapPath("~" + filePath);
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }


    }
}