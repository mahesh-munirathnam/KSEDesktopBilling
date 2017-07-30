using System.Windows;
using System.Windows.Controls;
using KSEDesktopBilling.ViewModels;
using System.Drawing.Printing;
using System;
using KSE.helpers;
using KSE.ViewModels;


namespace KSEDesktopBilling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //myBrowser.Navigate(new Uri("http://www.ksesilk.com"));
        }

        private void GenerateBill_Click(object sender, RoutedEventArgs e)
        {
            //PrintDialog printDlg = new PrintDialog();
            //PrintDocument DOC = new PrintDocument();
            //DOC.DefaultPageSettings.PaperSize.Height = 3;
            //DOC.DefaultPageSettings.PaperSize.Width = 3;
            //DOC.DefaultPageSettings.Landscape = false;
            ////DOC.
            //printDlg.PrintVisual(Layout, "Grid Printing.");
            BillViewModel bill = DataContext as BillViewModel;
            if(bill.getBillItemsCount() > 0)
            {
                PrintHelpers.HelloPdf(bill);
            }
            else
            {
                MessageBox.Show("Emtpy Bill, no items added");
            }
        }

    }
}
