using System;
using System.Collections.Generic;
using System.Text;

namespace Clipper.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.DataTransfer;
    using Windows.ApplicationModel.DataTransfer.ShareTarget;
    using Windows.Storage;
using Windows.UI.Xaml;

    public class ShareTargetPageViewModel : BindableBase
    {
        private ShareOperation operation = null;

        public ShareTargetPageViewModel()
        {
            CopyCommand = new RelayCommand(Copy,
                () => SelectedTextFormat != null);
            TweetCommand = new RelayCommand(Tweet,
                () => App.Twitter.AccessGranted
                    && SelectedTextFormat != null 
                    && SelectedTextFormat.Preview.Length <= 140);
        }

        private void Copy()
        {
            operation.ReportStarted();

            var package = new DataPackage();
            package.SetText(SelectedTextFormat.Preview);
            Clipboard.SetContent(package);

            Notification.Toast("Text is copied to clipboard." + "\r\n" + SelectedTextFormat.Preview);

            operation.ReportCompleted();
        }

        private async void Tweet()
        {
            operation.ReportStarted();

            try
            {
                await App.Twitter.UpdateStatus(SelectedTextFormat.Preview);
                Notification.Toast("Text is tweeted." + "\r\n" + SelectedTextFormat.Preview);
            }
            catch (Exception exception)
            {
                Notification.Toast("Error" + "\r\n" + exception.Message);
            }

            operation.ReportCompleted();
        }

        public RelayCommand CopyCommand { get; private set; }
        public RelayCommand TweetCommand { get; private set; }

        public async Task Load()
        {
            try
            {
                var folder = ApplicationData.Current.RoamingFolder;
                var file = await folder.GetFileAsync("TextFormats.json");
                var json = await FileIO.ReadTextAsync(file);

                List = json.Deserialize<ObservableCollection<TextFormat>>();

                ErrorGridVisibility = Visibility.Collapsed;
            }
            catch
            {
                ErrorGridVisibility = Visibility.Visible;
            }
        }
        public async void Initialize(ShareOperation operation)
        {
            this.operation = operation;

            await Load();

            var data = operation.Data;
            var properties = data.Properties;

            Title = properties.Title;
            Description = properties.Description;
            AppName = properties.ApplicationName;
            AppLink = properties.ContentSourceApplicationLink;
            WebLink = properties.ContentSourceWebLink;

            try
            {
                if (WebLink == null)
                    WebLink = await data.GetWebLinkAsync();
            }
            catch { }

            try
            {
                if (AppLink == null)
                    AppLink = await data.GetApplicationLinkAsync();
            }
            catch { }

            try
            {
                SelectedText = await data.GetTextAsync();
            }
            catch { }

            Comment = "This is a sample comment.";
        }

        private string title = string.Empty;
        public string Title
        {
            get { return title; }
            set
            {
                if (SetProperty(ref title, value))
                {
                    RaisePropertyChanged("Text");
                }
            }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private string appName = string.Empty;
        public string AppName
        {
            get { return appName; }
            set { SetProperty(ref appName, value); }
        }

        private Uri appLink = null;
        public Uri AppLink
        {
            get { return appLink; }
            set { SetProperty(ref appLink, value); }
        }

        private Uri webLink = null;
        public Uri WebLink
        {
            get { return webLink; }
            set { SetProperty(ref webLink, value); }
        }

        private string selectedText = string.Empty;
        public string SelectedText
        {
            get { return selectedText; }
            set
            {
                if (SetProperty(ref selectedText, value))
                {
                    RaisePropertyChanged("Text");
                }
            }
        }

        public string Text
        {
            get { return string.IsNullOrEmpty(selectedText) ? title : selectedText; }
        }

        private string comment = string.Empty;
        public string Comment
        {
            get { return comment; }
            set
            {
                if (SetProperty(ref comment, value))
                {
                    RaisePropertyChanged("Preview");

                    if (List == null) return;

                    foreach (var item in List)
                    {
                        item.UpdatePreview(item.Format, this);
                    }
                }
            }
        }

        private TextFormat selectedTextFormat = null;
        public TextFormat SelectedTextFormat
        {
            get { return selectedTextFormat; }
            set
            {
                if (SetProperty(ref selectedTextFormat, value))
                {
                    RaisePropertyChanged("Preview");
                    CopyCommand.RaiseCanExecuteChanged();
                    TweetCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<TextFormat> list;
        public ObservableCollection<TextFormat> List
        {
            get { return list; }
            set { SetProperty(ref list, value); }
        }

        private Visibility errorGridVisibility = Visibility.Visible;
        public Visibility ErrorGridVisibility
        {
            get { return errorGridVisibility; }
            set
            {
                if (SetProperty(ref errorGridVisibility, value))
                {
                    RaisePropertyChanged("MianGridVisibility");
                }
            }
        }

        public Visibility MianGridVisibility
        {
            get
            {
                return errorGridVisibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
            set
            {
                ErrorGridVisibility = value == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }


        public class TextFormat : BindableBase
        {
            private string title;
            public string Title
            {
                get { return title; }
                set { SetProperty(ref title, value); }
            }

            private string format;
            public string Format
            {
                get { return format; }
                set { SetProperty(ref format, value); }
            }

            private string preview;
            public string Preview
            {
                get { return preview; }
            }

            public void UpdatePreview(string format, ShareTargetPageViewModel viewModel)
            {
                try
                {
                    var f = format;
                    f = f.Replace("{{comment}}", viewModel.Comment);
                    f = f.Replace("{{applink}}", "{0}");
                    f = f.Replace("{{appname}}", "{1}");
                    f = f.Replace("{{description}}", "{2}");
                    f = f.Replace("{{desc}}", "{2}");
                    f = f.Replace("{{selected}}", "{3}");
                    f = f.Replace("{{text}}", "{4}");
                    f = f.Replace("{{title}}", "{5}");
                    f = f.Replace("{{weblink}}", "{6}");
                    f = f.Replace("{{link}}", "{6}");
                    f = f.Replace("{{url}}", "{6}");

                    preview = string.Format(f, viewModel.AppLink, viewModel.AppName,
                        viewModel.Description, viewModel.SelectedText, viewModel.Text,
                        viewModel.Title, viewModel.WebLink);
                }
                catch
                {
                    preview = string.Empty;
                }

                RaisePropertyChanged("Preview");
            }
        }
    }
}
