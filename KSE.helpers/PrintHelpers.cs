using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using KSE.ViewModels;

namespace KSE.helpers
{
    public static class PrintHelpers
    {
        static double A4Width = XUnit.FromCentimeter(21).Point;
        static double A4Height = XUnit.FromCentimeter(29.7).Point;

        public static void HelloPdf(BillViewModel b)
        {
            DateTime now = DateTime.Now;
            string filename = "MixMigraDocAndPdfSharp.pdf";
            filename = "D:\\"+Guid.NewGuid().ToString("D").ToUpper() + ".pdf";
            PdfDocument document = new PdfDocument();
            document.Info.Title = "PDFsharp XGraphic Sample";
            document.Info.Author = "Stefan Lange";
            document.Info.Subject = "Created with code snippets that show the use of graphical functions";
            document.Info.Keywords = "PDFsharp, XGraphics";

            GenerateBillPDF(document,b);

            Debug.WriteLine("seconds=" + (DateTime.Now - now).TotalSeconds.ToString());

            // Save the document...
            document.Save(filename);
            Process.Start(filename);
        }

        static void GenerateBillPDF(PdfDocument document,BillViewModel b)
        {
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            // HACK²
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;

            XFont Bigfont = new XFont("Verdana", 15, XFontStyle.Bold);
            XFont Midfont = new XFont("Verdana", 10, XFontStyle.Bold);
            XFont Smallfont = new XFont("Verdana", 8, XFontStyle.Bold);

            gfx.DrawString(ConfigurationSettings.AppSettings["ShopName"].ToString(), Bigfont, XBrushes.Black,
              new XRect(100, 10, page.Width - 200, 15), XStringFormats.Center);

            gfx.DrawString(ConfigurationSettings.AppSettings["ShopSubtitle"].ToString(), Smallfont, XBrushes.Black,
              new XRect(100, 20, page.Width - 200, 15), XStringFormats.Center);

            gfx.DrawString("Manufacturers and retailers of Silk Sarees and textiles", Midfont, XBrushes.Black,
              new XRect(100, 30, page.Width - 200, 15), XStringFormats.Center);
            
            gfx.DrawString("#179/12-01, Farah Point, 2nd cross Lalbagh", Midfont, XBrushes.Black,
            new XRect(100, 45, page.Width - 200, 15), XStringFormats.Center);

            gfx.DrawString("Hosur Road, Opp Hindustan Marbles &Granite, Bangalore - 560027", Midfont, XBrushes.Black,
            new XRect(100, 55, page.Width - 200, 15), XStringFormats.Center);

            gfx.DrawString("GST TIN : " + ConfigurationSettings.AppSettings["GSTNO"].ToString(), Midfont, XBrushes.Black,
            new XRect(100, 65, page.Width - 200, 15), XStringFormats.Center);

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Section BillSec = doc.AddSection();

            // Create the item table
            Table table = BillSec.AddTable();
            table.Style = "Table";
            table.Borders.Color = Colors.Black ;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            //description column
            Column column = table.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            //quantity
            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //rate per item column
            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            //Total Amount for item
            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Cells[0].AddParagraph("Description");
            row.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[1].AddParagraph("Quantity");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].AddParagraph("Unit Price");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[3].AddParagraph("Total Price");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            row.Cells[3].VerticalAlignment = VerticalAlignment.Center;
            
            foreach(var i in b.BillItems)
            {
                row = table.AddRow();
                row.HeadingFormat = true;
                row.Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].AddParagraph(i.Name);
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].AddParagraph(Convert.ToString(i.Quantity));
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[2].AddParagraph(Convert.ToString(i.Price));
                row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[3].AddParagraph(Convert.ToString(i.Total));
                row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            }

            table.SetEdge(0, 0, 4, b.getBillItemsCount(), Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(4), XUnit.FromCentimeter(3), "17cm", table);
        }


        /// <summary>
        /// Creates an absolutely minimalistic document.
        /// </summary>
        static Document CreateDocument()
        {
            // Create a new MigraDoc document
            Document document = new Document();

            // Add a section to the document
            Section section = document.AddSection();

            // Add a paragraph to the section
            Paragraph paragraph = section.AddParagraph();

            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);

            // Add some text to the paragraph
            paragraph.AddFormattedText("Hello, World!", TextFormat.Bold);

            return document;
        }

        static XRect GetRect(int index)
        {
            XRect rect = new XRect(0, 0, A4Width / 3 * 0.9, A4Height / 3 * 0.9);
            rect.X = (index % 3) * A4Width / 3 + A4Width * 0.05 / 3;
            rect.Y = (index / 3) * A4Height / 3 + A4Height * 0.05 / 3;
            return rect;
        }
    }
}
