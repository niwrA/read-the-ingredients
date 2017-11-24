using ProductsControllerShared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UserSettingsShared;

namespace UserSettingsControllerShared
{
    public class UserSettingsController
    {
        private UserSettings _userSettings { get; set; }
        private IList<string> _supportedLanguages;

        public UserSettingsController(UserSettings userSettings, IList<string> supportedLanguages)
        {
            _userSettings = userSettings;
            _supportedLanguages = supportedLanguages;
            _userLanguage = GetUserLanguage();
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _userDefaultLanguage = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        private string _userLanguage;
        public string UserLanguage
        {
            get
            {
                return _userLanguage;
            }
        }

        private string GetUserLanguage()
        {
            var setting = _userSettings.GetSetting("Language", "Preferred");
            if (setting.IsSet)
            {
                return setting.Value;
            }
            var language = _userDefaultLanguage;
            if (!_supportedLanguages.Contains(language))
            {
                language = "en";
            }
            return language;
        }
    }
}
