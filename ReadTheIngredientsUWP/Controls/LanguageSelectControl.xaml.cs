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
    public sealed partial class LanguageSelectControl : UserControl
    {
        public LanguageSelectControl()
        {
            this.InitializeComponent();

            this.Loaded += LanguageSelectControl_Loaded;
        }

        private void LanguageSelectControl_Loaded(object sender, RoutedEventArgs e)
        {
            LanguageCode = "nl";
        }

        private void LoadImage()
        {
            var path = Url;
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
            SetValue(CodeProperty, language);
            var path = "ms-appx:///Assets/" + language + ".svg";
            SetValue(UrlProperty, path);
        }

        public static readonly DependencyProperty CodeProperty =
    DependencyProperty.Register("LanguageCode", typeof(string), typeof(LanguageSelectControl), new PropertyMetadata(null, new PropertyChangedCallback(OnLanguageCodeChanged)));

        private static void OnLanguageCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) { return; }
            var dep = d as LanguageSelectControl;
            var language = "en";
            var value = e.NewValue.ToString();
            if (value == "nl")
            {
                language = "nl";
                //dep.LanguageTextBlock.Text = "Nederlands";
            }
            else if (value == "fr")
            {
                language = "fr";
                //dep.LanguageTextBlock.Text = "Français";
            }
            else if (value == "de")
            {
                language = "de";
                //dep.LanguageTextBlock.Text = "Français";
            }
            else if (value == "es")
            {
                language = "es";
                //dep.LanguageTextBlock.Text = "Français";
            }
            else
            {
                //dep.LanguageTextBlock.Text = "English";
            }
            dep.SetLanguage(language);
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
DependencyProperty.Register("Url", typeof(string), typeof(LanguageSelectControl), null);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var productVm = this.DataContext;
            if (LanguageCode == "nl")
            {
                LanguageCode = "en";
            }
            else if (LanguageCode == "en")
            {
                LanguageCode = "fr";
            }
            else if (LanguageCode == "fr")
            {
                LanguageCode = "de";
            }
            else if (LanguageCode == "es")
            {
                LanguageCode = "es";
            }
            else
            {
                LanguageCode = "nl";
            }
        }




    }
}
