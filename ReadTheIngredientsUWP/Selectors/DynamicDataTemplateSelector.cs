using ProductsControllerShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ReadTheIngredientsUWP
{
    public class DynamicDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IngredientDataTemplate { get; set; }
        public DataTemplate ExpandedIngredientDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ExpandedIngredientViewModel)
                return ExpandedIngredientDataTemplate;
            if (item is IngredientViewModel)
                return IngredientDataTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
