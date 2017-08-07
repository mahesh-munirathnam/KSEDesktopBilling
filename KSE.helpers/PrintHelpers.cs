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
using System.IO;

namespace KSE.helpers
{
    public static class PrintHelpers
    {
        //Standard A4 size Dimensions
        static double A4Width = XUnit.FromCentimeter(21).Point;
        static double A4Height = XUnit.FromCentimeter(29.7).Point;

        //setup before genrating the pdf
        public static void PrintA4Bill(BillViewModel bill)
        {
            //get the directort path to save the bills
            string billDirPath = ConfigurationSettings.AppSettings["DIRPath"].ToString();
            string billfolderPath = billDirPath + "\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");
            //Check if the application folder exists to store the bills
            CreateDirectory(billDirPath);

            //folder exists, now try to create today's folder
            CreateDirectory(billfolderPath);

            //get count of files in today directory
            int? filecount = GetFileCountInDirectory(billfolderPath);

            if (filecount.HasValue)
            {
                DateTime now = DateTime.Now;
                string filename = "Invoice_" + filecount + ".pdf";
                PdfDocument document = new PdfDocument();
                document.Info.Title = filename;
                document.Info.Author = "KSE";
                document.Info.Subject = "Tax Invoice";

                //Generate Bill in PDF format for Print
                GeneratePDFBill(document, bill);

                // Save the document...
                document.Save(billfolderPath + "\\" + filename);
                Process.Start(billfolderPath + "\\" + filename);
            }

        }

