using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace IgniteView.UWP
{
    /// <summary>
    /// Renders a WebWindow using a WebView2 control.
    /// </summary>
    public sealed partial class IgniteViewPage : Page
    {
        public static IgniteViewPage Instance;

        private static TaskCompletionSource InstanceCreateSource = new TaskCompletionSource();
        private static TaskCompletionSource CoreWebViewCreateSource = new TaskCompletionSource();


        static async Task<WebView2> GetWebView()
        {
            await InstanceCreateSource.Task;
            await CoreWebViewCreateSource.Task;

            return Instance.MainWebView;
        }

        /// <summary>
        /// Safely mutates the WebView2 control and ensures it is initialized before use.
        /// </summary>
        public static async Task MutateWebView(Action<WebView2> mutateFn)
        {
            await InstanceCreateSource.Task;

            await Instance.Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                await GetWebView();
                mutateFn(Instance.MainWebView);
            });
        }

        public IgniteViewPage()
        {
            Instance = this;
            this.InitializeComponent();

            
            MainWebView.CoreWebView2Initialized += (sender, args) =>
            {
                CoreWebViewCreateSource.SetResult();
            };
            MainWebView.EnsureCoreWebView2Async();

            InstanceCreateSource.SetResult();
        }
    }
}