using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace WPFTerminal
{
    public partial class Console : UserControl
    {
        public Console()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(
                nameof(Items),
                typeof(IList),
                typeof(Console),
                new PropertyMetadata(null));

        public IList Items
        {
            get => (IList)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
    }
}
