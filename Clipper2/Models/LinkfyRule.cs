using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clipper2.Models
{
    public class LinkfyRule : BindableBase
    {
        private Guid id = Guid.NewGuid();
        public Guid Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string name = "New Linkfy Rule";
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string rule = "<a href='{{url}}'>{{title}}</a>";
        public string Rule
        {
            get { return rule; }
            set { SetProperty(ref rule, value); RaisePropertyChanged(nameof(DisplayRule)); }
        }

        public string DisplayRule
        {
            get { return rule.Replace("\r", "\\r"); ; }
        }

        public string Generate(LinkfyData data)
        {
            try
            {
                var f = rule;
                f = f.Replace("{{comment}}", data.Comment);

                f = f.Replace("{{title}}", "{0}");

                f = f.Replace("{{description}}", "{1}");
                f = f.Replace("{{desc}}", "{1}");

                f = f.Replace("{{weblink}}", "{2}");
                f = f.Replace("{{link}}", "{2}");
                f = f.Replace("{{url}}", "{2}");
                f = f.Replace("{{uri}}", "{2}");

                f = f.Replace("{{text}}", "{3}");

                f = f.Replace("{{appname}}", "{4}");
                f = f.Replace("{{applink}}", "{5}");

                f = f.Replace("{{html}}", "{6}");

                return string.Format(f, data.Title, data.Description, data.WebLink, data.Text, data.ApplicationName, data.ApplicationLink, data.HtmlFormat);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
