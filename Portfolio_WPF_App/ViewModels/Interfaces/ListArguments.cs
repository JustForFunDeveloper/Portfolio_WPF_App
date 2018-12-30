using System;
using System.Collections.Generic;

namespace Portfolio_WPF_App.ViewModels.Interfaces
{
    public class ListArguments : EventArgs
    {
        public List<object> Value;

        public ListArguments(List<object> value)
        {
            Value = value;
        }
    }
}
