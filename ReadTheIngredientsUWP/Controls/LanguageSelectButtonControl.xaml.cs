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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ReadTheIngredientsUWP.Controls
{
    public sealed partial class LanguageSelectButtonControl : UserControl
    {
        public LanguageSelectButtonControl()
        {
            this.InitializeComponent();

            
//            this.Loaded += LanguageSelectButtonControl_Loaded;
        }

        private void LanguageSelectButtonControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void LoadImage()
        {
                var path = "ms-appx:///Assets/" + this.LanguageCode + ".svg";
                var result = StorageFile.GetFileFromApplicationUriAsync(new Uri(path));
                var task = result.AsTask();
                task.Wait();
                var file = task.Result;
                this.SvgImageControl.LoadFileAsync(file);

        }

        // Url="Assets/en.svg" 

        public string LanguageCode
        {
            get { return (string)GetValue(CodeProperty); }
            set
            {
                SetValue(CodeProperty, value);
            }
        }

        private void SetLanguage(string language)
        {
            var ctrl = this.DataContext as ProductsControllerShared.ProductsController;

            ctrl.NextLanguage();
        }

        public static readonly DependencyProperty CodeProperty =
    DependencyProperty.Register("LanguageCode", typeof(string), typeof(LanguageSelectButtonControl), new PropertyMetadata(null, new PropertyChangedCallback(OnLanguageCodeChanged)));

        private static void OnLanguageCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) { return; }
            var dep = d as LanguageSelectButtonControl;
            var language = "en";
            var value = e.NewValue.ToString();
            if (value == "nl")
            {
                dep.LanguageTextBlock.Text = "Nederlands";
            }
            else if (value == "fr")
            {
                dep.LanguageTextBlock.Text = "Français";
            }
            else if (value == "de")
            {
                dep.LanguageTextBlock.Text = "Deutsch";
            }
            else if (value == "es")
            {
                dep.LanguageTextBlock.Text = "Español";
            }
            else
            {
                dep.LanguageTextBlock.Text = "English";
            }
            //dep.SetLanguage(language);
            dep.LoadImage();
        }

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set
            {
                var path = "ms-appx:///" + value;
                SetValue(UrlProperty, path);
            }
        }
        public static readonly DependencyProperty UrlProperty =
DependencyProperty.Register("Url", typeof(string), typeof(LanguageSelectButtonControl), null);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var productVm = this.DataContext as ProductsControllerShared.ProductsController;
            productVm.NextLanguage();
        }
    }
}
