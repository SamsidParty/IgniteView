
namespace WebFramework.MAUI
{
    // All the code in this file is included in all platforms.
    public class MAUIWebWindow : WebWindow
    {
        public override async Task Init()
        {
            await base.Init();
            var src = new UrlWebViewSource();
            src.Url = "http://localhost:" + Server.HTTPPort + "/index.html";
            WebFrameworkPage.Instance.Source = src;
        }

        public override async Task ExecuteJavascript(string js)
        {
            WebFrameworkPage.Instance.Eval(js);
        }


    }
}
