using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HR_PayRoll_Management.Helpers
{
    public static class PdfHelper
    {


        public static Font TitleFont()
        {
            return FontFactory.GetFont(
                FontFactory.HELVETICA_BOLD,
                18,
                BaseColor.BLACK);
        }



        public static Font HeadingFont()
        {
            return FontFactory.GetFont(
                FontFactory.HELVETICA_BOLD,
                13,
                BaseColor.BLACK);
        }



        public static void AddCell(
            PdfPTable table,
            string text)
        {

            PdfPCell cell =
                new PdfPCell(
                    new Phrase(text));


            cell.Padding = 5;


            table.AddCell(cell);

        }




        public static void AddHeaderCell(
            PdfPTable table,
            string text)
        {


            PdfPCell cell =
                new PdfPCell(
                    new Phrase(text));


            cell.HorizontalAlignment =
                Element.ALIGN_CENTER;


            cell.Padding = 5;


            cell.BackgroundColor =
                BaseColor.LIGHT_GRAY;


            table.AddCell(cell);

        }


    }
}