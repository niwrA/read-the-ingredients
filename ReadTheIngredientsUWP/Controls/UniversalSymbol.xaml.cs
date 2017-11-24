using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ReadTheIngredientsUWP.Controls
{
    public partial class UniversalSymbol : UserControl
    {
        private IReadOnlyDictionary<string, int> _faDict { get; set; }
        private IReadOnlyDictionary<string, int> _symbolDict { get; set; }
        private IReadOnlyDictionary<string, int> _emojiDict { get; set; }
        private FontFamily originalFontFamily;
        public UniversalSymbol()
        {
            this.InitializeComponent();
            originalFontFamily = this.FontFamily;
            _faDict = new Dictionary<string, int> {
                { "comment", 0xf075 },
                { "credit-card", 0xf09d },
                { "check-square", 0xf14a },
                { "cog", 0xf013 },
                { "cogs", 0xf085 },
                { "code-fork", 0xf126 },
                { "remove", 0xf00d },
                { "database", 0xf1c0 },
                { "calendar", 0xf073 },
                { "asterisk", 0xf069 },
                { "filter", 0xf0b0 },
                { "globe", 0xf0ac },
                { "home", 0xf015 },
                { "inbox", 0xf01c },
                { "keyboard-o", 0xf11c },
                { "search", 0xf002 },
                { "lightbulb", 0xf0eb },
                { "plus", 0xf067 },
                { "sign-in", 0xf090 },
                { "sign-out", 0xf08b },
                { "toggle-on", 0xf205 },
                { "user", 0xf007 },
                { "users", 0xf0c0 },
                { "wrench", 0xf0ad },
                { "trash-o", 0xf014 },
                { "folder-open-o", 0xf115 },
                { "tree", 0x11bb },
                { "angle-down", 0xf107 },
                { "angle-up", 0xf106 },
                { "camera", 0xf030 }
            };
            _symbolDict = new Dictionary<string, int> {
                { "forward", 0xE72A },
                { "back", 0xE72B },
                { "edit", 0xE104 },
                { "hamburger", 0xE700 },
                { "save", 0xE74E },
                { "share", 0xE72D },
                { "favorites",0xE734 },
                { "add", 0xE710 },
                { "accept", 0xE8FB },
                { "delete", 0xE74D },
                { "cancel", 0xE711 },
                { "folder", 0xE8B7 },
                { "filter", 0xE71C },
                { "show_details", 0xE8C4 },
                { "hide_details", 0xE8C5 },
                { "camera", 0xE722 },
                { "find", 0xE11A },
                { "search", 0xE721 }
            };
            _emojiDict = new Dictionary<string, int> {
                { "Earth Globe Europe-Africa", 0x1F30D },
                { "Earth Globe Americas", 0x1F30E },
                { "Earth Globe Asia-Australia", 0x1F30F },
                { "Jack-O-Lantern", 0x1F383 },
                { "Check Mark", 0x2713 },
                { "Ballot X", 0x2717 },
                { "Floppy Disk", 0x1F4BE },
                { "Books", 0x1F4DA },
                { "Open Book", 0x1F4D6 },
                { "Closed Book", 0x1F4D5 },
                { "Green Book", 0x1F4D7 },
                { "Orange Book", 0x1F4D9 },
                { "File Folder", 0x1F4C1 },
                { "Open File Folder", 0x1F4C2 },
                { "Page Facing Up", 0x1F4C4 }
            };
        }

        public string FontAwesomeName
        {
            get { return (string)GetValue(FontAwesomeNameProperty); }
            set
            {
                var name = value;
                if (value.StartsWith("fa-")) { name = value.Substring(3); }

                if (_faDict.ContainsKey(value))
                {
                    var fontChar = 0;
                    if (_faDict.TryGetValue(value, out fontChar))
                    {
                        if (this.FontFamily.Source != "Assets/fontawesome-webfont.ttf#FontAwesome")
                        {
                            var fam = new FontFamily("Assets/fontawesome-webfont.ttf#FontAwesome");
                            this.IconTextBlock.FontFamily = fam;
                        }
                        this.IconTextBlock.Text = Char.ConvertFromUtf32(fontChar);
                    }
                    this.IconTextBlockOverlay.Text = "";
                }

                SetValue(FontAwesomeNameProperty, value);
            }
        }

        public string SegoeMDL2AssetName
        {
            get { return (string)GetValue(SegoeMDL2AssetNameProperty); }
            set
            {
                var name = value;
                var overlay = "";
                if (value.Contains(","))
                {
                    var values = name.Split(',');
                    name = values[0];
                    overlay = values[1];
                }
                if (_symbolDict.ContainsKey(name) || _symbolDict.ContainsKey(overlay))
                {
                    var fontChar = 0;
                    if (_symbolDict.TryGetValue(name, out fontChar))
                    {
                        if (this.FontFamily.Source != "Segoe MDL2 Assets")
                        {
                            FontFamily fam = new FontFamily("Segoe MDL2 Assets");
                            this.IconTextBlock.FontFamily = fam;
                        }
                        this.IconTextBlock.Text = Char.ConvertFromUtf32(fontChar);
                    }
                    var fontCharOverlay = 0;
                    if (_symbolDict.TryGetValue(overlay, out fontCharOverlay))
                    {
                        if (this.FontFamily.Source != "Segoe MDL2 Assets")
                        {
                            FontFamily fam = new FontFamily("Segoe MDL2 Assets");
                            this.IconTextBlockOverlay.FontFamily = fam;
                        }
                        //IconTextBlockOverlay.Margin = new Thickness(10,10,0,0);
                        this.IconTextBlockOverlay.Text = Char.ConvertFromUtf32(fontCharOverlay);
                    }
                    else
                    {
                        //IconTextBlockOverlay.Margin = new Thickness(0,0,0,0);
                        this.IconTextBlockOverlay.Text = "";
                    }
                }
                SetValue(SegoeMDL2AssetNameProperty, value);
            }
        }

        public string EmojiName
        {
            get { return (string)GetValue(SegoeMDL2AssetNameProperty); }
            set
            {
                var name = value;

                if (_emojiDict.ContainsKey(value))
                {
                    var fontChar = 0;
                    if (_emojiDict.TryGetValue(value, out fontChar))
                    {
                        if (this.FontFamily.Source != "Segoe UI Emoji")
                        {
                            FontFamily fam = new FontFamily("Segoe UI Emoji");
                            this.IconTextBlock.FontFamily = fam;
                        }
                        this.IconTextBlock.Text = Char.ConvertFromUtf32(fontChar);
                    }
                    this.IconTextBlockOverlay.Text = "";
                }

                SetValue(SegoeMDL2AssetNameProperty, value);
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {

                if (this.FontFamily != this.originalFontFamily)
                {
                    this.FontFamily = this.originalFontFamily;
                }
                this.IconTextBlock.Text = value;
            }
        }
        public static readonly DependencyProperty TextProperty =
DependencyProperty.Register("Text", typeof(string), typeof(UniversalSymbol), null);
        // Using a DependencyProperty as the backing store for FontAwesomeName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontAwesomeNameProperty =
            DependencyProperty.Register("FontAwesomeName", typeof(string), typeof(UniversalSymbol), null);

        public static readonly DependencyProperty SegoeMDL2AssetNameProperty =
            DependencyProperty.Register("SegoeMDL2AssetName", typeof(string), typeof(UniversalSymbol), null);

        public static readonly DependencyProperty EmojiNameProperty =
    DependencyProperty.Register("EmojiName", typeof(string), typeof(UniversalSymbol), null);



    }
}
