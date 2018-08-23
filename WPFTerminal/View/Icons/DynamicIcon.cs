using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFTerminal.View.Icons
{
    public static class DynamicIcon
    {
        private static void SourceResourceKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as Image;

            if (element != null)
            {
                element.SetResourceReference(Image.SourceProperty, e.NewValue);
            }
        }

        public static readonly DependencyProperty SourceResourceKeyProperty = DependencyProperty.RegisterAttached("SourceResourceKey",
               typeof(object),
               typeof(DynamicIcon),
               new PropertyMetadata(String.Empty, SourceResourceKeyChanged));

        public static void SetSourceResourceKey(Image element, Object value) => element.SetValue(SourceResourceKeyProperty, value);

        public static object GetSourceResourceKey(Image element) => element.GetValue(SourceResourceKeyProperty);
    }
}
