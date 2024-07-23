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


        public override async Task Init()
        {
            await base.Init();

            Logger.LogInfo("Creating PT");

            if (WindowManager.Options.EnableAcrylic)
            {
                Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
            }

            Native = new PhotinoWindow();
            Native.UseOsDefaultSize = false;
            Native.SetLogVerbosity(0);
            Native.Title = "";

            RegisterNativeEvents();

            if (this == WindowManager.MainWindow)
            {
                Native.SetSize(new System.Drawing.Size(WindowManager.Options.StartWidthHeight.Width, WindowManager.Options.StartWidthHeight.Height));
                Native.SetResizable(!WindowManager.Options.LockWidthHeight);
                Native.Load(AppManager.GetMainURL());
                Logger.LogInfo("Starting PT");
                Native.WaitForClose();
                await Close();
            }
        }

        public void ApplyIcon()
        {
            if (File.Exists(WindowManager.Options.IconPath)) { 

                //Convert Icon First, Then Apply It
                if (Platform.isWindowsPT)
                {
                    Native.SetIconFile(WindowsIconConverter.ConvertPngToIco(File.ReadAllBytes(WindowManager.Options.IconPath)));
                }
                else
                {
                    Native.SetIconFile(WindowManager.Options.IconPath);
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

                if (WindowManager.Options.EnableAcrylic)
                {
                    WinHelperLoader.Current.EnableMica(hwnd);
                }
                else
                {
                    WinHelperLoader.Current.SetTitlebarColor(hwnd, WindowManager.Options._WinTBC);   
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                MacHelper.Current.Init();
                MacHelper.Current.OnIconChanged(WindowManager.Options.IconPath);
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
