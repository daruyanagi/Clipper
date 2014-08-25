﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Clipper.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Windows.System;

    public class MainPageViewModel : BindableBase
    {
        public MainPageViewModel()
        {
            LoadCommand = new RelayCommand(Load);
            LoadDefaultCommand = new RelayCommand(LoadDefault);
            SaveCommand = new RelayCommand(Save);
            MoveUpCommand = new RelayCommand(MoveUp, () => SelectedTextFormat != null && SelectedTextFormatIndex != 0);
            MoveDownCommand = new RelayCommand(MoveDown, () => SelectedTextFormat != null && SelectedTextFormatIndex != List.Count - 1);
            RemoveCommand = new RelayCommand(Remove, () => SelectedTextFormat != null);
            AddCommand = new RelayCommand(Add);
            ClearAllCommand = new RelayCommand(ClearAll);
            TwitterAuthenticateCommand = new RelayCommand(TwitterAuthenticate);
            GoToHomePageCommand = new RelayCommand(GoToHomepage);

            Load();

            PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "Preview")
                    {
                        foreach (var item in List)
                        {
                            item.UpdatePreview(item.Format, this);
                        }
                    }
                };
        }

        private string homePageUrl = "http://daruyanagi.net/";
        private string privacyPolicyUrl = "http://download.daruyanagi.net/privacy%20policy";

        public void MoveUp()
        {
            var index = List.IndexOf(selectedTextFormat);

            List.Move(index, --index);

            SelectedTextFormatIndex = index;
        }

        public void MoveDown()
        {
            var index = List.IndexOf(selectedTextFormat);

            List.Move(index, ++index);

            SelectedTextFormatIndex = index;
        }

        public void Add()
        {
            var newItem = new TextFormat()
            {
                Title = "New Format",
                Format = "{{comment}}",
            };
            List.Add(newItem);
            SelectedTextFormat = newItem;
        }

        public void ClearAll()
        {
            List.Clear();
            SelectedTextFormat = null;
        }

        public void Remove()
        {
            List.Remove(selectedTextFormat);

            SelectedTextFormat = null;
        }

        public async void TwitterAuthenticate()
        {
            await App.Twitter.GainAccessToTwitter();
        }

        public async void GoToHomepage()
        {
            await Launcher.LaunchUriAsync(new Uri(homePageUrl));
        }

        public RelayCommand LoadCommand { get; private set; }
        public RelayCommand LoadDefaultCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand MoveUpCommand { get; private set; }
        public RelayCommand MoveDownCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand ClearAllCommand { get; private set; }
        public RelayCommand TwitterAuthenticateCommand { get; private set; }
        public RelayCommand GoToHomePageCommand { get; private set; }

        public async void Load()
        {
            try
            {
                var folder = ApplicationData.Current.RoamingFolder;
                var file = await folder.GetFileAsync("TextFormats.json");
                var json = await FileIO.ReadTextAsync(file);

                if (string.IsNullOrEmpty(json)) throw new Exception();

                List = json.Deserialize<ObservableCollection<TextFormat>>();
            }
            catch
            {
                Notification.Toast("Can not load Setting file.\r\nLoad Default Settings...");
                LoadDefault();
            }
        }

        public void LoadDefault()
        {
            List = new ObservableCollection<TextFormat>()
            {
                new TextFormat()
                {
                    Title = "Title and URL",
                    Format = "{{title}}\r\n{{url}}",
                },
                new TextFormat()
                {
                    Title = "Selected Text(or Title) and URL",
                    Format = "{{text}}\r\n{{url}}",
                },
                new TextFormat()
                {
                    Title = "Comment and URL",
                    Format = "{{comment}} / {{url}}",
                },
                new TextFormat()
                {
                    Title = "Markdown",
                    Format = "[{{text}}]({{url}})",
                },
            };
        }

        public async void Save()
        {
            try
            {
                var folder = ApplicationData.Current.RoamingFolder;
                var file = await folder.CreateFileAsync("TextFormats.json", CreationCollisionOption.ReplaceExisting);
                var json = List.Serialize();
                
                await FileIO.WriteTextAsync(file, json);
            }
            catch
            {
                
            }
        }

        private string title = "This is title";
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

        private string description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.";
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private string appName = "Clipper";
        public string AppName
        {
            get { return appName; }
            set { SetProperty(ref appName, value); }
        }

        private Uri appLink = new Uri("clipper://dummy");
        public Uri AppLink
        {
            get { return appLink; }
            set { SetProperty(ref appLink, value); }
        }

        private Uri webLink = new Uri("http://daruyanagi.net/");
        public Uri WebLink
        {
            get { return webLink; }
            set { SetProperty(ref webLink, value); }
        }

        private string selectedText = "Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged.";
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

        private int selectedTextFormatIndex;
        public int SelectedTextFormatIndex
        {
            get { return selectedTextFormatIndex; }
            set
            {
                if (SetProperty(ref selectedTextFormatIndex, value))
                {
                    MoveUpCommand.RaiseCanExecuteChanged();
                    MoveDownCommand.RaiseCanExecuteChanged();
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
                    MoveUpCommand.RaiseCanExecuteChanged();
                    MoveDownCommand.RaiseCanExecuteChanged();
                    RemoveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private ObservableCollection<TextFormat> list;
        public ObservableCollection<TextFormat> List
        {
            get { return list; }
            set
            {
                if (SetProperty(ref list, value))
                {
                    RaisePropertyChanged("Preview");
                    MoveUpCommand.RaiseCanExecuteChanged();
                    MoveDownCommand.RaiseCanExecuteChanged();
                }
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

            public void UpdatePreview(string format, dynamic viewModel)
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
