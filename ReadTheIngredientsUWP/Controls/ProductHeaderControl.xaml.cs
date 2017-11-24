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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ReadTheIngredientsUWP.Controls
{
    public sealed partial class ProductHeaderControl : UserControl
    {
        public ProductHeaderControl()
        {
            this.InitializeComponent();
        }

        private async void ProductNameCtrl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var product = this.DataContext as ProductViewModel;
            var uri = new Uri(@"http://" + product.OriginalLanguage + ".openfoodfacts.org/en:product/" + product.Barcode);
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
