using System.Windows;
using KSE.helpers;
using KSE.ViewModels;
using System.IO;
using System.Printing;
using System.Diagnostics;

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
            BillViewModel bill = DataContext as BillViewModel;
            if (bill.getBillItemsCount() > 0)
            {
                string BillPath = PrintHelpers.PrintA4Bill(bill);
                SendToPrinter(BillPath);
            }
            else
            {
                MessageBox.Show("Emtpy Bill, no items added");
            }
        }

        private void SendToPrinter(string filename)
        {

            //Using below code we can print any document
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = filename;
            info.Verb = "Print";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info);
        }

    }
}
