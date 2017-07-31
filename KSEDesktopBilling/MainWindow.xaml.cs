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
            BillViewModel bill = DataContext as BillViewModel;
            if(bill.getBillItemsCount() > 0)
            {
                PrintHelpers.PrintA4Bill(bill);
            }
            else
            {
                MessageBox.Show("Emtpy Bill, no items added");
            }
        }

    }
}
