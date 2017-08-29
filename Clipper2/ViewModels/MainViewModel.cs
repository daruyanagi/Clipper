using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clipper2.Models;

namespace Clipper2.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            AddCommand = new RelayCommand(AddRule);
            EditCommand = new RelayCommand(EditRule, CheckRuleIsSelected);
            DeleteCommand = new RelayCommand(DeleteRuleAsync, CheckRuleIsSelected);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
        }

        private bool CheckRuleIsSelected()
        {
            return selectedLinkfyRule != null;
        }

        private void AddRule()
        {
            var frame = Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            frame?.Navigate(typeof(Views.AddRulePage));
        }

        private void EditRule()
        {
            var frame = Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            frame?.Navigate(typeof(Views.EditRulePage), selectedLinkfyRule);
        }

        private async void DeleteRuleAsync()
        {
            Repositories.LinkfyRuleRepository.Delete(selectedLinkfyRule);
            await Repositories.LinkfyRuleRepository.SaveAsync();
        }

        private void OpenSettings()
        {
            var frame = Windows.UI.Xaml.Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            frame?.Navigate(typeof(Views.SettingsPage));
        }

        public async Task InitializeAsync()
        {
            await Repositories.LinkfyRuleRepository.LoadAsync();
            LinkfyRules = Repositories.LinkfyRuleRepository.GetAll();
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand OpenSettingsCommand { get; private set; }

        private ObservableCollection<Models.LinkfyRule> linkfyRules = default;
        public ObservableCollection<Models.LinkfyRule> LinkfyRules
        {
            get { return linkfyRules; }
            set { SetProperty(ref linkfyRules, value); }
        }

        private Models.LinkfyRule selectedLinkfyRule = default;
        public Models.LinkfyRule SelectedLinkfyRule
        {
            get { return selectedLinkfyRule; }
            set
            {
                SetProperty(ref selectedLinkfyRule, value);
                AddCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        private Models.LinkfyData linkfyData = new Models.LinkfyData();
        public Models.LinkfyData LinkfyData
        {
            get { return linkfyData; }
            set { SetProperty(ref linkfyData, value); }
        }
    }
}
