using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Configuration;
using System.Diagnostics;
using KSE.ViewModels;
using System.IO;

namespace KSE.helpers
{
    public static class PrintHelpers
    {
        //Standard A4 size Dimensions
        static double A5Height = XUnit.FromCentimeter(21).Point;
        static double A5Width = XUnit.FromCentimeter(15).Point;

        //setup before genrating the pdf
        public static string PrintA4Bill(BillViewModel bill)
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
                GeneratePDFBill(document, bill, "Customer Copy");
                GeneratePDFBill(document, bill, "Shop Copy");

                // Save the document...
                document.Save(billfolderPath + "\\" + filename);
                //Process.Start(billfolderPath + "\\" + filename);
                return billfolderPath + "\\" + filename;
            }
            return string.Empty;
        }

        //Generate the pdf in format
        static void GeneratePDFBill(PdfDocument document, BillViewModel b, string PDFType)
        {
            PdfPage page = document.AddPage();
            page.Width = A5Width;
            page.Height = A5Height;
            XGraphics gfx = XGraphics.FromPdfPage(page);

            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;

            XFont Bigfont = new XFont("Verdana", 8, XFontStyle.Bold);
            XFont Midfont = new XFont("Verdana", 7, XFontStyle.Regular);
            XFont Smallfont = new XFont("Verdana", 6, XFontStyle.Regular);
            XPen pen = new XPen(XColors.Black, 0.5);
            XPen Dottedpen = new XPen(XColors.Black, 0.5);
            Dottedpen.DashStyle = XDashStyle.Dash;
            //Shop name and Tin Line
            gfx.DrawLine(Dottedpen, 0, 27, page.Width, 27);
            //address and phone number line
            gfx.DrawLine(Dottedpen, 0, 60, page.Width, 60);
            //Invoice line
            gfx.DrawLine(Dottedpen, 0, 100, page.Width, 100);
            //Terms and condition line
            gfx.DrawLine(Dottedpen, 0, page.Height - 50, page.Width, page.Height - 50);
            #region Bill Header
            //Bill Header
            gfx.DrawString(ConfigurationSettings.AppSettings["ShopName"].ToString() + " " + ConfigurationSettings.AppSettings["ShopSubtitle"].ToString(), Bigfont, XBrushes.Black,
              new XRect(5, 5, 100, 10), XStringFormats.TopLeft);

            gfx.DrawString("GSTNO: " + ConfigurationSettings.AppSettings["GSTNO"].ToString(), Midfont, XBrushes.Black,
              new XRect(page.Width - 100, 16, 100, 10), XStringFormats.TopLeft);

            gfx.DrawString("(" + PDFType + ")", Midfont, XBrushes.Black,
              new XRect(page.Width - 100, 5, 100, 10), XStringFormats.TopLeft);

            gfx.DrawString("Manufacturers and retailers of Silk Sarees and textiles", Midfont, XBrushes.Black,
              new XRect(5, 16, page.Width - 200, 10), XStringFormats.TopLeft);

            gfx.DrawString(ConfigurationSettings.AppSettings["Addr1"].ToString(), Midfont, XBrushes.Black,
            new XRect(5, 30, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString(ConfigurationSettings.AppSettings["Addr2"].ToString(), Midfont, XBrushes.Black,
            new XRect(5, 40, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("Contact : " + ConfigurationSettings.AppSettings["ContactNo"].ToString(), Midfont, XBrushes.Black,
            new XRect(5, 50, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("*Terms and Conditions:", Midfont, XBrushes.Black,
            new XRect(5, page.Height - 40, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString(ConfigurationSettings.AppSettings["Terms"].ToString(), Midfont, XBrushes.Black,
            new XRect(5, page.Height - 32, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("Authorized Signature: ", Bigfont, XBrushes.Black,
            new XRect(page.Width - 120, page.Height - 40, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("For " + ConfigurationSettings.AppSettings["ShopName"].ToString(), Bigfont, XBrushes.Black,
            new XRect(page.Width - 120, page.Height - 20, page.Width - 200, 15), XStringFormats.TopLeft);

            gfx.DrawString("Generated On: " + System.DateTime.Now.ToString(), Smallfont, XBrushes.Black,
            new XRect(page.Width / 3, page.Height - 10, page.Width - 200, 15), XStringFormats.TopLeft);
            #endregion

            // You always need a MigraDoc document for rendering.
            Document doc = new Document();
            Section BillSec = doc.AddSection();

            #region Invoce Table

            //Invoice Table
            Table InvoiceTable = BillSec.AddTable();
            InvoiceTable.Style = "Table";
            InvoiceTable.Borders.Color = Colors.Black;
            InvoiceTable.Format.Font.Size = 7;
            //Adding three columns 
            //1.Invoice Date
            Column iColumn = InvoiceTable.AddColumn("2cm");
            iColumn.Format.Alignment = ParagraphAlignment.Center;
            //2. Invoice Number
            iColumn = InvoiceTable.AddColumn("3cm");
            iColumn.Format.Alignment = ParagraphAlignment.Center;
            //3. Payment Terms
            iColumn = InvoiceTable.AddColumn("2cm");
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
            iRow.Cells[2].AddParagraph(b.paymentMode);
            #endregion

            #region Bill Table

            // Create the item table
            Table BillTable = BillSec.AddTable();
            BillTable.Style = "Table";
            BillTable.Borders.Color = Colors.Black;
            BillTable.Format.Font.Size = 5;
            // Before you can add a row, you must define the columns

            //1.Item name
            Column bColumn = BillTable.AddColumn("1.5cm");
            bColumn.Format.Alignment = ParagraphAlignment.Center;
            //2.Quantity
            bColumn = BillTable.AddColumn("1cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //3.Unit Price
            bColumn = BillTable.AddColumn("1.4cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //4.Amount
            bColumn = BillTable.AddColumn("1.4cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //5.Discount (%)
            bColumn = BillTable.AddColumn("1.4cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //6.Taxable Amount
            bColumn = BillTable.AddColumn("1.4cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //7.CGST Rate
            bColumn = BillTable.AddColumn("1cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //8.CGST Amount
            bColumn = BillTable.AddColumn("1.4cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //9.SGST Rate
            bColumn = BillTable.AddColumn("1cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //10.SGST Amount
            bColumn = BillTable.AddColumn("1.4cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;
            //11.TOTAl
            bColumn = BillTable.AddColumn("1.5cm");
            bColumn.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            Row bRow = BillTable.AddRow();
            bRow.HeadingFormat = true;
            bRow.Format.Alignment = ParagraphAlignment.Center;
            bRow.Format.Font.Size = 7;
            //Item Header
            bRow.Cells[0].AddParagraph("Item");
            bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[0].MergeDown = 1;
            //Quantity Header
            bRow.Cells[1].AddParagraph("Quantity");
            bRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[1].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[1].MergeDown = 1;
            //Price Header
            bRow.Cells[2].AddParagraph("Unit Price(₹)");
            bRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[2].MergeDown = 1;
            //Amount Header
            bRow.Cells[3].AddParagraph("Amount(₹)");
            bRow.Cells[3].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[3].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[3].MergeDown = 1;
            //Discount Header
            bRow.Cells[4].AddParagraph("Discount(%)");
            bRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[4].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[4].MergeDown = 1;
            //Taxable Amount
            bRow.Cells[5].AddParagraph("Taxable Amount(₹)");
            bRow.Cells[5].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[5].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[5].MergeDown = 1;
            //CGST Header
            bRow.Cells[6].AddParagraph("CGST");
            bRow.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[6].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[6].MergeRight = 1;
            //SGST Header
            bRow.Cells[8].AddParagraph("SGST");
            bRow.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[8].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[8].MergeRight = 1;
            //Total Header
            bRow.Cells[10].AddParagraph("Total(₹)");
            bRow.Cells[10].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[10].VerticalAlignment = VerticalAlignment.Center;
            bRow.Cells[10].MergeDown = 1;

            bRow = BillTable.AddRow();
            bRow.HeadingFormat = true;
            bRow.Format.Alignment = ParagraphAlignment.Center;
            bRow.Format.Font.Size = 6;
            //CSGT Rate
            bRow.Cells[6].AddParagraph("Rate(%)");
            bRow.Cells[6].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[6].VerticalAlignment = VerticalAlignment.Center;
            //CGST Amount
            bRow.Cells[7].AddParagraph("Amount(₹)");
            bRow.Cells[7].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[7].VerticalAlignment = VerticalAlignment.Center;
            //SGST Rate
            bRow.Cells[8].AddParagraph("Rate(%)");
            bRow.Cells[8].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[8].VerticalAlignment = VerticalAlignment.Center;
            //SGST Amount
            bRow.Cells[9].AddParagraph("Amount(₹)");
            bRow.Cells[9].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[9].VerticalAlignment = VerticalAlignment.Center;

            foreach (var i in b.BillItems)
            {
                bRow = BillTable.AddRow();
                bRow.Format.Alignment = ParagraphAlignment.Center;
                bRow.Format.Font.Size = 6.5;
                //Item Name
                bRow.Cells[0].AddParagraph(i.Name);
                bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
                //Quantity
                bRow.Cells[1].AddParagraph(Convert.ToString(i.Quantity));
                bRow.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                //Unit Price
                bRow.Cells[2].AddParagraph(i.Price.ToString("0.##"));
                bRow.Cells[2].Format.Alignment = ParagraphAlignment.Center;
                //Amount
                bRow.Cells[3].AddParagraph(i.Amount.ToString("0.##"));
                bRow.Cells[3].Format.Alignment = ParagraphAlignment.Center;
                //Discount rate %
                bRow.Cells[4].AddParagraph(Convert.ToString(i.DiscountRate));
                bRow.Cells[4].Format.Alignment = ParagraphAlignment.Center;
                //Taxable Amount
                bRow.Cells[5].AddParagraph(i.TaxableAmount.ToString("0.##"));
                bRow.Cells[5].Format.Alignment = ParagraphAlignment.Center;
                //CSGT Rate
                bRow.Cells[6].AddParagraph(i.TaxRateString);
                bRow.Cells[6].Format.Alignment = ParagraphAlignment.Center;
                //CGST Amount
                bRow.Cells[7].AddParagraph(i.TaxAmount.ToString("0.##"));
                bRow.Cells[7].Format.Alignment = ParagraphAlignment.Center;
                //SGST Rate
                bRow.Cells[8].AddParagraph(i.TaxRateString);
                bRow.Cells[8].Format.Alignment = ParagraphAlignment.Center;
                //SGST Amount
                bRow.Cells[9].AddParagraph(i.TaxAmount.ToString("0.##"));
                bRow.Cells[9].Format.Alignment = ParagraphAlignment.Center;
                //Item Total
                bRow.Cells[10].AddParagraph(i.Total.ToString("0.##"));
                bRow.Cells[10].Format.Alignment = ParagraphAlignment.Center;
            }

            //Print tht Bill total Details
            bRow = BillTable.AddRow();
            bRow.Format.Alignment = ParagraphAlignment.Center;
            bRow.Format.Font.Size = 6.5;

            bRow.Cells[0].AddParagraph("Bill Amount (₹)");
            bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[0].MergeRight = 8;
            bRow.Cells[9].AddParagraph(b.BillAmount.ToString());
            bRow.Cells[9].MergeRight = 1;

            //Tax Total
            bRow = BillTable.AddRow();
            bRow.Format.Alignment = ParagraphAlignment.Center;
            bRow.Format.Font.Size = 6.5;

            bRow.Cells[0].AddParagraph("CGST Total (₹)");
            bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[0].MergeRight = 8;
            bRow.Cells[9].AddParagraph(b.tax.ToString());
            bRow.Cells[9].MergeRight = 1;

            //Tax Total
            bRow = BillTable.AddRow();
            bRow.Format.Alignment = ParagraphAlignment.Center;
            bRow.Format.Font.Size = 6.5;

            bRow.Cells[0].AddParagraph("SGST Total (₹)");
            bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[0].MergeRight = 8;
            bRow.Cells[9].AddParagraph(b.tax.ToString());
            bRow.Cells[9].MergeRight = 1;

            //Total
            bRow = BillTable.AddRow();
            bRow.Format.Alignment = ParagraphAlignment.Center;
            bRow.Format.Font.Size = 6.5;

            bRow.Cells[0].AddParagraph("CGST Total (₹)");
            bRow.Cells[0].Format.Alignment = ParagraphAlignment.Center;
            bRow.Cells[0].MergeRight = 8;
            bRow.Cells[9].AddParagraph(b.BillTotal.ToString());
            bRow.Cells[9].MergeRight = 1;
            BillTable.SetEdge(0, 0, 4, b.getBillItemsCount(), Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            #endregion

            #region Document Render
            // Create a renderer and prepare (=layout) the document
            MigraDoc.Rendering.DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(4.2), XUnit.FromCentimeter(2.3), "8cm", InvoiceTable);
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(0.2), XUnit.FromCentimeter(3.8), "10cm", BillTable);
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
            //get the directort path to save the bills
            string billDirPath = ConfigurationSettings.AppSettings["DIRPath"].ToString();
            string billfolderPath = billDirPath + "\\" + DateTime.Now.Date.ToString("dd-MM-yyyy");

            return ConfigurationSettings.AppSettings["ShopCode"].ToString() + "/" + System.DateTime.Now.ToString("yyMMdd") + "/" + GetFileCountInDirectory(billfolderPath);
        }

    }
}