        //Generate the pdf in format
        static void GeneratePDFBill(PdfDocument document, BillViewModel b)
        {
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;

            XFont Bigfont = new XFont("Verdana", 10, XFontStyle.Bold);
            XFont Midfont = new XFont("Verdana", 8, XFontStyle.Regular);
            XFont Smallfont = new XFont("Verdana", 7, XFontStyle.Regular);
            XPen pen = new XPen(XColors.Black, 0.5);
            XPen Dottedpen = new XPen(XColors.Black, 0.5);
            Dottedpen.DashStyle = XDashStyle.Dash;
            //Shop name and Tin Line
            gfx.DrawLine(Dottedpen, 0, 25, page.Width, 25);
            //address and phone number line
            gfx.DrawLine(Dottedpen, 0, 65, page.Width, 65);
            //Invoice line
            gfx.DrawLine(Dottedpen, 0, 110, page.Width, 110);
            #region Bill Header
            //Bill Header
            gfx.DrawString(ConfigurationSettings.AppSettings["ShopName"].ToString(), Bigfont, XBrushes.Black,
              new XRect(50, 5, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("GST TIN : " + ConfigurationSettings.AppSettings["GSTNO"].ToString(), Midfont, XBrushes.Black,
            new XRect(300, 10, page.Width - 200, 15), XStringFormats.TopCenter);

            gfx.DrawString(ConfigurationSettings.AppSettings["ShopSubtitle"].ToString(), Smallfont, XBrushes.Black,
              new XRect(200, 9, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("Manufacturers and retailers of Silk Sarees and textiles", Midfont, XBrushes.Black,
              new XRect(50, 30, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("#179/12-01, Farah Point, 2nd cross Lalbagh", Midfont, XBrushes.Black,
            new XRect(50, 40, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("Hosur Road, Opp Hindustan Marbles &Granite, Bangalore - 560027", Midfont, XBrushes.Black,
            new XRect(50, 50, page.Width - 200, 15), XStringFormats.TopLeft);


            #endregion

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Section BillSec = doc.AddSection();

            #region Invoce Table

            //Invoice Table
            Table InvoiceTable = BillSec.AddTable();
            InvoiceTable.Style = "Table";
            InvoiceTable.Borders.Color = Colors.Black;

            //Adding three columns 
            //1.Invoice Date
            Column iColumn = InvoiceTable.AddColumn("5cm");
            iColumn.Format.Alignment = ParagraphAlignment.Center;
            //2. Invoice Number
            iColumn = InvoiceTable.AddColumn("5cm");
            iColumn.Format.Alignment = ParagraphAlignment.Center;
            //3. Payment Terms
            iColumn = InvoiceTable.AddColumn("5cm");
            iColumn.Format.Alignment = ParagraphAlignment.Center;

            Row iRow = InvoiceTable.AddRow();
            iRow.HeadingFormat = true;
            iRow.Format.Alignment = ParagraphAlignment.Center;

            //1st row  Heading
            iRow.Cells[0].AddParagraph("Invoice Date");
            iRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            iRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            iRow.Cells[1].AddParagraph("Invoice Number");
            iRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            iRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
            iRow.Cells[2].AddParagraph("Payment Terms");
            iRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            iRow.Cells[2].VerticalAlignment = VerticalAlignment.Center;

            //2nd row Data
            iRow = InvoiceTable.AddRow();
            iRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            iRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            iRow.Cells[0].AddParagraph(DateTime.Now.Date.ToShortDateString());

            iRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            iRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
            iRow.Cells[1].AddParagraph(GetInvoiceNo());

            iRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            iRow.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            iRow.Cells[2].AddParagraph("Cash");
            #endregion
            
            #region Bill Table

            // Create the item table
            Table BillTable = BillSec.AddTable();
            BillTable.Style = "Table";
            BillTable.Borders.Color = Colors.Black;

            // Before you can add a row, you must define the columns
            //description column
            Column bColumn = BillTable.AddColumn("2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Center;

            //quantity
            bColumn = BillTable.AddColumn("2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;

            //rate per item column
            bColumn = BillTable.AddColumn("2.2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;

            //Amount for item
            bColumn = BillTable.AddColumn("2.2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //Discount
            bColumn = BillTable.AddColumn("2.2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //taxable amount
            bColumn = BillTable.AddColumn("2.2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //CGST
            bColumn = BillTable.AddColumn("2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //SGST
            bColumn = BillTable.AddColumn("2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //TOTAl
            bColumn = BillTable.AddColumn("2cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            Row bRow = BillTable.AddRow();
            bRow.HeadingFormat = true;
            bRow.Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[0].AddParagraph("Item");
            bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[1].AddParagraph("Quantity");
            bRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[2].AddParagraph("Unit Price(₹)");
            bRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[3].AddParagraph("Amount(₹)");
            bRow.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[3].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[4].AddParagraph("Discount(%)");
            bRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[4].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[5].AddParagraph("Taxable Amount(₹)");
            bRow.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[5].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[6].AddParagraph("CGST(₹)");
            bRow.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[6].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[7].AddParagraph("SGST(₹)");
            bRow.Cells[7].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[7].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[8].AddParagraph("Total(₹)");
            bRow.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[8].VerticalAlignment = VerticalAlignment.Center;

            foreach (var i in b.BillItems)
            {
                bRow = BillTable.AddRow();
                bRow.Cells[0].AddParagraph(i.Name);
                bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[1].AddParagraph(Convert.ToString(i.Quantity));
                bRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[2].AddParagraph(i.Price.ToString("0.##"));
                bRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[3].AddParagraph(i.Amount.ToString("0.##"));
                bRow.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[4].AddParagraph(Convert.ToString(i.DiscountRate));
                bRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[5].AddParagraph(i.TaxableAmount.ToString("0.##"));
                bRow.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[6].AddParagraph(i.TaxAmount.ToString("0.##"));
                bRow.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[7].AddParagraph(i.TaxAmount.ToString("0.##"));
                bRow.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                bRow.Cells[8].AddParagraph(i.Total.ToString("0.##"));
                bRow.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            }

            BillTable.SetEdge(0, 0, 4, b.getBillItemsCount(), Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            #endregion

            #region Document Render
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(3.5), XUnit.FromCentimeter(2.6), "17cm", InvoiceTable);
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(2), XUnit.FromCentimeter(4.5), "20cm", BillTable);
            #endregion
        }

        static bool CreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                return true;
            }
            return false;
        }

        static int? GetFileCountInDirectory(string DirPath)
        {
            if (Directory.Exists(DirPath))
            {
                return Directory.GetFiles(DirPath).Length;
            }
            return null;
        }

        static string GetInvoiceNo()
        {
            return "123456789";
        }
    }
}
