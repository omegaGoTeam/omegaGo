using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OmegaGo.UI.WindowsUniversal.Extensions.Xaml
{
    public class WebViewHtmlBinding
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
            "Html", typeof(string), typeof(WebViewHtmlBinding), new PropertyMetadata(default(string), OnHtmlChanged));

        private static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            WebView webView = dependencyObject as WebView;
            webView?.NavigateToString((string)dependencyPropertyChangedEventArgs.NewValue);
        }

        public static void SetHtml(DependencyObject element, string value)
        {
            element.SetValue(HtmlProperty, value);
        }

        public static string GetHtml(DependencyObject element)
        {
            return (string)element.GetValue(HtmlProperty);
        }
    }
}
