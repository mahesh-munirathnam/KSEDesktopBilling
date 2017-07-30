using System.Linq;
using KSE.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using KSE.ViewModels.Common;
using System.Collections;
using System.Collections.Generic;
using System;

namespace KSE.ViewModels
{
    public class BillViewModel : ViewModelBase
    {
        private BillItem _b;
        private decimal _tax = new decimal(2.5);
        private readonly ObservableCollection<BillItem> billitems = new ObservableCollection<BillItem>();
        public ObservableCollection<ItemType> types = new ObservableCollection<ItemType>();
        public ItemType ditemtype = new ItemType("def","Select Item");
        public BillViewModel()
        {
            _b = new BillItem();
            _b.Name = txtItemName;
            _b.Price = txtItemPrice;
            _b.Quantity = txtItemQuatity;
            types.Add(new ItemType("Ss", "Saree/fabric"));
            types.Add(new ItemType("Rm","Ready Made"));
        }

        public string txtItemName
        {
            get
            {
                return _b.Name;
            }
            set
            {
                if (_b.Name != value)
                    _b.Name = value;
                RaisePropertyChanged("txtItemQuatity");
            }
        }

        public decimal txtItemQuatity
        {
            get
            {
                return _b.Quantity;
            }
            set
            {
                if (_b.Quantity != value)
                    _b.Quantity = value;
                RaisePropertyChanged("txtItemQuatity");
                RaisePropertyChanged("lblItemTotal");
            }
        }

        public decimal txtItemPrice
        {
            get
            {
                return _b.Price;
            }
            set
            {
                if (_b.Price != value)
                    _b.Price = value;
                RaisePropertyChanged("txtItemPrice");
                RaisePropertyChanged("lblItemTotal");
            }
        }

        public decimal lblItemTotal
        {
            get
            {
                return  txtItemPrice * txtItemQuatity;
            }
        }

        public decimal BillTotal
        {
            get
            {
                return billitems.Sum(i => i.Total);
            }
        }

        public decimal tax
        {
            get
            {
                return Math.Round((_tax * billitems.Sum(i => i.Total) / 100));
            }
        }

        public decimal BillAmount
        {
            get { return BillTotal + tax; }
        }

        public IEnumerable<BillItem> BillItems
        {
            get
            {
                return billitems;
            }
        }

        public ICommand AddCommand
        {
            get { return new DelegateCommand(AddItem); }
        }

        public ICommand ClearCommand
        {
            get { return new DelegateCommand(ClearBill); }
        }

        private void AddItem()
        {
            BillItem B = new BillItem();
            if (!string.IsNullOrWhiteSpace(txtItemName) &&  txtItemPrice> 0 &&  txtItemQuatity> 0)
            {
                B.Name = txtItemName;
                B.Quantity = txtItemPrice;
                B.Price = txtItemQuatity;
                billitems.Add(B);
                txtItemName = string.Empty;
                txtItemPrice = 0;
                txtItemPrice = 0;
            }
            RaisePropertyChanged("BillTotal");
            RaisePropertyChanged("tax");
            RaisePropertyChanged("BillAmount");
        }

        private void ClearBill()
        {
            billitems.Clear();
        }

        //public ICommand RemoveCommand
        //{
        //    get { return new DelegateCommand(RemoveItem())}
        //}

        private void RemoveItem(BillItem i)
        {
            billitems.Remove(i);
        }

        public int getBillItemsCount()
        {
            return BillItems.Count();
        }

    }
}
