using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using System.IO;
using System.Web;

namespace HR_PayRoll_Management.Helpers
{
    public static class ImageCompressionHelper
    {
        public static MemoryStream CompressImage(
            HttpPostedFileBase file)
        {

            using (var inputStream = new MemoryStream())
            {

                file.InputStream.CopyTo(inputStream); 


                inputStream.Position = 0;


                using (var factory = new ImageFactory())
                {

                    factory.Load(inputStream);



                    Size newSize =
                        CalculateNewSize(
                            factory.Image.Width,
                            factory.Image.Height
                        );



                    factory.Resize(newSize);



                    factory.Format(
                        new JpegFormat()
                    );


                    factory.Quality(75);



                    var outputStream =
                        new MemoryStream();



                    factory.Save(outputStream);



                    outputStream.Position = 0;


                    return outputStream;

                }

            }

        }



        private static Size CalculateNewSize(
            int width,
            int height)
        {

            int maxWidth = 600;

            int maxHeight = 600;



            double ratio =
                System.Math.Min(
                    (double)maxWidth / width,
                    (double)maxHeight / height
                );



            if (ratio > 1)
            {
                ratio = 1;
            }



            return new Size
            (
                (int)(width * ratio),
                (int)(height * ratio)
            );

        }

    }
}