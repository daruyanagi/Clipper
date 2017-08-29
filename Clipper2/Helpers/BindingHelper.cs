using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Clipper2.Helpers
{
    public class BindingHelper
    {
        public static string GetHtml(DependencyObject obj)
        {
            return (string)obj.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject obj, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            obj.SetValue(HtmlProperty, value);
        }

        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof(string), typeof(BindingHelper),
            new PropertyMetadata(0, new PropertyChangedCallback(OnHtmlChanged))
        );

        private static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var webView = d as WebView;

            if (webView != null)
            {
                webView.NavigationCompleted += async (sender, args) =>
                {
                    var value = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });
                    if (int.TryParse(value, out int height))
                    {
                        webView.Height = height;
                    }
                };

                webView.NavigateToString((string)e.NewValue);
            }
        }
    }
}
