using System;
using System.Collections.Generic;
using System.Text;

namespace Clipper
{
    using System;
    using Windows.Data.Xml.Dom;
    using Windows.UI.Notifications;

    public static class Notification
    {
        public static void Toast(string text)
        {
            var toastTemplate = ToastTemplateType.ToastImageAndText01;
            var toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(text));

            // XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
            // ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "https://si0.twimg.com/profile_images/2508237913/rctgk6cpgea29ejboclt.png");
            // ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "profile icon");
            // 
            // IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            // ((XmlElement)toastNode).SetAttribute("duration", "long");
            // 
            // toastNode = toastXml.SelectSingleNode("/toast");
            // XmlElement audio = toastXml.CreateElement("audio");
            // audio.SetAttribute("silent", "true");
            // 
            // toastNode.AppendChild(audio);

            // トーストをクリックした場合にアプリを起動する
            // ローカル通知の場合はあまり関係ない
            // 下記のようにパラメーターを渡すこともできる
            // ((XmlElement)toastNode).SetAttribute("launch", "{\"type\":\"toast\",\"param1\":\"12345\",\"param2\":\"67890\"}");

            // 表示
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
