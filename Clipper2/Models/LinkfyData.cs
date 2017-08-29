using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;

namespace Clipper2.Models
{
    public class LinkfyData : BindableBase
    {
        public LinkfyData()
        {

        }

        public async Task InitializeAsync(ShareOperation operation)
        {
            ApplicationName = operation?.Data.Properties.ApplicationName;

            Description = operation?.Data.Properties.Description;

            Title = operation?.Data.Properties.Title;

            ApplicationLink = operation.Data.Contains(StandardDataFormats.ApplicationLink)
                ? await operation.Data.GetApplicationLinkAsync()
                : default;

            WebLink = operation.Data.Contains(StandardDataFormats.WebLink)
                ? await operation.Data.GetWebLinkAsync()
                : default;

            Text = operation.Data.Contains(StandardDataFormats.Text)
                ? await operation.Data.GetTextAsync()
                : default;

            var html = operation.Data.Contains(StandardDataFormats.Html)
                ? await operation.Data.GetHtmlFormatAsync()
                : default;

            var regex = new Regex("<!--StartFragment -->(.+?)<!--EndFragment -->", RegexOptions.Singleline);
            HtmlFormat = regex.Match(html).Groups.Cast<Group>().LastOrDefault()?.Value ?? string.Empty;

            // 以下はテスト
            var temp1 = operation.Data.Contains(StandardDataFormats.Bitmap)
                ? await operation.Data.GetBitmapAsync()
                : default;

            var temp3 = operation.Data.Contains(StandardDataFormats.Rtf)
                ? await operation.Data.GetRtfAsync()
                : default;

            var temp4 = operation.Data.Contains(StandardDataFormats.StorageItems)
                ? await operation.Data.GetStorageItemsAsync()
                : default;
        }

        private string applicationName = "Sample Application";
        public string ApplicationName
        {
            get { return applicationName; }
            set { SetProperty(ref applicationName, value); }
        }

        private string description = "This description is sample.";
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private string title = "Sample Title";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private Uri applicationLink = default;
        public Uri ApplicationLink
        {
            get { return applicationLink; }
            set { SetProperty(ref applicationLink, value); }
        }

        private string comment = "This is a sample comment.";
        public string Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }

        private string htmlFormat = default;
        public string HtmlFormat
        {
            get { return htmlFormat; }
            set { SetProperty(ref htmlFormat, value); }
        }

        private string text = "Sample text.";
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        private Uri webLink = new Uri("http://sample.com/");
        public Uri WebLink
        {
            get { return webLink; }
            set { SetProperty(ref webLink, value); }
        }
    }
}
