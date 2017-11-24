using ProductsControllerShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ReadTheIngredientsUWP.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class SearchControl : UserControl
    {
        private ProductsController _ctrl;
        public event EventHandler<SearchEventArgs> StartSearch;
        public class SearchEventArgs: EventArgs
        {
            public SearchEventArgs(string searchText)
            {
                SearchText = searchText;
            }
            public string SearchText { get; }
        }

        public void OnStartSearch()
        {
            EventHandler<SearchEventArgs> handler = StartSearch;
            var e = new SearchEventArgs(SearchTextBox.Text);          
            handler?.Invoke(this, e);
        }

        public SearchControl()
        {
            this.InitializeComponent();

            this.DataContextChanged += SearchControl_DataContextChanged;
        }

        private void SearchControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (this.DataContext is ProductsController)
            {
                _ctrl = this.DataContext as ProductsController;
            }
        }

        private void SearchProductClicked(object sender, RoutedEventArgs e)
        {
            if (_ctrl is ProductsController)
            {
                OnStartSearch();
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                OnStartSearch();
            }
        }
    }
}
