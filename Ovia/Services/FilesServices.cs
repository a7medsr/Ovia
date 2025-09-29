using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing.Imaging;


namespace Ovia.Services
{
    public class FilesServices
    {
        public byte[] GeneratePdfReport(IEnumerable<dynamic> reportData)
        {
            // Define page size and margins
            PdfDocument document = new PdfDocument();
            XUnit pageWidth = XUnit.FromInch(8.5);
            XUnit pageHeight = XUnit.FromInch(11);
            XUnit margin = XUnit.FromInch(0.5);

            // Calculate available width for table columns
            double availableWidth = pageWidth.Point - 2 * margin.Point;

            // Define table column widths
            double[] columnWidths = { availableWidth * 0.2, availableWidth * 0.1, availableWidth * 0.2, availableWidth * 0.1, availableWidth * 0.2, availableWidth * 0.2 };

            // Split reportData into chunks to fit on pages
            int itemsPerPage = 35; // Adjust as needed
            var chunks = reportData.Chunk(itemsPerPage);

            foreach (var chunk in chunks)
            {
                // Create new PDF document
                var page = document.AddPage();
                page.Width = pageWidth;
                page.Height = pageHeight;

                // Get an XGraphics object for drawing on the page
                var gfx = XGraphics.FromPdfPage(page);

                // Create a font that supports Arabic characters
                var font = new XFont("Arial Unicode MS", 12);
                var font1 = new XFont("Arial Unicode MS", 10);

                // Draw the table headers
                DrawTableHeader(gfx, font, columnWidths, margin, pageHeight);

                // Draw the data as a table
                int y = (int)margin.Point + 50; // Start below the headers
                foreach (var item in chunk)
                {
                    DrawTableRow(gfx, font1, item, columnWidths, margin, ref y);
                }
            }

            // Save the document to a MemoryStream
            var stream = new MemoryStream();
            document.Save(stream, false);
            stream.Position = 0;

            // Return the MemoryStream as a byte array
            return stream.ToArray();
        }

        private void DrawTableHeader(XGraphics gfx, XFont font, double[] columnWidths, XUnit margin, XUnit pageHeight)
        {
            string[] headers = { "Customer Name", "Back Office", "Package Name", "Price", "Invoice Serial", "Creation Date" };

            double x = margin.Point ;
            double y = margin.Point + 10; // Start below the top margin
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawString(headers[i], font, XBrushes.Black, new XRect(x, y, columnWidths[i], 20), XStringFormats.TopLeft);
                x += columnWidths[i];
            }
        }

        private void DrawTableRow(XGraphics gfx, XFont font, dynamic item, double[] columnWidths, XUnit margin, ref int y)
        {
            string[] data = { item.NameEn, item.ReferId.ToString(), item.Name, item.Price.ToString(), item.InvoiceSerial.ToString(), item.CreationDate.ToString() };

            double x = margin.Point;
            for (int i = 0; i < data.Length; i++)
            {
                gfx.DrawString(data[i], font, XBrushes.Black, new XRect(x, y, columnWidths[i], 20), XStringFormats.TopLeft);
                x += columnWidths[i];
            }

            y += 20; // Move to the next row
        }
   
    
    
    
    }

    // Extension method to chunk the report data into smaller lists
    public static class EnumerableExtensions
    {
        public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize).ToList();
                source = source.Skip(chunkSize);
            }
        }
    }
}
