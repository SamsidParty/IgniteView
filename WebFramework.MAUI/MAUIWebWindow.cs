
using WebFramework.Backend;

namespace WebFramework.MAUI
{
    // All the code in this file is included in all platforms.
    public class MAUIWebWindow : WebWindow
    {
        public MAUIWebWindow(WindowOptions options) : base(options) { }

        public override async Task Init()
        {
            await base.Init();
            var src = new UrlWebViewSource();
            src.Url = AppManager.GetMainURL();
            WebFrameworkPage.Instance.Source = src;
        }

        public override async Task ExecuteJavascript(string js)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                WebFrameworkPage.Instance.Eval(js);
            });
        }

        public override async Task UpdateTitle(string title)
        {
            //Does Nothing, MAUI Doesn't Have Titles
        }

        public override async Task Close()
        {
            await CleanUp.RunCleanUpActions();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.Quit();
                Environment.Exit(0);
            });
        }
    }
}
