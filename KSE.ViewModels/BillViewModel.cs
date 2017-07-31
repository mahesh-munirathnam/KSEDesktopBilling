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

        private decimal txtBillDiscount_;
        private readonly ObservableCollection<BillItem> billitems = new ObservableCollection<BillItem>();
        private readonly ObservableCollection<ItemType> types_ = new ObservableCollection<ItemType>();
        private ItemType SelectItemType_;

        public BillViewModel()
        {
            _b = new BillItem();
            _b.Name = txtItemName;
            _b.Price = txtItemPrice;
            _b.Quantity = txtItemQuatity;
            types_.Add(new ItemType("Ss", "Saree/fabric"));
            types_.Add(new ItemType("Rm", "Ready Made"));
        }

        public ItemType SelectedItemType
        {
            get
            {
                return SelectItemType_;
            }
            set
            {
                SelectItemType_ = value;
            }
        }

        public IList<ItemType> types
        {
            get { return types_; }
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
                RaisePropertyChanged("txtItemName");
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
                RaisePropertyChanged("lblDiscountTotal");
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
                RaisePropertyChanged("lblDiscountTotal");
            }
        }

        public decimal txtDiscount
        {
            get
            {
                return _b.DiscountRate;
            }
            set
            {
                if (_b.DiscountRate != value)
                    _b.DiscountRate = value;
                RaisePropertyChanged("txtDiscount");
                RaisePropertyChanged("lblItemTotal");
                RaisePropertyChanged("lblDiscountTotal");
            }
        }

        public decimal lblItemTotal
        {
            get
            {
                return txtItemPrice * txtItemQuatity;
            }
        }

        public decimal lblDiscountTotal
        {
            get
            {
                return (txtDiscount * lblItemTotal) / 100;
            }
        }

        public decimal BillAmount
        {
            get
            {
                return billitems.Sum(i => i.TaxableAmount);
            }
        }

        public decimal txtBillDiscount
        {
            get
            {
                return txtBillDiscount_;
            }
            set
            {
                if (txtBillDiscount_ != value)
                    txtBillDiscount_ = value;
                RaisePropertyChanged("txtBillDiscount");
            }
        }

        public decimal tax
        {
            get
            {
                return ComputeTotalTax();
            }
        }

        public decimal BillTotal
        {
            get { return BillAmount + (2*tax); }
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

        public ICommand DiscountCommand
        {
            get { return new DelegateCommand(DiscountBill); }
        }

        private void AddItem()
        {
            BillItem B = new BillItem();
            if (!string.IsNullOrWhiteSpace(txtItemName) && txtItemPrice > 0 && txtItemQuatity > 0)
            {
                B.Name = txtItemName;
                B.Quantity = txtItemQuatity;
                B.Price = txtItemPrice;
                B.DiscountRate = txtDiscount;
                B.ItemCode = SelectedItemType.ItemCode;
                billitems.Add(B);
                txtItemName = string.Empty;
                txtItemPrice = 0;
                txtItemQuatity = 0;
                txtDiscount = 0;
                UpdateBill();
            }
            else
            {

            }
        }

        private void ClearBill()
        {
            billitems.Clear();
            UpdateBill();
        }

        private void UpdateBill()
        {
            RaisePropertyChanged("BillTotal");
            RaisePropertyChanged("tax");
            RaisePropertyChanged("BillAmount");
        }

        private void DiscountBill()
        {
            BillItems.ToList().ForEach(i => i.DiscountRate = txtBillDiscount);
            UpdateBill();
        }

        public int getBillItemsCount()
        {
            return BillItems.Count();
        }

        private decimal ComputeTotalTax()
        {
            return billitems.Sum(i => i.TaxAmount);
        }

        //public ICommand RemoveCommand
        //{
        //    get { return new DelegateCommand(RemoveItem())}
        //}

        //private void RemoveItem(BillItem i)
        //{
        //    billitems.Remove(i);
        //}
    }
}
