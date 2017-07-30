using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace KSE.ViewModels.BillCommands
{
    public class AddItemCommand : ICommand
    {
        private BillViewModel _obj;

        public AddItemCommand(BillViewModel Obj)
        {
            _obj = Obj;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(_obj.txtItemName) && _obj.txtItemQuatity > 0 && _obj.txtItemPrice > 0)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            _obj.AddItem();
        }
    }
}
