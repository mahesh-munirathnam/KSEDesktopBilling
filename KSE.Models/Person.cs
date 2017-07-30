using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSE.Models
{
    public class Person
    {
        private string _FName;
        private string _LName;

        public string FName
        {
            get { return _FName; }
            set { if (_FName != value) _FName = value; }
        }
        public string LName
        {
            get { return _LName; }
            set { if (_LName != value) _LName = value; }
        }
    }
}
