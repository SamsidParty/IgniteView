using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.Backend;

namespace WebFramework
{
    //Mainly For Linux But Works On Other Platforms
    public class HelperWindow : Window
    {
        public static HelperWindow Instance;

        public HelperWindow() : base("IgniteView Helper Window")
        {
            SetDefaultSize(250, 250);
            SetPosition(WindowPosition.Center);

            DeleteEvent += delegate { Application.Quit(); };

            Show();
        }

        public static async Task<HelperWindow> Main()
        {
            try{
                Application.Init();
                Settings.Default.ApplicationPreferDarkTheme = true;
                Instance = new HelperWindow();
                Application.Run();
                return Instance;
            }
            catch (Exception ex){
                Logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
