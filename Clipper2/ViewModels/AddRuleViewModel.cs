using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipper2.ViewModels
{
    public class AddRuleViewModel : BindableBase
    {
        public AddRuleViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            CloseCommand = new RelayCommand(Close);
        }

        public void Initialize(Models.LinkfyRule rule)
        {
            LinkfyRule = rule;
        }

        private async void Save()
        {
            Repositories.LinkfyRuleRepository.Add(linkfyRule);
            await Repositories.LinkfyRuleRepository.SaveAsync();

            Close();
        }

        private void Close()
        {
            var frame = Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            frame?.Navigate(typeof(Views.MainPage));
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }

        public string Preview
        {
            get { return linkfyRule?.Generate(linkfyData); }
        }

        private Models.LinkfyRule linkfyRule = default;
        public Models.LinkfyRule LinkfyRule
        {
            get { return linkfyRule; }
            set
            {
                SetProperty(ref linkfyRule, value);

                if (linkfyRule != null)
                {
                    linkfyRule.PropertyChanged += (sender, args) => RaisePropertyChanged(nameof(Preview));
                }
            }
        }

        private Models.LinkfyData linkfyData = new Models.LinkfyData();
        public Models.LinkfyData LinkfyData
        {
            get { return linkfyData; }
            set
            {
                SetProperty(ref linkfyData, value);
            }
        }
    }
}
