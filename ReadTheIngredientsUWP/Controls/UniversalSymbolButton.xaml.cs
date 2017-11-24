using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ReadTheIngredientsUWP.Controls
{
    /// <summary>
    /// Interaction logic for UniversalSymbolButton.xaml
    /// </summary>
    public partial class UniversalSymbolButton : Button
    {
        public UniversalSymbolButton()
        {
            this.InitializeComponent();
        }
        public string FontAwesomeName
        {
            get
            {
                SetValue(FontAwesomeNameProperty, this.UniversalSymbolControl.FontAwesomeName);
                return (string)GetValue(FontAwesomeNameProperty);
            }
            set
            {
                this.UniversalSymbolControl.FontAwesomeName = value;
            }
        }

        public string SegoeMDL2AssetName
        {
            get { return this.UniversalSymbolControl.SegoeMDL2AssetName; }
            set
            {
                this.UniversalSymbolControl.SegoeMDL2AssetName = value;
            }
        }

        public string EmojiName
        {
            get { return this.UniversalSymbolControl.EmojiName; }
            set
            {
                this.UniversalSymbolControl.EmojiName = value;
            }
        }

        public string Text
        {
            get { return this.UniversalSymbolControl.Text; }
            set
            {
                this.UniversalSymbolControl.Text = value;
            }
        }

        // Using a DependencyProperty as the backing store for FontAwesomeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontAwesomeNameProperty =
            DependencyProperty.Register("FontAwesomeName", typeof(string), typeof(UniversalSymbolButton), null);

        public static readonly DependencyProperty SegoeMDL2AssetNameProperty =
            DependencyProperty.Register("SegoeMDL2AssetName", typeof(string), typeof(UniversalSymbolButton), null);

        public static readonly DependencyProperty EmojiNameProperty =
    DependencyProperty.Register("EmojiName", typeof(string), typeof(UniversalSymbolButton), null);

        public static readonly DependencyProperty TextProperty =
DependencyProperty.Register("Text", typeof(string), typeof(UniversalSymbolButton), null);
    }
}
