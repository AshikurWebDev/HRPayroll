using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Drawing;
using System.IO;
using System.Web;

namespace HR_PayRoll_Management.Helpers
{
    public static class EmployeeImageHelper
    {
        private const int MaxWidth = 600;
        private const int MaxHeight = 600;
        private const int MaxFileSize = 100 * 1024;

        public static string Upload(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            string extension = Path.GetExtension(file.FileName).ToLower();

            if (extension != ".jpg" &&
                extension != ".jpeg" &&
                extension != ".png")
            {
                throw new Exception("Only jpg and png images are allowed.");
            }

            using (var stream = new MemoryStream())
            {
                file.InputStream.CopyTo(stream);
                stream.Position = 0;
                byte[] imageBytes = CompressImage(stream);

                string folder = "~/Uploads/Employees/";

                string physicalFolder =
                    HttpContext.Current.Server.MapPath(folder);

                if (!Directory.Exists(physicalFolder))
                    Directory.CreateDirectory(physicalFolder);

                string fileName = Guid.NewGuid() + ".jpg";

                string fullPath = Path.Combine(physicalFolder, fileName);

                File.WriteAllBytes(fullPath, imageBytes);

                return folder + fileName;
            }
        }


        private static byte[] CompressImage(MemoryStream input)
        {
            int quality = 80;
            byte[] result;

            using (var imageFactory = new ImageFactory())
            {
                imageFactory.Load(input);

                Size size = CalculateSize(
                    imageFactory.Image.Width,
                    imageFactory.Image.Height
                );

                imageFactory.Resize(size);
                imageFactory.Format(new JpegFormat());

                do
                {
                    using (var output = new MemoryStream())
                    {
                        imageFactory.Quality(quality);
                        imageFactory.Save(output);

                        result = output.ToArray();
                    }

                    quality -= 10;

                } while (result.Length > MaxFileSize && quality > 10);
            }

            return result;
        }


        private static Size CalculateSize(int width, int height)
        {
            double ratio = Math.Min(
                (double)MaxWidth / width,
                (double)MaxHeight / height
            );

            if (ratio > 1)
                ratio = 1;

            return new Size(
                (int)(width * ratio),
                (int)(height * ratio)
            );
        }


        public static void Delete(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            string fullPath = HttpContext.Current.Server.MapPath(path);

            if (File.Exists(fullPath)){
                File.Delete(fullPath);
            }
        }
    }
}