using PhotinoNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework.PT
{
    public class PTWebWindow : WebWindow
    {

        public PhotinoWindow Native;

        public PTWebWindow(WindowOptions options) : base(options) { }

        public override async Task Init()
        {

            Logger.LogInfo("Creating PT");

            if (Options.EnableAcrylic)
            {
                Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
            }

            if (this == WindowManager.MainWindow)
            {
                Native = new PhotinoWindow();
            }
            else
            {
                Native = new PhotinoWindow((WindowManager.MainWindow as PTWebWindow).Native);
            }

            Native.UseOsDefaultSize = false;
            Native.SetLogVerbosity(0);
            Native.Title = "";
            Native.WebSecurityEnabled = false;

            RegisterNativeEvents();

            Native.SetSize(new System.Drawing.Size(Options.StartWidthHeight.Width, Options.StartWidthHeight.Height));
            Native.SetResizable(!Options.LockWidthHeight);
            Native.SetFullScreen(Options.Fullscreen);
            Native.SetChromeless(Options.Fullscreen);
            Native.Load(AppManager.GetMainURL());
            Logger.LogInfo("Starting PT");
            Native.WaitForClose();

            if (WindowManager.MainWindow == this)
            {
                await Close();
            }
        }

        public void ApplyIcon()
        {
            if (File.Exists(Options.IconPath)) { 

                //Convert Icon First, Then Apply It
                if (Platform.isWindowsPT)
                {
                    Native.SetIconFile(WindowsIconConverter.ConvertPngToIco(File.ReadAllBytes(Options.IconPath)));
                }
                else
                {
                    Native.SetIconFile(Options.IconPath);
                }
            }
        }

        public void RegisterNativeEvents()
        {
            Native.RegisterCustomSchemeHandler("fs", (object sender, string scheme, string url, out string contentType) => {

                var path = url.Replace("fs://", "");

                contentType = "application/octet-stream";
                if (File.Exists(path))
                {
                    if (SimpleHttpServer._mimeTypeMappings.ContainsKey(Path.GetExtension(path)))
                    {
                        contentType = SimpleHttpServer._mimeTypeMappings[Path.GetExtension(path)];
                    }
                    return new FileStream(path, FileMode.Open, FileAccess.Read);
                }
                return new MemoryStream(Encoding.UTF8.GetBytes("404 Not Found"));
            });

            //Register Interop
            Native.WebMessageReceived += async (s, e) =>
            {
                Update(Native);
                MSGHandler.OnMessage(e, this);
            };

            Native.WindowCreated += async (s, e) => {
                Jobs.Add(ApplyIcon, this as WebWindow);
                Update(Native);
            };
        }

        public override async Task Close()
        {
            await CleanUp.RunCleanUpActions();
            Environment.Exit(0);
        }

        public override async Task WindowReady()
        {
            await base.WindowReady();
        }

        void Update(PhotinoWindow w)
        {
            Logger.LogInfo("Recieved Update From PT");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var hwnd = w.WindowHandle;

                if (Options.EnableAcrylic)
                {
                    WinHelperLoader.Current.EnableMica(hwnd);
                }
                else
                {
                    WinHelperLoader.Current.SetTitlebarColor(hwnd, Options._WinTBC);   
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                MacHelper.Current.Init();
                MacHelper.Current.OnIconChanged(Options.IconPath);
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
                MacHelper.Current.OnTitleChanged(title);
            }
            
            Native.Title = title;
        }
    }
}
