using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework;
using WebFramework.Backend;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WebFramework.UWP
{
    public class UWPWebWindow : WebWindow
    {
        public override async Task Init()
        {
            await base.Init();

            WebFrameworkPage.Instance.WebView.CoreWebView2Initialized += WebView_CoreWebView2Initialized;
            WebFrameworkPage.Instance.WebView.Source = new Uri(AppManager.GetMainURL());

            if (!WindowManager.Options.NativeGamepadSupport)
            {
                WebFrameworkPage.Instance.RequiresPointer = RequiresPointer.WhenFocused;
            }
        }

        private void WebView_CoreWebView2Initialized(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.UI.Xaml.Controls.CoreWebView2InitializedEventArgs args)
        {
            //Set Dark Mode Enabled If Forced
            if (DarkMode.EnableForce)
            {
                WebFrameworkPage.Instance.WebView.CoreWebView2.Profile.PreferredColorScheme = Microsoft.Web.WebView2.Core.CoreWebView2PreferredColorScheme.Dark;
            }

            WebFrameworkPage.Instance.WebView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            WebFrameworkPage.Instance.WebView.CoreWebView2.Settings.AreDevToolsEnabled = DevTools.Enabled;
            WebFrameworkPage.Instance.WebView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            WebFrameworkPage.Instance.WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        }

        private void CoreWebView2_NavigationCompleted(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            WebFrameworkPage.Instance.WebView.Visibility = Visibility.Collapsed;
            WebFrameworkPage.Instance.WebView.Visibility = Visibility.Visible;
            WebFrameworkPage.Instance.WebView.Focus(FocusState.Keyboard);
        }

        private void CoreWebView2_WebMessageReceived(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            Update();
            MSGHandler.OnMessage(args.TryGetWebMessageAsString(), this);
        }

        void Update()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            var col = WindowManager.Options.TitlebarColor.Value;
            titleBar.BackgroundColor = Color.FromArgb(col.A, col.R, col.G, col.B);
            titleBar.ButtonBackgroundColor = Color.FromArgb(col.A, col.R, col.G, col.B);
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
            await CleanUp.RunCleanUpActions();
            Application.Current.Exit();
        }
    }
}
