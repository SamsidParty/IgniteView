using System;
using System.Diagnostics;
using System.Drawing;
using WebFramework;
using WebFramework.Test;
using WebFramework.Backend;
using System.Threading.Tasks;
using System.IO;

public class IVApplication
{

    static ThemeBasedColor TitlebarColor;

    public static void Main(string[] args)
    {
        AppManager.Validate(args, "SamsidParty", "IgniteViewTest"); // Required Or Crash
        App();
    }

    public static async Task App()
    {
        await Logger.SetFileName("main");
        Logger.ForceOpenConsole();
        DevTools.Enable();

        /*if (Directory.Exists("C:\\Users\\SamarthCat\\Documents\\Programming Stuff\\WebFramework\\WebFramework.Test\\WWW"))
        {
            //DevTools.HotReload("C:\\Users\\SamarthCat\\Documents\\Programming Stuff\\WebFramework\\WebFramework.Test\\WWW");
        }*/


        //Change Color Based On Theme (light, dark)
        TitlebarColor = new ThemeBasedColor(Color.FromArgb(255, 255, 255), Color.FromArgb(34, 34, 34));

        WindowManager.Options = new WindowOptions()
        {
            EnableAcrylic = true,
            TitlebarColor = TitlebarColor,
            StartWidthHeight = new Rectangle(0, 0, 1280, 720),
            NativeGamepadSupport = false
        };


        WebScript.Register<TestScript>("demo");
        await AppManager.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WWW"), OnReady);
    }

    public static async Task OnReady(WebWindow w)
    {
        w.BackgroundColor = TitlebarColor;
    }

}