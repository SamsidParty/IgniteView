using PhotinoNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework
{
    public class PTWebWindow : WebWindow
    {

        public PhotinoWindow Native;

        public override async Task Init()
        {
            await base.Init();
            Native = new PhotinoWindow();
            Native.UseOsDefaultSize = false;
            Native.SetLogVerbosity(0);
            Native.Title = "";
            if (File.Exists(WindowManager.Options.IconPath)) { Native.SetIconFile(WindowManager.Options.IconPath); }
            Native.RegisterCustomSchemeHandler("fs", (object sender, string scheme, string url, out string contentType) => {

                var path = url.Replace("fs://", "");

                contentType = "application/octet-stream";
                if (File.Exists(path)){
                    if (SimpleHttpServer._mimeTypeMappings.ContainsKey(Path.GetExtension(path))){
                        contentType = SimpleHttpServer._mimeTypeMappings[Path.GetExtension(path)];
                    }
                    return new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                return new MemoryStream(Encoding.UTF8.GetBytes("404 Not Found"));
            });
            Native.WebMessageReceived += async (s, e) =>
            {
                Update(Native);
                MSGHandler.OnMessage(e, this);
            };

            if (this == WindowManager.MainWindow)
            {
                Native.SetSize(new System.Drawing.Size(WindowManager.Options.StartWidthHeight.Width, WindowManager.Options.StartWidthHeight.Height));
                Native.SetResizable(!WindowManager.Options.LockWidthHeight);
                Native.Load("http://localhost:" + Server.HTTPPort + "/index.html");
                Native.WaitForClose();
                Process.GetCurrentProcess().Kill();
            }


        }

        void Update(PhotinoWindow w)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var hwnd = w.WindowHandle;
                PhotinoWindow.DwmSetWindowAttribute(hwnd, 35, new int[] { WindowManager.Options._WinTBC }, 4);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                MacHelper.Init();
                MacHelper.UpdateIcon(WindowManager.Options.IconPath);
            }
        }


        public override async Task ExecuteJavascript(string js)
        {
            Native.SendWebMessage(js);
        }

        public override async Task UpdateTitle(string title)
        {
            await base.UpdateTitle(title);

            if (this == WindowManager.MainWindow)
            {
                MacHelper.OnTitleChanged(title);
            }
            
            Native.Title = title;
        }
    }
}
