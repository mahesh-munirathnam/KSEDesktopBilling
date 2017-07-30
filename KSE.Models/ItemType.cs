using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSE.Models
{
   public class ItemType
    {
        private string ItemCode_;
        private string ItemtypeName_;

        public ItemType(string code, string name)
        {
            ItemCode_ = code;
            ItemtypeName_ = name;
        }
        public string ItemCode
        {
            get
            {
                return ItemCode_;
            }
            set
            {
                if (ItemCode_ != value)
                    ItemCode_ = value;
            }
        }

        public string ItemtypeName
        {
            get
            {
                return ItemtypeName_;
            }
            set
            {
                if (ItemtypeName_ != value)
                    ItemtypeName_ = value;
            }
        }
    }
}
