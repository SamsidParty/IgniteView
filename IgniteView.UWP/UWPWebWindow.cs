using IgniteView.Core;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace IgniteView.UWP
{
    public class UWPWebWindow : WebWindow
    {
        #region Properties

        public override string Title { get; set; }
        public override string IconPath { get; set; }
        public override string URL { get => base.URL; set { base.URL = value; IgniteViewPage.Instance.WebView.Source = new Uri(base.URL); } }
        public override IntPtr NativeHandle { get => -1; }
        protected override bool TitleBarVisible { get; set; }

        public override WindowBounds Bounds
        {
            get => base.Bounds;
            set
            {
                base.Bounds = value;
            }
        }

        #endregion


        #region Virtual Method Overrides

        public override WebWindow Show()
        {
            base.Show();

            if (IgniteViewPage.Instance != null) {
                IgniteViewPage.Instance.WebView.CoreWebView2Initialized += (WebView2 sender, CoreWebView2InitializedEventArgs args) =>
                {
                    sender.CoreWebView2.WebMessageReceived += (sender, args) =>
                    {
                        var commandString = args.TryGetWebMessageAsString();
                        if (!string.IsNullOrEmpty(commandString))
                        {
                            ExecuteCommand(commandString);
                        }
                    };
                };

                IgniteViewPage.Instance.WebView.Visibility = Visibility.Visible;
                IgniteViewPage.Instance.WebView.Source = new Uri(CurrentAppManager.CurrentServerManager.BaseURL);
            }
            else
            {
                Task.Run(async () =>
                {
                    while (IgniteViewPage.Instance == null) { await Task.Delay(100); } // Wait for the main page to be loaded
                    await IgniteViewPage.Instance.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Show());  
                });
            }


            return this;
        }

        public override void Close()
        {
            base.Close();
            Application.Current.Exit();
        }

        public override void ExecuteJavaScript(string scriptData)
        {
            IgniteViewPage.Instance.WebView.CoreWebView2.ExecuteScriptAsync(JavaScriptConverter.WrapCode(scriptData));
            base.ExecuteJavaScript(scriptData);
        }

        #endregion

        public UWPWebWindow() : base()
        {

        }
    }
}
