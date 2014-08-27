using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Clipper.ViewModels
{
    public class TextFormatBase : BindableBase
    {
        public TextFormatBase()
        {

        }

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

    public static class TextFormatRepository
    {
        private static readonly string SETTINGS_FILE_PATH = "TextFormats.json";

        public static async Task<ObservableCollection<T>> Load<T>()
            where T : TextFormatBase
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.GetFileAsync(SETTINGS_FILE_PATH);
            var json = await FileIO.ReadTextAsync(file);

            return json.Deserialize<ObservableCollection<T>>();
        }


        public static ObservableCollection<T> LoadDefault<T>()
            where T : TextFormatBase, new()
        {
            return new ObservableCollection<T>()
                {
                    new T()
                    {
                        Title = "Title and URL",
                        Format = "{{title}}\r\n{{url}}",
                    },
                    new T()
                    {
                        Title = "Selected Text(or Title) and URL",
                        Format = "{{text}}\r\n{{url}}",
                    },
                    new T()
                    {
                        Title = "Comment and URL",
                        Format = "{{comment}} / {{url}}",
                    },
                    new T()
                    {
                        Title = "Markdown",
                        Format = "[{{text}}]({{url}})",
                    },
                };
        }

        public static async Task Save<T>(ObservableCollection<T> list)
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.CreateFileAsync(
                SETTINGS_FILE_PATH,
                CreationCollisionOption.ReplaceExisting);
            var json = list.Serialize();

            await FileIO.WriteTextAsync(file, json);
        }
    }
}
