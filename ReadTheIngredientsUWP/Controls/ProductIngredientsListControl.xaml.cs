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
    public sealed partial class ProductIngredientsListControl : UserControl
    {
        public ProductIngredientsListControl()
        {
            this.InitializeComponent();
        }

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


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                //if (item is ExpandedIngredientViewModel)
                //{
                //    var ingredient = item as ExpandedIngredientViewModel;
                //    if (ingredient.Parent == null)
                //    {
                //        ingredient.Product.Collapse(ingredient);
                //    }
                //    else
                //    {
                //        ingredient.Parent.Collapse(ingredient);
                //    }
                //}
                //else if (item is IngredientViewModel)
                //{
                //    var ingredient = item as IngredientViewModel;
                //    if (ingredient.Parent == null)
                //    {
                //        ingredient.Product.Expand(ingredient);
                //    }
                //    else
                //    {
                //        ingredient.Parent.Expand(ingredient);
                //    }
                //}
                //if (sender is ListView)
                //{
                //    var listView = sender as ListView;
                //    listView.SelectedItem = item;
                //    OnIngredientDetailRequested(item as IngredientViewModel);
                //}

            }
        }

        private void ExpandClick(object sender, TappedRoutedEventArgs e)
        {
            // todo: expand
            var source = sender as Control;
            var ingredient = source.DataContext as IngredientViewModel;

            if (ingredient.Parent == null)
            {
                ingredient.Product.Expand(ingredient);
            }
            else
            {
                ingredient.Parent.Expand(ingredient);
            }
        }

        private void CollapseClick(object sender, TappedRoutedEventArgs e)
        {

            var source = sender as Control;
            var ingredient = source.DataContext as ExpandedIngredientViewModel;
            if (ingredient.Parent == null)
            {
                ingredient.Product.Collapse(ingredient);
            }
            else
            {
                ingredient.Parent.Collapse(ingredient);
            }
        }

        private void IngredientClick(object sender, TappedRoutedEventArgs e)
        {
            var source = sender as TextBlock;
            var ingredient = source.DataContext as IngredientViewModel;
            if(ingredient!=null)
            {
                OnIngredientDetailRequested(ingredient);
            }
        }

        private void ExpandedIngredientClick(object sender, TappedRoutedEventArgs e)
        {
            var source = sender as TextBlock;
            var ingredient = source.DataContext as ExpandedIngredientViewModel;
            if (ingredient != null)
            {
                // todo
                // OnIngredientDetailRequested(ingredient);
            }
        }
    }
}
