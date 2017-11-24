using ProductsControllerShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class IngredientDetailControl : Page
    {
        private IngredientViewModel _ingredient;
        public IngredientDetailControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += IngredientDetailControl_DataContextChanged;
        }

        private void IngredientDetailControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            _ingredient = args.NewValue as IngredientViewModel;
        }


        private async void WikiData(object sender, TappedRoutedEventArgs e)
        {
            var uri = new Uri(@"https://www.wikidata.org/wiki/Q" + _ingredient.WikiDataId.ToString());
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
