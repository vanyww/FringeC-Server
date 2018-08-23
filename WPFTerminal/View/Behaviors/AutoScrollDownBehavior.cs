using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WPFTerminal.View.Behaviors
{
    public sealed class AutoScrollDownBehavior : Behavior<ScrollViewer>
    {
        protected override void OnAttached()
        {
            AssociatedObject.ScrollChanged += OnScrollChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ScrollChanged -= OnScrollChanged;
        }


        private void OnScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            Boolean AutoScrollToEnd = true;

            if (sv.Tag != null)
                AutoScrollToEnd = (Boolean)sv.Tag;

            if (e.ExtentHeightChange == 0)
                AutoScrollToEnd = sv.ScrollableHeight == sv.VerticalOffset;
            else
                if (AutoScrollToEnd)
                    sv.ScrollToEnd();

            sv.Tag = AutoScrollToEnd;
        }
    }
}
