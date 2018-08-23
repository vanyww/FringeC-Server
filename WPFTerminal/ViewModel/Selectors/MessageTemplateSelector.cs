using System;
using System.Windows;
using System.Windows.Controls;
using WPFTerminal.Model;

namespace WPFTerminal.ViewModel.Selectors
{
    internal sealed class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IconAndSource { get; set; }
        public DataTemplate NoSource { get; set; }
        public DataTemplate NoIcon { get; set; }
        public DataTemplate NoIconAndSource { get; set; }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            var mesg = (ConsoleMessage)item;

            if (mesg.Image == null && mesg.SourceObject == null) return NoIconAndSource;
            if (mesg.Image == null) return NoIcon;
            if (mesg.SourceObject == null) return NoSource;
            return IconAndSource;
        }
    }
}
