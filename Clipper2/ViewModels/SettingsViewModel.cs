using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Clipper2.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public RelayCommand BackCommand { get; } = new RelayCommand(() =>
        {
            var frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(Views.MainPage));
        });

        public string ApplicationName => Package.Current.DisplayName;

        public string ApplicationVersion
        {
            get
            {
                var v = Package.Current.Id.Version;
                return string.Format("{0}.{1}.{2}.{3}", v.Major, v.Minor, v.Revision, v.Build);
            }
        }

        public string ApplicationUrl => "http://daruyanagi.jp/";

        public string PrivacyPolicyUrl => "http://daruyanagi.jp/entry/privacy_policy";
    }
}
