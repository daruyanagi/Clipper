using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace Clipper2.ViewModels
{
    public class ShareTargetViewModel : BindableBase
    {
        private ShareOperation operation = null;

        public RelayCommand CopyToClipboardCommand { get; private set; }
        public RelayCommand ShareCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }

        public ShareTargetViewModel()
        {
            CopyToClipboardCommand = new RelayCommand(CopyToClipboardAndClose, CheckPreviewAvailable);
            ShareCommand = new RelayCommand(Share, CheckPreviewAvailable);
            CloseCommand = new RelayCommand(Close);

            DataTransferManager.GetForCurrentView().DataRequested += (sender, args) =>
            {
                args.Request.Data.SetText(Preview);
                args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
            };
        }

        private void CopyToClipboardAndClose()
        {
            var package = new DataPackage();
            package.RequestedOperation = DataPackageOperation.Copy;
            package.SetText(Preview);

            void Notify(string message)
            {
                var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
                template.GetElementsByTagName("text").First().AppendChild(template.CreateTextNode(message));
                ToastNotification toast = new ToastNotification(template);
                ToastNotificationManager.CreateToastNotifier().Show(toast);
            };
            
            try
            {
                Clipboard.SetContent(package);
            }
            catch (Exception e)
            {
                Notify($"Clipboard.SetContent(): {e.Message}");
            }

            try
            {
                Clipboard.Flush();
            }
            catch (Exception e)
            {
                Notify($"Clipboard.Flush(): {e.Message}");
            }

            Close();
        }

        private void Share()
        {
            DataTransferManager.ShowShareUI();
        }

        private static void Close()
        {
            Windows.UI.Xaml.Window.Current.Close();
        }

        private bool CheckPreviewAvailable()
        {
            return !string.IsNullOrEmpty(Preview);
        }

        public async Task InitializeAsync(ShareOperation operation)
        {
            this.operation = operation;

            LinkfyData = new Models.LinkfyData();
            await LinkfyData.InitializeAsync(operation);

            await Repositories.LinkfyRuleRepository.LoadAsync();
            LinkfyRules = Repositories.LinkfyRuleRepository.GetAll();
        }

        private void UpdatePreview()
        {
            RaisePropertyChanged(nameof(Preview));
            CopyToClipboardCommand.RaiseCanExecuteChanged();
            ShareCommand.RaiseCanExecuteChanged();
        }

        public string Preview
        {
            get { return selectedLinkfyRule?.Generate(linkfyData); }
        }

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
                UpdatePreview();
            }
        }

        private Models.LinkfyData linkfyData = default;

        public Models.LinkfyData LinkfyData
        {
            get { return linkfyData; }
            set
            {
                SetProperty(ref linkfyData, value);

                if (linkfyData != null) linkfyData.PropertyChanged += (s, e) => UpdatePreview();
            }
        }
    }
}
