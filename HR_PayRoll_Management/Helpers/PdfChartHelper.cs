using HR_PayRoll_Management.ViewModels.AttendanceVM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace HR_PayRoll_Management.Helpers
{
    public static class PdfChartHelper
    {
        public static byte[] CreateAttendanceStatusChart(
     double present,
     double absent,
     double leave)
        {
            using (Bitmap bitmap = new Bitmap(500, 300))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);

                    using (Font titleFont = new Font("Arial", 18, FontStyle.Bold))
                    using (Font labelFont = new Font("Arial", 12))
                    {
                        graphics.DrawString("Attendance Status", titleFont, Brushes.Black, new Point(130, 20));

                        float total = (float)(present + absent + leave);
                        if (total == 0)
                            total = 1;

                        Rectangle chartArea = new Rectangle(50, 70, 180, 180);

                        float startAngle = 0;
                        float presentAngle = (float)(present / total * 360);
                        float absentAngle = (float)(absent / total * 360);
                        float leaveAngle = (float)(leave / total * 360);

                        graphics.FillPie(Brushes.Green, chartArea, startAngle, presentAngle);
                        startAngle += presentAngle;

                        graphics.FillPie(Brushes.Red, chartArea, startAngle, absentAngle);
                        startAngle += absentAngle;

                        graphics.FillPie(Brushes.Orange, chartArea, startAngle, leaveAngle);

                        graphics.DrawString($"Present : {Math.Round(present)}%", labelFont, Brushes.Black, new Point(280, 90));
                        graphics.DrawString($"Absent : {Math.Round(absent)}%", labelFont, Brushes.Black, new Point(280, 130));
                        graphics.DrawString($"Leave : {Math.Round(leave)}%", labelFont, Brushes.Black, new Point(280, 170));
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }

        public static byte[] CreateDepartmentChart(
    IEnumerable<DepartmentAttendanceReportVM> departments)
        {
            using (Bitmap bitmap = new Bitmap(600, 350))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);

                    using (Font titleFont = new Font("Arial", 18, FontStyle.Bold))
                    using (Font labelFont = new Font("Arial", 11))
                    {
                        graphics.DrawString(
                            "Department Attendance %",
                            titleFont,
                            Brushes.Black,
                            new Point(120, 20));

                        int startY = 80;

                        foreach (var item in departments)
                        {
                            double percentage = item.AttendancePercentage;

                            // Department Name
                            graphics.DrawString(
                                item.DepartmentName,
                                labelFont,
                                Brushes.Black,
                                new Point(30, startY));

                            // Empty Bar Background
                            graphics.FillRectangle(
                                Brushes.LightGray,
                                180,
                                startY,
                                250,
                                20);

                            // Attendance Percentage Bar
                            graphics.FillRectangle(
                                Brushes.Green,
                                180,
                                startY,
                                (float)(percentage * 2.5),
                                20);

                            // Percentage Text
                            graphics.DrawString(
                                Math.Round(percentage) + "%",
                                labelFont,
                                Brushes.Black,
                                new Point(450, startY));

                            startY += 45;
                        }
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    return stream.ToArray();
                }
            }
        }
    }
}