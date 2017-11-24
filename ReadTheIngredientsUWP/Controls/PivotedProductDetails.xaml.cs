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
    public sealed partial class PivotedProductDetails : UserControl
    {
        public event EventHandler<IngredientDetailRequestEventArgs> IngredientDetailRequested;
        public class IngredientDetailRequestEventArgs : EventArgs
        {
            public IngredientDetailRequestEventArgs(IngredientViewModel vm)
            {
                Ingredient = vm;
            }
            public IngredientViewModel Ingredient { get; }
        }

        public void OnIngredientDetailRequested(IngredientViewModel vm)
        {
            EventHandler<IngredientDetailRequestEventArgs> handler = IngredientDetailRequested;
            var e = new IngredientDetailRequestEventArgs(vm);
            handler?.Invoke(this, e);
        }

        public PivotedProductDetails()
        {
            this.InitializeComponent();
            this.IngredientsCtrl.IngredientDetailRequested += IngredientsCtrl_IngredientDetailRequested;
        }

        private void IngredientsCtrl_IngredientDetailRequested(object sender, ProductIngredientsListControl.IngredientDetailRequestEventArgs e)
        {
            OnIngredientDetailRequested(e.Ingredient);
        }
    }
}
