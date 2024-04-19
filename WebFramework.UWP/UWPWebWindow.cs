using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework;

namespace WebFramework.UWP
{
    public class UWPWebWindow : WebWindow
    {
        public override async Task Init()
        {
            await base.Init();
            WebFrameworkPage.Instance.WebView.Source = new Uri("http://localhost:" + Server.HTTPPort + "/index.html");
        }
    }
}
