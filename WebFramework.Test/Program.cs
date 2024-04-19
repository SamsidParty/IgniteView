using System;
using System.Diagnostics;
using System.Drawing;
using WebFramework;
using WebFramework.Test;

public class Program
{

    static ThemeBasedColor TitlebarColor;

    [STAThread] // Required On Windows Or Crash
    public static void Main(string[] args)
    {
        AppManager.Validate(args); // Required Or Crash

        App();
    }

    public static async Task App()
    {

        var systemPath = Environment.GetEnvironmentVariable ("Path", EnvironmentVariableTarget.Machine);
        var gtkPath = "/usr/local/Cellar/gtk+3/3.24.37";
        Environment.SetEnvironmentVariable ("Path", $"{gtkPath};{systemPath}");
        
        DevTools.Enable();
        //DevTools.HotReload("C:\\Users\\SamarthCat\\Documents\\Programming Stuff\\WebFramework\\WebFramework.Test\\WWW");

        //Change Color Based On Theme (light, dark)
        TitlebarColor = new ThemeBasedColor(Color.FromArgb(255, 255, 255), Color.FromArgb(34, 34, 34));

        WindowManager.Options = new WindowOptions() {
            TitlebarColor = TitlebarColor,
            StartWidthHeight = new Rectangle(0, 0, 1280, 720)
        };
        WebScript.Register<TestScript>("demo");
        await AppManager.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WWW"), OnReady);
    }

    public static async Task OnReady(WebWindow w)
    {
        w.BackgroundColor = TitlebarColor;
    }

}