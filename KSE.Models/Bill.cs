using System.Collections.Generic;

namespace KSE.Models
{

    public class Bill
    {
        private int _receiptNo;
        private List<BillItem> _items;

        public int receiptNo
        {
            get
            {
               return _receiptNo;
            }
            set
            {
                if(_receiptNo != value)
                {
                    _receiptNo = value;

                }
            }
        }

        public List<BillItem> items
        {
            get
            {
                return _items;
            }
            set
            {
                if(_items != value)
                {
                    _items = value;
                }
            }
        }
        
    }
}
