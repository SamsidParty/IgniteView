using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework;
using WebFramework.Backend;
using Windows.UI.Xaml;

namespace WebFramework.UWP
{
    public class UWPWebWindow : WebWindow
    {
        public override async Task Init()
        {
            await base.Init();
            WebFrameworkPage.Instance.WebView.Source = new Uri("http://localhost:" + Server.HTTPPort + "/index.html");
            WebFrameworkPage.Instance.WebView.CoreWebView2Initialized += WebView_CoreWebView2Initialized;
        }

        private void WebView_CoreWebView2Initialized(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.UI.Xaml.Controls.CoreWebView2InitializedEventArgs args)
        {
            WebFrameworkPage.Instance.WebView.CoreWebView2.Settings.AreDevToolsEnabled = DevTools.Enabled;
            WebFrameworkPage.Instance.WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            MSGHandler.OnMessage(args.TryGetWebMessageAsString(), this);
        }

        //Override Lib To Use WebView2 Communication Method
        public override string OverrideLib(string lib)
        {
            Logger.LogInfo("Overriding Lib For UWP WebView2");
            return lib.Replace("window.external.sendMessage", "window.chrome.webview.postMessage").Replace("window.external.receiveMessage(", "window.chrome.webview.addEventListener('message', ");
        }

        public override async Task ExecuteJavascript(string js)
        {
            await WebFrameworkPage.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                WebFrameworkPage.Instance.WebView.CoreWebView2.ExecuteScriptAsync(js);
            });
        }

        public override async Task UpdateTitle(string title)
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Title = title;
        }

        public override async Task Close()
        {
            Application.Current.Exit();
        }
    }
}
