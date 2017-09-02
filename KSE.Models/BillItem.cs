using System;

namespace KSE.Models
{

    public class BillItem 
    {
        private decimal _ltax = new decimal(2.5);
        private decimal _htax = new decimal(6);

        //Private Members
        private string _name;
        private decimal _quantity;
        private decimal _price;
        private string _itemCode;
        private decimal _discount;

        //Public members

        //name Of the items
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                    _name = value;
            }
        }

        //code for the item
        public string ItemCode
        {
            get
            {
                return _itemCode;
            }
            set
            {
                if (_itemCode != value)
                    _itemCode = value;
            }
        }

        //Item Name with Code
        public string FullName
        {
            get
            {
                return _itemCode + " - " + _name;
            }
        }

        //No of peices being sold
        public decimal Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                if (_quantity != value)
                    _quantity = value;
            }
        }

        //price per unit of the item
        public decimal Price
        {
            get
            {
                return decimal.Round(_price, 2, MidpointRounding.AwayFromZero);
            }
            set
            {
                if (_price != value)
                    _price = value;
            }
        }

        //Rate of discount
        public decimal DiscountRate
        {
            get
            {
                return _discount;
            }
            set
            {
                if (_discount != value)
                    _discount = value;
            }
        }

        public decimal DiscountRateString
        {
            get { return decimal.Round(_discount, 2, MidpointRounding.AwayFromZero); }
        }

        //Amount
        public decimal Amount
        {
            get
            {
                return decimal.Round(_price * _quantity, 2, MidpointRounding.AwayFromZero);
            }
        }

        //Taxable Amount
        public decimal TaxableAmount
        {
            get
            {
                return GetTaxableAmount();
            }
        }

        //rate of tax being applied
        public decimal TaxRate
        {
            get
            {
                return GetTaxRate();
            }
        }

        //string format of the tax rate
        public string TaxRateString
        {
            get
            {
                return GetTaxRate().ToString() + "%"; 
            }
        }
        
        //tax amount on the Taxable Total
        public decimal TaxAmount
        {
            get
            {
              return decimal.Round(GetTaxAmount(), 2, MidpointRounding.AwayFromZero);
            }
        }

        //Total amount of the item after discount including the tax
        public decimal Total
        {
            get
            {
                return TaxableAmount + (2*TaxAmount);
            }
        }

        //-----------------------methods--------------------------
        //Function to get Tax rates
        private decimal GetTaxRate()
        {
            if ((Price - GetDiscountPerItem())>= 1000 && !ItemCode.Equals("Ss"))
            {
                return _htax;
            }
            else
            {
                return _ltax;
            }
        }

        //function to get Tax amount
        private decimal GetTaxAmount()
        {
            return (TaxableAmount * GetTaxRate()) / 100;
        }

        //function to get Taxable Amount
        private decimal GetTaxableAmount()
        {
            return Amount - decimal.Round(GetDiscountAmount(), 2, MidpointRounding.AwayFromZero);
        }

        //function to get Discount Amount
        private decimal GetDiscountAmount()
        {
            if (DiscountRate != default(decimal))
            {
                return  ((DiscountRate * Amount) / 100);
            }
            return 0;
        }

        //returns the discount amount per item 
        private decimal GetDiscountPerItem()
        {
            if (DiscountRate != default(decimal))
            {
                return ((DiscountRate * Price) / 100);
            }
            return 0;
        }

    }
}
