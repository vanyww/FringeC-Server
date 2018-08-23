using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WPFTerminal.Model;

namespace WPFTerminal.ViewModel
{
    public class ConsoleViewModel
    {
        static ConsoleViewModel()
        {
            Consoles = new Dictionary<String, ConsoleViewModel>();
        }

        public ConsoleViewModel(string name)
        {
            Messages = new ObservableCollection<ConsoleMessage>();
            Consoles.Add(name, this);
        }

        public ObservableCollection<ConsoleMessage> Messages { get; }
        public static Dictionary<String, ConsoleViewModel> Consoles { get; }
    }
}
