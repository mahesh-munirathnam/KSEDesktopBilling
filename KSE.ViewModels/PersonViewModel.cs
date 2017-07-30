using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSE.Models;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Input;
using KSE.ViewModels.Common;

namespace KSE.ViewModels
{
    public class PersonViewModel : ViewModelBase
    {
        private Person _p;

        private readonly ObservableCollection<string> _names = new ObservableCollection<string>();

        public PersonViewModel()
        {
            _p = new Person();
            _p.FName = txtFName;
            _p.LName = txtFName;
        }

        public string txtFName
        {
            get
            {
                return _p.FName;
            }
            set
            {
                if(_p.FName != value)
                {
                    _p.FName = value;
                    RaisePropertyChanged("txtFName");
                    RaisePropertyChanged("lblFullName");
                }
            }
        }

        public string txtLname
        {
            get
            {
                return _p.LName;
            }
            set
            {
                if(_p.LName != value)
                {
                    _p.LName = value;
                    RaisePropertyChanged("txtLname");
                    RaisePropertyChanged("lblFullName");
                }
            }
        }

        public string lblFullName
        {
            get
            {
                return _p.FName + " " + _p.LName;
            }
        }

        public IEnumerable<string> Names
        {
            get
            {
                return _names;
            }
        }

        public ICommand AddNameCommand
        {
            get { return new DelegateCommand(AddNames); }
        }

        private void AddNames()
        {
            if (string.IsNullOrWhiteSpace(lblFullName)) return;
            AddToCollection(lblFullName);
            txtFName = string.Empty;
            txtLname = string.Empty;
        }

        private void AddToCollection(string s)
        {
            if (!_names.Contains(s))
                _names.Add(s);
        }

    }
}
