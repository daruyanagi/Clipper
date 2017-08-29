using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clipper2.Models;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace Clipper2.Repositories
{
    public static class LinkfyRuleRepository
    {
        private const string SETTINGS_FILE_PATH = "LinkfyRules.json";
        
        private static ObservableCollection<Models.LinkfyRule> items = default;

        public static async Task LoadAsync()
        {
            try
            {
                var folder = ApplicationData.Current.RoamingFolder;
                var file = await folder.GetFileAsync(SETTINGS_FILE_PATH);
                var json = await FileIO.ReadTextAsync(file);

                items = json.Deserialize<ObservableCollection<Models.LinkfyRule>>();
            }
            catch
            {
                // ToDo: 通知
                LoadDefault();
            }
        }

        public static void LoadDefault()
        {
            items = new ObservableCollection<Models.LinkfyRule>()
            {
                new Models.LinkfyRule()
                {
                    Name = "Title and URL (2 lines)",
                    Rule = "{{title}}\r\n{{url}}",
                },
                new Models.LinkfyRule()
                {
                    Name = "Comment and URL",
                    Rule = "{{comment}} / {{url}}",
                },
                new Models.LinkfyRule()
                {
                    Name = "Markdown",
                    Rule = "[{{text}}]({{url}})",
                },
                new Models.LinkfyRule()
                {
                    Name = "Html Fragment",
                    Rule = "{{html}}",
                },
            };
        }

        public static async Task SaveAsync()
        {
            var folder = ApplicationData.Current.RoamingFolder;
            var file = await folder.CreateFileAsync(
                SETTINGS_FILE_PATH,
                CreationCollisionOption.ReplaceExisting);
            var json = items.Serialize();

            await FileIO.WriteTextAsync(file, json);
        }

        public static ObservableCollection<Models.LinkfyRule> GetAll()
        {
            return items;
        }

        public static void Add(Models.LinkfyRule item)
        {
            items.Add(item);
        }

        public static void Edit(Models.LinkfyRule item)
        {
            var target = items.FirstOrDefault(_ => _.Id == item.Id);

            if (target != null)
            {
                target.Name = item.Name;
                target.Rule = item.Rule;
            }
        }

        public static void Delete(Models.LinkfyRule item)
        {
            var rerult = items.Remove(item);
        }
    }
}
