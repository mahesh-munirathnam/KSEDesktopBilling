namespace KSE.Models
{

    public class BillItem 
    {
        //Private Members
        private string _name;
        private decimal _quantity;
        private decimal _price;

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

        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (_price != value)
                    _price = value;
            }
        }

        public decimal Total
        {
            get
            {
                return _price * _quantity;
            }
        }
    }
}
